// Learn more about F# at http://fsharp.org

open System
open Parsing
open GA
open System.Collections.Generic

[<EntryPoint>]
let main argv =
    let parameters = {
        pop_size = 200;
        tournament_size = 3;
        p_mutation = 0.01;
        iterations = 10000;
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
    0 // return an integer exit code
