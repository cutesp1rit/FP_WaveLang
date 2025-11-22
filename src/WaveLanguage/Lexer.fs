namespace WaveLanguage

/// Определение токенов для лексера
type Token =
    | Number of float
    | Identifier of string
    | Operator of string
    | LeftParen
    | RightParen
    | LeftBracket
    | RightBracket
    | Tilde
    | Equals
    | Comma

module Lexer =
    open System
    open System.Text.RegularExpressions

    /// Функция для токенизации строки
    let tokenize (input: string): Token list =
        let tokenPatterns: (string * (Match -> Token)) list = [
            @"\d+(\.\d+)?", (fun m -> Number (float m.Value))
            @"[a-zA-Z_]\w*", (fun m -> Identifier m.Value)
            @"[+\-*/]", (fun m -> Operator m.Value)
            @"\(", (fun _ -> LeftParen)
            @"\)", (fun _ -> RightParen)
            @"\[", (fun _ -> LeftBracket)
            @"\]", (fun _ -> RightBracket)
            @"~", (fun _ -> Tilde)
            @"=", (fun _ -> Equals)
            @",", (fun _ -> Comma)
            @"\r?\n", (fun _ -> Comma)  // Временно заменяем переносы строк на запятые
        ]

        let regex: Regex = Regex(String.Join("|", tokenPatterns |> List.map fst))
        let matches: MatchCollection = regex.Matches(input)

        [ for m in matches do
            let tokenType: Token option = 
                tokenPatterns
                |> List.tryPick (fun (pattern, creator) ->
                    if Regex.IsMatch(m.Value, pattern) then Some (creator m) else None)
            match tokenType with
            | Some token -> token
            | None -> failwithf "Неизвестный токен: %s" m.Value ]