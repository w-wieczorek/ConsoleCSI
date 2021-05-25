module NUnitTestParsing

open NUnit.Framework
open Parsing
open System.Collections.Generic

(* A grammar that generates the language a^m b^m, m >= 1.
   It is context-free.
*)
let createGrammar1 () = 
    let S = Nonterminal(0) :> Symbol
    let a = Terminal('a') :> Symbol
    let b = Terminal('b') :> Symbol
    Type1Grammar(SortedSet [{lhs = [S]; rhs = [a; b]}; 
                            {lhs = [S]; rhs = [a; S; b]}], S :?> Nonterminal)

(* A grammar that generates the language a^m b^m c^m, m >= 1.
   It is context-sensitive.
*)
let createGrammar2 () = 
    let S = Nonterminal(0) :> Symbol
    let Q = Nonterminal(1) :> Symbol
    let a = Terminal('a') :> Symbol
    let b = Terminal('b') :> Symbol
    let c = Terminal('c') :> Symbol
    Type1Grammar(SortedSet [{lhs = [S]; rhs = [a; S; Q]};
                            {lhs = [S]; rhs = [a; b; c]};
                            {lhs = [b; Q; c]; rhs = [b; b; c; c]};
                            {lhs = [c; Q]; rhs = [Q; c]}], S :?> Nonterminal)

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

[<Test>]
let ``Positive words for g1 `` () =
    let g1 = createGrammar1 ()
    for word in ["ab"; "aabb"; "aaabbb"; "aaaaaabbbbbb"] do
        Assert.That(g1.accepts word, Is.True,
                        $"{word} has to be accepted by g1")

[<Test>]
let ``Negative words for g1 `` () =
    let g1 = createGrammar1 ()
    for word in ["a"; "b"; "aab"; "abb"; "ba"; "aaaaaabbbbbbb"] do
        Assert.That(g1.accepts word, Is.False,
                        $"{word} has to be rejected by g1")

[<Test>]
let ``Positive words for g2 `` () =
    let g2 = createGrammar2 ()
    for word in ["abc"; "aabbcc"; "aaabbbccc"; "aaaaaabbbbbbcccccc"] do
        Assert.That(g2.accepts word, Is.True,
                        $"{word} has to be accepted by g2")

[<Test>]
let ``Negative words for g2 `` () =
    let g2 = createGrammar2 ()
    for word in ["a"; "b"; "c"; "aabc"; "abbcc"; "ba"; "ac"; "aaaabbbbccccc"] do
        Assert.That(g2.accepts word, Is.False,
                        $"{word} has to be rejected by g2")           

[<Test>]
let ``Positive words for g3 `` () =
    let g3 = createGrammar3 ()
    for word in ["ab"; "abbabb"; "abbbabbbabbb"] do
        Assert.That(g3.accepts word, Is.True,
                        $"{word} has to be accepted by g3")

[<Test>]
let ``Negative words for g3 `` () =
    let g3 = createGrammar3 ()
    for word in ["a"; "b"; "ba"; "abab"; "abbabbabb"; "abbabbb"; "abbab"; "abbbabbb"] do
        Assert.That(g3.accepts word, Is.False,
                        $"{word} has to be rejected by g3")           

[<Test>]
let ``Language for g1 `` () =
    let g1 = createGrammar1 ()
    let expected = ["ab"; "aabb"; "aaabbb"]
    let actual = g1.language() |> Seq.take 3 |> List.ofSeq
    Assert.That(actual, Is.EqualTo expected, "Wrong Language 1.")

[<Test>]
let ``Language for g2 `` () =
    let g2 = createGrammar2 ()
    let expected = ["abc"; "aabbcc"; "aaabbbccc"]
    let actual = g2.language() |> Seq.take 3 |> List.ofSeq
    Assert.That(actual, Is.EqualTo expected, "Wrong Language 2.")

[<Test>]
let ``Language for g3 `` () =
    let g3 = createGrammar3 ()
    let expected = ["ab"; "abbabb"]
    let actual = g3.language() |> Seq.take 2 |> List.ofSeq
    Assert.That(actual, Is.EqualTo expected, "Wrong Language 3.")

[<EntryPoint>] 
let main argv =
    0
