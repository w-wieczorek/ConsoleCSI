module Parsing

open System.Collections.Generic

[<AbstractClass; AllowNullLiteral>]
type Symbol () =
    abstract member name : string
    interface System.IComparable with
        member x.CompareTo yObj =
            match yObj with
            | :? Symbol as y -> x.name.CompareTo y.name
            | _ -> failwith "incompatible types"

type Terminal (c : char) =
    inherit Symbol ()
    member x.letter = c
    override x.name = string c

[<AllowNullLiteral>]
type Nonterminal (idx : int) =
    inherit Symbol ()
    member x.idx = idx
    override x.name = "V" + string idx

let sl2str (sep : string) (xs : Symbol list) : string =
    xs
    |> List.map (fun x -> x.name)
    |> String.concat sep

let str2sl (xs : string) : Symbol list =
    xs
    |> Seq.toList
    |> List.map (fun c -> Terminal(c) :> Symbol)

type Rule =
    {lhs : Symbol list; rhs : Symbol list}
    override rule.ToString () = sl2str " " rule.lhs + " -> " + sl2str " " rule.rhs

type Type1Grammar (rs : SortedSet<Rule>, initial : Nonterminal) =
    member val Rules = rs with get, set
    member val Start = initial with get, set

    member private g.appliedTo (s : Symbol list) : Symbol list list =
        let mutable result : Symbol list list = []
        let mutable rhs_size = 0
        let mutable i = 0
        for rule in g.Rules do
            rhs_size <- List.length rule.rhs
            i <- 0
            for subseq in List.windowed rhs_size s do
                if subseq = rule.rhs then
                    result <- (List.take i s @ rule.lhs @ List.skip (i + rhs_size) s) :: result
                i <- i + 1
        result

    member private g.derived (s : Symbol list) : Symbol list list =
        let mutable result : Symbol list list = []
        let mutable lhs_size = 0
        let mutable i = 0
        for rule in g.Rules do
            lhs_size <- List.length rule.lhs
            i <- 0
            for subseq in List.windowed lhs_size s do
                if subseq = rule.lhs then
                    result <- (List.take i s @ rule.rhs @ List.skip (i + lhs_size) s) :: result
                i <- i + 1
        result

    member g.accepts (s : string) : bool =
        let sentences = SortedSet<Symbol list>()
        let done_sentences = SortedSet<Symbol list>()
        sentences.Add(str2sl s) |> ignore
        let mutable changed = true
        while not (sentences.Contains([g.Start :> Symbol])) && changed do
            let copy = set sentences
            changed <- false
            for s in copy do
                if not (done_sentences.Contains s) then
                    for u in g.appliedTo s do
                        if sentences.Add(u) then
                            changed <- true
                    done_sentences.Add s |> ignore
        sentences.Contains([g.Start :> Symbol])  

    member g.language () = seq {
        let q = Queue<Symbol list>()
        q.Enqueue([g.Start :> Symbol])
        while q.Count > 0 do
            let sl = q.Dequeue()
            if List.forall (fun (sym : Symbol) -> sym :? Terminal) sl then
                yield sl2str "" sl
            else
                for u in g.derived sl do
                    q.Enqueue(u)
    }

    override g.ToString () =
        g.Rules
        |> List.ofSeq
        |> List.map (fun x -> x.ToString() + "\n")
        |> List.reduce (+)
