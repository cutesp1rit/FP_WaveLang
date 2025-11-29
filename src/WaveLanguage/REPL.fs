namespace WaveLanguage

module REPL =
    open WaveLanguage
    open System

    /// Начальное окружение
    let initialEnv : Env =
        Builtins.builtins |> Map.ofList

    /// Простой REPL
    let rec repl (env: Env) =
        Console.Write("> ")
        let line = Console.ReadLine()
        match line with
        | null | ":q" -> ()
        | cmd when cmd.Trim() = "" -> repl env
        | code ->
            try
                let tokens = Lexer.tokenize code
                let ast = Parser.parse tokens
                let value = Evaluator.eval env ast
                Builtins.prettyPrint value
                printfn ""
                repl env
            with e ->
                printfn "Error: %s" e.Message
                repl env