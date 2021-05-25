# Context-Sensitive Grammar Induction by Genetic Search

This repository contains the genetic algorithm for context sensitive grammar induction. Please read the presentation of the library below.

## Quick presentation

The library contains two main modules: `Parsing` and `GA`. The former represents [a Type-1 grammar](https://en.wikipedia.org/wiki/Chomsky_hierarchy#Type-1_grammars) alongside with helper functions.
The latter is the implementaytion of [a genenetic algorithm](https://en.wikipedia.org/wiki/Genetic_algorithm) with all required [genetic operators](https://en.wikipedia.org/wiki/Genetic_algorithm#Genetic_operators).

## Example of usage

```fsharp
let parameters = {
    pop_size = 200;
    tournament_size = 3;
    p_mutation = 0.01;
    iterations = 100;
    verbose = 500;
    rnd = Random();
    grammar_size = 6;
    variables = 4;
    alphabet = ['a'; 'b'];
    examples = SortedSet ["ab"; "aabb"; "aaabbb"]
    counterexamples = SortedSet ["a"; "b"; "aa"; "bb"; "ba"; "abb"; "bba"; "abab"]
}
let grammar, bar = runGA parameters
printfn $"{grammar}"
printfn $"\nwith bar = {bar}"
```

```
[Output:]
100: best bar = 7/16
V0 -> b
V0 V0 -> V3 V0
V0 V1 -> V2 V0
V0 V3 -> V0 V0
V2 V0 -> V2 V3
V2 V1 -> V2 V0


with bar = 0,4375
```

