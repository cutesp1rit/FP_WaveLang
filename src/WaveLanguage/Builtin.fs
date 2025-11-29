#nowarn "25"

namespace WaveLanguage

module Builtins =
    open System

    let rec prettyPrint v =
        match v with
        | Value.List xs ->
            match xs with
            | [] -> printf "[]"
            | x::rest ->
                printf "["
                prettyPrint x
                rest |> List.iter (fun v -> printf ", "; prettyPrint v)
                printf "]"
        | Value.Number n -> printf "%g" n
        | Value.Fun _    -> printf "<fun>"
        | Value.BuiltinFun _ -> printf "<builtin>"

    let builtins : (string * Value) list = [
        // Одноаргументные
        ("print", Value.BuiltinFun (fun [v] -> prettyPrint v; printfn ""; v));
        ("inc",   Value.BuiltinFun (function [Value.Number x] -> Value.Number (x + 1.0) | _ -> failwith "inc expects a number"));
        ("dec",   Value.BuiltinFun (function [Value.Number x] -> Value.Number (x - 1.0) | _ -> failwith "dec expects a number"));
        ("square", Value.BuiltinFun (function [Value.Number x] -> Value.Number (x*x) | _ -> failwith "square expects a number"));
        
        // Для работы со списками
        ("head", Value.BuiltinFun (function 
            | [Value.List (x::_)] -> x 
            | _ -> failwith "head: non-empty list expected"));
            
        ("tail", Value.BuiltinFun (function 
            | [Value.List (_::xs)] -> Value.List xs 
            | _ -> failwith "tail: non-empty list expected"));
            
        ("sum", Value.BuiltinFun (function
            | [Value.List xs] ->
                xs 
                |> List.sumBy (function Value.Number x -> x | _ -> failwith "sum error: list contains non-number")
                |> Value.Number
            | _ -> failwith "sum: list expected"))
    ]