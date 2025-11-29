namespace WaveLanguage

open System
open System.IO
open WaveLanguage
open WaveLanguage.Lexer
open WaveLanguage.Parser


module Program =

    // Флаг для отладочного режима (печать токенов/AST вместо вычисления)
    let isDebugArg (arg:string) =
        arg = "--debug" || arg = "-d"

    [<EntryPoint>]
    let main argv =
        try
            match Array.toList argv with
            | [filename; flag] when isDebugArg flag ->
                // Запуск с файлом и флагом --debug
                if not (File.Exists filename) then
                    printfn "Файл не найден: %s" filename
                    1
                else
                    let code = File.ReadAllText(filename)
                    printfn "Файл: %s" filename
                    printfn "Исходный код:\n%s" code
                    printfn "\n-------------------\n"
                    let tokens = Lexer.tokenize code
                    printfn "Токены:\n%A" tokens
                    printfn "\n-------------------\n"
                    let ast = Parser.parse tokens
                    printfn "AST:\n%A" ast
                    0

            | [filename] ->
                // Запуск только файла — вычисление результата
                if not (File.Exists filename) then
                    printfn "Не найден файл: %s" filename
                    1
                else
                    let code = File.ReadAllText(filename)
                    let tokens = Lexer.tokenize code
                    let ast = Parser.parse tokens
                    let result = Evaluator.eval (Builtins.builtins |> Map.ofList) ast
                    Builtins.prettyPrint result
                    printfn ""
                    0

            | [] ->
                printfn "Wave REPL. Используйте :q для выхода. (Для запуска файла: wave.exe myprog.wave [--debug])"
                REPL.repl (Builtins.builtins |> Map.ofList)
                0

            | _ ->
                printfn "Использование:\n    wave.exe <файл.wave> [--debug]\n    или без аргументов для REPL."
                1
        with
        | Parser.ParseError msg ->
            printfn "Ошибка парсинга: %s" msg
            1
        | ex ->
            printfn "Ошибка: %s" ex.Message
            // Можно убрать stack trace для пользователя:
            // printfn "Stack trace: %s" ex.StackTrace
            1