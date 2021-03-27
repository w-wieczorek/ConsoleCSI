module NUnitTestGA

open System.Collections.Generic
open NUnit.Framework
open Parsing
open GA

[<Test>]
let ``Grammar from numbers `` () =
    let parameters = {
        pop_size = 0;
        tournament_size = 0;
        p_mutation = 0.0;
        iterations = 0;
        verbose = 0;
        rnd = null;
        grammar_size = 5;
        variables = 2;
        alphabet = ['a'; 'b'];
        examples = SortedSet [];
        counterexamples = SortedSet []
    }
    let V0 = Nonterminal(0) :> Symbol
    let V1 = Nonterminal(1) :> Symbol
    let a = Terminal('a') :> Symbol
    let b = Terminal('b') :> Symbol
    let r1 = {lhs = [V0]; rhs = [a]}
    let r2 = {lhs = [V1]; rhs = [b]}
    let r3 = {lhs = [V1]; rhs = [V0]}
    let r4 = {lhs = [V0]; rhs = [V0; V1]}
    let r5 = {lhs = [V1; V0]; rhs = [V1; V1]}
    let actual = indiv2Grammar parameters [0; 3; 6; 9; 20]
    Assert.That(actual.Start, Is.EqualTo V0, "Wrong start symbol")
    Assert.That(actual.Rules.Count, Is.EqualTo 5, "Wrong grammar size")
    Assert.That(actual.Rules.Contains(r1), Is.True, "r1 has to be inside")
    Assert.That(actual.Rules.Contains(r2), Is.True, "r2 has to be inside")
    Assert.That(actual.Rules.Contains(r3), Is.True, "r3 has to be inside")
    Assert.That(actual.Rules.Contains(r4), Is.True, "r4 has to be inside")
    Assert.That(actual.Rules.Contains(r5), Is.True, "r5 has to be inside")
