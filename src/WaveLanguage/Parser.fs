namespace WaveLanguage

open WaveLanguage

/// Модуль для парсинга токенов в AST
module Parser =

    /// Исключение для ошибок парсинга
    exception ParseError of string

    /// Функция для парсинга списка токенов в AST
    let rec parse (tokens: Token list): Expr =
        parseProgram tokens

    /// Парсинг программы (последовательность выражений)
    and parseProgram (tokens: Token list): Expr =
        // Пропускаем запятые (переносы строк)
        let rec skipCommas tokens =
            match tokens with
            | Token.Comma :: rest -> skipCommas rest
            | _ -> tokens
            
        let rec parseAllExpressions tokens acc =
            let tokens = skipCommas tokens
            match tokens with
            | [] -> List.rev acc
            | _ ->
                let expr, remaining = parseTopLevel tokens
                match skipCommas remaining with
                | [] -> List.rev (expr :: acc)
                | rest -> parseAllExpressions rest (expr :: acc)
                
        let expressions = parseAllExpressions tokens []
        match expressions with
        | [] -> raise (ParseError "Пустая программа")
        | [single] -> single
        | multiple -> Expr.Sequence multiple

    /// Парсинг выражения верхнего уровня
    and parseTopLevel (tokens: Token list): Expr * Token list =
        // Пропускаем запятые (переносы строк)
        let rec skipCommas tokens =
            match tokens with
            | Token.Comma :: rest -> skipCommas rest
            | _ -> tokens
            
        let tokens = skipCommas tokens
        match tokens with
        // Определение функции: name ~ params ~ body
        | Token.Identifier name :: Token.Tilde :: rest ->
            parseFunctionDefinition name rest
        // Вызов функции или простое выражение
        | _ -> parseExpression tokens

    /// Парсинг определения функции
    and parseFunctionDefinition (name: string) (tokens: Token list): Expr * Token list =
        // Пропускаем запятые (переносы строк)
        let rec skipCommas tokens =
            match tokens with
            | Token.Comma :: rest -> skipCommas rest
            | _ -> tokens
            
        match tokens with
        | Token.Identifier param :: Token.Tilde :: rest ->
            // Обычное определение функции с телом
            let rest = skipCommas rest
            let body, remaining = parseFunctionBody rest
            Expr.Let(name, Expr.Lambda([param], body), Expr.Identifier name), remaining
        | Token.Identifier param :: rest when isPatternMatch (skipCommas rest) ->
            // Определение функции с паттерн-матчингом
            let rest = skipCommas rest
            let body, remaining = parseFunctionBody rest
            Expr.Let(name, Expr.Lambda([param], body), Expr.Identifier name), remaining
        | Token.Identifier param :: rest ->
            // Параметр без ~ - ошибка
            raise (ParseError "Ожидался символ ~ после параметра функции")
        | _ -> raise (ParseError "Ожидался параметр функции")

    /// Парсинг тела функции (может быть паттерн-матчинг)
    and parseFunctionBody (tokens: Token list): Expr * Token list =
        // Пропускаем запятые (переносы строк)
        let rec skipCommas tokens =
            match tokens with
            | Token.Comma :: rest -> skipCommas rest
            | _ -> tokens
            
        let rec parsePatterns acc tokens =
            let tokens = skipCommas tokens
            match tokens with
            | Token.Identifier var :: Token.Equals :: Token.Number n :: Token.Tilde :: rest ->
                // Паттерн: var = n ~ result
                let rest = skipCommas rest
                let result, remaining = parseExpression rest
                let pattern = Expr.BinaryOp("=", Expr.Identifier var, Expr.Number n)
                parsePatterns ((pattern, result) :: acc) remaining
            | Token.Identifier "_" :: Token.Tilde :: rest ->
                // Паттерн по умолчанию: _ ~ result
                let rest = skipCommas rest
                let result, remaining = parseExpression rest
                (Expr.Match(Expr.Identifier "n", List.rev ((Expr.Identifier "_", result) :: acc)), remaining)
            | _ ->
                match acc with
                | [] -> parseExpression tokens
                | _ ->
                    let result, remaining = parseExpression tokens
                    (Expr.Match(Expr.Identifier "n", List.rev ((Expr.Identifier "_", result) :: acc)), remaining)
        
        parsePatterns [] (skipCommas tokens)


    /// Упрощенная проверка паттерн-матчинга
    and isPatternMatch (tokens: Token list): bool =
        // Пропускаем запятые (переносы строк)
        let rec skipCommas tokens =
            match tokens with
            | Token.Comma :: rest -> skipCommas rest
            | _ -> tokens
            
        match skipCommas tokens with
        | Token.Identifier _ :: Token.Equals :: Token.Number _ :: Token.Tilde :: _ -> true
        | Token.Identifier "_" :: Token.Tilde :: _ -> true
        | _ -> false

    /// Парсинг выражения
    and parseExpression (tokens: Token list): Expr * Token list =
        parseComposition tokens

    /// Парсинг композиции функций
    and parseComposition (tokens: Token list): Expr * Token list =
        let left, rest = parseBinaryOp tokens
        match rest with
        | Token.Tilde :: rest2 ->
            // Парсинг композиции функций
            parseFunctionComposition left rest2
        | _ -> left, rest

    /// Парсинг композиции функций
    and parseFunctionComposition (leftExpr: Expr) (tokens: Token list): Expr * Token list =
        match tokens with
        | Token.Identifier funcName :: Token.Tilde :: rest ->
            // Еще одна композиция
            let applied = Expr.Application(Expr.Identifier funcName, [leftExpr])
            parseFunctionComposition applied rest
        | Token.Identifier funcName :: rest ->
            // Последняя функция в цепочке композиции
            let applied = Expr.Application(Expr.Identifier funcName, [leftExpr])
            applied, rest
        | _ ->
            let tokenStr = tokens |> List.truncate 3 |> List.map string |> String.concat " "
            raise (ParseError $"Ожидался идентификатор функции в композиции, получено: {tokenStr}...")

    /// Парсинг бинарных операций
    and parseBinaryOp (tokens: Token list): Expr * Token list =
        let left, rest = parsePrimary tokens
        match rest with
        | Token.Operator op :: rest2 ->
            let right, remaining = parseBinaryOp rest2
            Expr.BinaryOp(op, left, right), remaining
        | _ -> left, rest

    /// Парсинг первичных выражений
    and parsePrimary (tokens: Token list): Expr * Token list =
        match tokens with
        | Token.Number n :: rest ->
            Expr.Number n, rest
        | Token.Identifier id :: Token.LeftParen :: rest ->
            // Вызов функции
            parseApplication id rest
        | Token.Identifier id :: rest ->
            Expr.Identifier id, rest
        | Token.LeftParen :: rest ->
            let expr, remaining = parseExpression rest
            match remaining with
            | Token.RightParen :: tail -> expr, tail
            | _ -> raise (ParseError "Ожидалась закрывающая скобка")
        | Token.LeftBracket :: rest ->
            // Парсинг списка
            parseList rest
        | _ ->
            let tokenStr = tokens |> List.truncate 10 |> List.map string |> String.concat " "
            raise (ParseError $"Неожиданный токен в выражении. Контекст: {tokenStr}...")

    /// Парсинг вызова функции
    and parseApplication (funcName: string) (tokens: Token list): Expr * Token list =
        let rec parseArgs acc tokens =
            match tokens with
            | Token.RightParen :: rest -> (List.rev acc, rest)
            | Token.Comma :: rest -> parseArgs acc rest
            | _ ->
                let arg, remaining = parseExpression tokens
                match remaining with
                | Token.Comma :: rest -> parseArgs (arg :: acc) rest
                | Token.RightParen :: rest -> (List.rev (arg :: acc), rest)
                | _ ->
                    let tokenStr = remaining |> List.truncate 3 |> List.map string |> String.concat " "
                    raise (ParseError $"Ожидалась запятая или закрывающая скобка, получено: {tokenStr}...")
        
        let args, remaining = parseArgs [] tokens
        Expr.Application(Expr.Identifier funcName, args), remaining

    /// Парсинг списков
    and parseList (tokens: Token list): Expr * Token list =
        let rec parseListElements acc tokens =
            match tokens with
            | Token.RightBracket :: rest ->
                // Закрывающая скобка - конец списка
                Expr.List(List.rev acc), rest
            | Token.Comma :: rest ->
                // Пропускаем запятые
                parseListElements acc rest
            | _ ->
                // Парсим элемент списка
                let element, remaining = parseExpression tokens
                match remaining with
                | Token.Comma :: rest ->
                    // После элемента идет запятая - продолжаем
                    parseListElements (element :: acc) rest
                | Token.RightBracket :: rest ->
                    // После элемента идет закрывающая скобка - конец списка
                    Expr.List(List.rev (element :: acc)), rest
                | _ ->
                    let tokenStr = remaining |> List.truncate 3 |> List.map string |> String.concat " "
                    raise (ParseError $"Ожидалась запятая или закрывающая скобка в списке, получено: {tokenStr}...")
        
        parseListElements [] tokens