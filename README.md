# Context-Sensitive Grammar Induction by Genetic Search

This repository contains the genetic algorithm for context-sensitive language induction. Please read the presentation of the library below.

## Quick presentation

The library contains two main modules: `Parsing` and `GA`. The former represents [a Type-1 grammar](https://en.wikipedia.org/wiki/Chomsky_hierarchy#Type-1_grammars) alongside helper functions.
The latter is the implementation of [a genetic algorithm](https://en.wikipedia.org/wiki/Genetic_algorithm) with all required [genetic operators](https://en.wikipedia.org/wiki/Genetic_algorithm#Genetic_operators).

## Installation

You can install the library as the NuGet package, search the phrase: `ContextSensitiveGrammarInduction.GeneticSearch` (package source: nuget.org).

## Example of usage

```fsharp
let parameters = {
    pop_size = 200;
    tournament_size = 3;
    p_mutation = 0.01;
    iterations = 100;
    verbose = 20;
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
20: best bar = 7/16
40: best bar = 7/16
60: best bar = 7/16
80: best bar = 7/16
100: best bar = 7/16
100: best bar = 7/16
V0 V0 -> V0 V3
V1 -> a
V2 V3 -> V2 V2
V3 V0 -> V0 V2
V3 V2 -> V1 V1
V3 V3 -> V1 V1


with bar = 0,4375
```

## Documentation

To run the genetic algorithm, use the `runGA` function (from the `GA` module) that takes a set of parameters as an argument. The set is the type of `GAParams` and contains parameters values for the genetic algorithm and the configuration for an induced grammar. The function returns the best-found grammar as well as the best score (see the example above).

```fsharp
type GAParams = {
    
    // Population size for GA.
    pop_size : int;
    
    // Tournament size for the tournament selection method.
    tournament_size : int;

    // Mutation rate for GA.
    p_mutation : float;

    // Number of iteration for GA.
    iterations : int;

    // If the verbose is set more than zero then the program will print the best individual
    // in each n-th and the last iteration.
    verbose : int;

    // Radomizer for genetic operators (the crossover and mutation), it must be the `Random` type.
    rnd : Random;

    // ...
    grammar_size : int;

    // ...
    variables : int;

    // ...
    alphabet : char list;

    // The list of words that should be accepted by the induced grammar.
    examples : SortedSet<string>;

    // The list of words that should not be accepted by the induced grammar.
    counterexamples : SortedSet<string>
}
```