// Learn more about F# at http://fsharp.org

open System
open Parsing
open System.Collections.Generic

(* A grammar that generates (a b^m)^m, m >= 1.
   It is context-sensitive.
 *)
let createGrammar3 () =
    let S = Nonterminal(0) :> Symbol
    let A = Nonterminal(1) :> Symbol
    let D = Nonterminal(2) :> Symbol
    let T = Nonterminal(3) :> Symbol
    let a = Terminal('a') :> Symbol
    let b = Terminal('b') :> Symbol
    Type1Grammar(SortedSet [{lhs = [S]; rhs = [D; T; A]};
                            {lhs = [S]; rhs = [a; b]};
                            {lhs = [T]; rhs = [D; T; a]};
                            {lhs = [T]; rhs = [D; a]};
                            {lhs = [D; a]; rhs = [a; b; D]};
                            {lhs = [D; b]; rhs = [b; D]};
                            {lhs = [D; A]; rhs = [A; b]};
                            {lhs = [A]; rhs = [a]}], S :?> Nonterminal)
[<EntryPoint>]
let main argv =
    let g = createGrammar3 ()
    let actual = g.language() |> Seq.take 5 |> List.ofSeq
    printfn $"{g}"
    printf $"{actual}"
    0 // return an integer exit code
