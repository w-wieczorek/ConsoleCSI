module GA

open System
open Parsing
open System.Collections.Generic
open MathNet.Numerics

type GAParams = {
    pop_size : int;
    tournament_size : int;
    p_mutation : float;
    iterations : int;
    verbose : int;
    rnd : Random;
    grammar_size : int;
    variables : int;
    alphabet : char list;
    examples : SortedSet<string>;
    counterexamples : SortedSet<string>
}

let calculateMaxRuleNumber (parameters : GAParams) : int =
    let n = parameters.variables
    let t = parameters.alphabet.Length
    n*(t + n + n*n) + pown n 4

let rndIndividual (parameters : GAParams) : int list =
    let r = calculateMaxRuleNumber parameters
    [for _ in 1 .. parameters.grammar_size -> parameters.rnd.Next(r)]

let rec crossover (rnd : Random) (ind1 : int list) (ind2 : int list) : int list =
    match ind1, ind2 with
    | x :: xs, y :: ys -> match x, y with
                          | _ when x = y -> x :: crossover rnd xs ys
                          | _ when x < y ->
                            let zs = crossover rnd xs ind2
                            if rnd.NextDouble() < 0.5 then x :: zs else zs
                          | _ -> 
                            let zs = crossover rnd ind1 ys
                            if rnd.NextDouble() < 0.5 then y :: zs else zs
    | (x :: xs), [] -> let zs = crossover rnd xs []
                       if rnd.NextDouble() < 0.5 then x :: zs else zs
    | [], (y :: ys) -> let zs = crossover rnd [] ys
                       if rnd.NextDouble() < 0.5 then y :: zs else zs
    | [], [] -> []

let mutate (rnd : Random) (max_val : int) (ind : int list) : int list =
    let a = Array.ofList ind
    let n = a.Length
    if n > 0 then
        let mutable r = rnd.Next(max_val)
        while Array.IndexOf(a, r) >= 0 do
            r <- rnd.Next(max_val)
        a.[rnd.Next(n)] <- r
        a |> Array.sort |> Array.toList
    else
        ind

let num2Rule (alphabet : char list) (n : int) (t : int) (x : int) : Rule =
    let term i = Terminal(List.item i alphabet) :> Symbol
    let nont i = Nonterminal(i) :> Symbol
    let m = n*(t + n + n*n) + pown n 4 - 1
    match x with
    | _ when x < n*t ->  // A -> a
                {lhs = [nont (x / t)]; rhs = [term (x % t)]}
    | _ when x >= n*t && x < n*t + n*n ->  // A -> A
                {lhs = [nont ((x - n*t) / n)]; rhs = [nont ((x - n*t) % n)]}
    | _ when x >= n*t + n*n && x < n*(t + n + n*n) ->  // A -> A B
                {lhs = [nont ((x - n*(t + n)) / (n*n))]; 
                 rhs = [nont (((x - n*(t + n)) % (n*n)) / n); nont (((x - n*(t + n)) % (n*n)) % n)]}
    | _ -> {lhs = [nont ((m - x) / (n*n*n)); nont (((m - x) / (n*n)) % n)]; 
            rhs = [nont (((m - x) / n) % n); nont ((m - x) % n)]}

let indiv2Grammar (parameters : GAParams) (ind : int list) : Type1Grammar = 
    let result = Type1Grammar(SortedSet [], null)
    let n = parameters.variables
    let t = parameters.alphabet.Length
    List.iter (fun x -> result.Rules.Add(num2Rule parameters.alphabet n t x) |> ignore) ind
    let min_rule = 
        try
            result.Rules 
            |> List.ofSeq
            |> List.filter (fun r -> List.length r.lhs = 1 && List.head r.lhs :? Nonterminal) 
            |> List.minBy (fun r -> (List.head r.lhs :?> Nonterminal).idx)
        with
            | :? ArgumentException -> {lhs = []; rhs = []}
    result.Start <- match min_rule.lhs with
                    | variable :: _ -> variable :?> Nonterminal
                    | [] -> null
    result

let eval (parameters : GAParams) (ind : int list) : BigRational =
    let grammar = indiv2Grammar parameters ind
    let mutable tp = 0N
    let mutable fn = 0N
    let mutable fp = 0N
    let mutable tn = 0N
    for s in parameters.examples do
        if grammar.accepts s then
            tp <- tp + 1N
        else
            fn <- fn + 1N
    for s in parameters.counterexamples do
        if grammar.accepts s then
            fp <- fp + 1N
        else
            tn <- tn + 1N
    let p = tp + fn
    let n = fp + tn
    let auc = (tp/p + tn/n)/2N
    if auc = 1N/2N then
        0N
    else
        auc

let runGA (parameters : GAParams) : Type1Grammar * float =
    let mrn = calculateMaxRuleNumber parameters
    let P = Array.init parameters.pop_size (fun _ -> rndIndividual parameters)
    let bar = Array.init parameters.pop_size (fun i -> eval parameters P.[i])
    assert (parameters.tournament_size >= 3)
    let T : int [] = Array.zeroCreate parameters.tournament_size
    let mutable max_bar = Array.max bar 
    let mutable iteration = 0
    while max_bar < 1N && iteration < parameters.iterations do
        iteration <- iteration + 1
        if parameters.verbose > 0 && iteration % parameters.verbose = 0 then
            printfn $"{iteration}: best bar = {max_bar}"
        for i = 0 to parameters.tournament_size - 1 do
            T.[i] <- parameters.rnd.Next(parameters.pop_size)
        Array.sortInPlaceWith (fun j k -> if bar.[j] < bar.[k] then -1 else 1) T
        let mutable new_ind = crossover parameters.rnd P.[T.[^0]] P.[T.[^1]]
        if parameters.rnd.NextDouble() < parameters.p_mutation then
            new_ind <- mutate parameters.rnd mrn new_ind
        P.[T.[0]] <- new_ind
        bar.[T.[0]] <- eval parameters new_ind
        if bar.[T.[0]] > max_bar then
            max_bar <- bar.[T.[0]]
    if parameters.verbose > 0 then
        printfn $"{iteration}: best bar = {max_bar}"
    let mutable i = 0
    while bar.[i] < max_bar do
        i <- i + 1
    indiv2Grammar parameters P.[i], float max_bar
