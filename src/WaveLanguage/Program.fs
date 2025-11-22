namespace WaveLanguage

open System.IO
open WaveLanguage.Lexer
open WaveLanguage.Parser

module Program =
    [<EntryPoint>]
    let main argv =
        try
            let filename = 
                if argv.Length > 0 then argv.[0]
                else "examples/simple.wave"
                
            if not (File.Exists filename) then
                printfn "Файл не найден: %s" filename
                1
            else
                let code = File.ReadAllText(filename)
                printfn "Файл: %s" filename
                printfn "Исходный код:"
                printfn "%s" code
                printfn "\n-------------------\n"
                
                let tokens = Lexer.tokenize code
                printfn "Токены:"
                tokens |> List.iter (fun token -> printf "%A " token)
                printfn "\n\n-------------------\n"
                
                let ast = Parser.parse tokens
                printfn "AST (Абстрактное синтаксическое дерево):"
                printfn "%A" ast
                
                0
        with
        | ParseError msg ->
            printfn "Ошибка парсинга: %s" msg
            1
        | ex ->
            printfn "Ошибка: %s" ex.Message
            printfn "Stack trace: %s" ex.StackTrace
            1
