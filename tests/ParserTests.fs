namespace WaveLanguage.Tests

open WaveLanguage
open WaveLanguage.Lexer
open WaveLanguage.Parser
open NUnit.Framework

[<TestFixture>]
module ParserTests =

    [<Test>]
    let ``Test parsing a number`` () =
        let tokens = [Token.Number 42.0]
        let result = parse tokens
        Assert.That(result, Is.EqualTo(Expr.Number 42.0))

    [<Test>]
    let ``Test parsing an identifier`` () =
        let tokens = [Token.Identifier "x"]
        let result = parse tokens
        Assert.That(result, Is.EqualTo(Expr.Identifier "x"))

    [<Test>]
    let ``Test parsing a simple lambda`` () =
        let tokens = [Token.Identifier "double"; Token.Tilde; Token.Identifier "x"; Token.Tilde; Token.Identifier "x"; Token.Operator "*"; Token.Number 2.0]
        let result = parse tokens
        let expected = Expr.Let("double", Expr.Lambda(["x"], Expr.BinaryOp("*", Expr.Identifier "x", Expr.Number 2.0)), Expr.Identifier "double")
        Assert.That(result, Is.EqualTo(expected))

    [<Test>]
    let ``Test parsing a function call`` () =
        let tokens = [Token.Identifier "factorial"; Token.LeftParen; Token.Number 5.0; Token.RightParen]
        let result = parse tokens
        let expected = Expr.Application(Expr.Identifier "factorial", [Expr.Number 5.0])
        Assert.That(result, Is.EqualTo(expected))

    [<Test>]
    let ``Test parsing binary operation`` () =
        let tokens = [Token.Number 2.0; Token.Operator "+"; Token.Number 3.0]
        let result = parse tokens
        let expected = Expr.BinaryOp("+", Expr.Number 2.0, Expr.Number 3.0)
        Assert.That(result, Is.EqualTo(expected))

    [<Test>]
    let ``Test parsing complex binary operation`` () =
        let tokens = [Token.Identifier "n"; Token.Operator "*"; Token.Identifier "factorial"; Token.LeftParen; Token.Identifier "n"; Token.Operator "-"; Token.Number 1.0; Token.RightParen]
        let result = parse tokens
        let expected = Expr.BinaryOp("*",
                                    Expr.Identifier "n",
                                    Expr.Application(Expr.Identifier "factorial",
                                                   [Expr.BinaryOp("-", Expr.Identifier "n", Expr.Number 1.0)]))
        Assert.That(result, Is.EqualTo(expected))

    [<Test>]
    let ``Test parsing factorial example`` () =
        let code = """factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)

factorial(5)"""
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Выводим результат для отладки
        printfn "Parsed AST: %A" result
        
        // Проверяем, что результат содержит вызов функции
        match result with
        | Expr.Application(Expr.Identifier "factorial", [Expr.Number 5.0]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался вызов функции factorial(5)")

    [<Test>]
    let ``Test tokenizing factorial example`` () =
        let code = """factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)"""
        let tokens = Lexer.tokenize code
        
        // Выводим токены для отладки
        printfn "Tokens: %A" tokens
        
        // Проверяем наличие ключевых токенов
        Assert.That(tokens |> List.contains (Token.Identifier "factorial"), Is.True)
        Assert.That(tokens |> List.contains Token.Tilde, Is.True)
        Assert.That(tokens |> List.contains (Token.Identifier "n"), Is.True)
        Assert.That(tokens |> List.contains Token.Equals, Is.True)
        Assert.That(tokens |> List.contains (Token.Number 0.0), Is.True)
        
    [<Test>]
    let ``Test parsing pattern matching in function`` () =
        let code = """fact ~ n
  n = 0 ~ 1
  _ ~ n * fact(n - 1)"""
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата
        match result with
        | Expr.Let("fact", Expr.Lambda(_, Expr.Match(_, _)), _) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидалось определение функции с паттерн-матчингом")
        
    [<Test>]
    let ``Test parsing multiple expressions with newlines`` () =
        let code = """double ~ x ~ x * 2

double(5)"""
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем, что результат содержит вызов функции
        match result with
        | Expr.Application(Expr.Identifier "double", [Expr.Number 5.0]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался вызов функции double(5)")
        
    [<Test>]
    let ``Test parsing function with pattern matching and function call`` () =
        let code = """factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)

factorial(5)"""
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем, что результат содержит вызов функции
        match result with
        | Expr.Application(Expr.Identifier "factorial", [Expr.Number 5.0]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался вызов функции factorial(5)")
        
    [<Test>]
    let ``Test parsing simple list`` () =
        let code = "[1, 2, 3]"
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата
        match result with
        | Expr.List([Expr.Number 1.0; Expr.Number 2.0; Expr.Number 3.0]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался список из трех чисел")
        
    [<Test>]
    let ``Test parsing empty list`` () =
        let code = "[]"
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата
        match result with
        | Expr.List([]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался пустой список")
        
    [<Test>]
    let ``Test parsing list with identifiers`` () =
        let code = "[x, y, z]"
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата
        match result with
        | Expr.List([Expr.Identifier "x"; Expr.Identifier "y"; Expr.Identifier "z"]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидался список из трех идентификаторов")
        
    [<Test>]
    let ``Test parsing function composition`` () =
        let code = "3 ~ double ~ addOne"
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата: addOne(double(3))
        match result with
        | Expr.Application(Expr.Identifier "addOne", [Expr.Application(Expr.Identifier "double", [Expr.Number 3.0])]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидалась композиция функций addOne(double(3))")
        
    [<Test>]
    let ``Test parsing list operations`` () =
        let code = "[1, 2, 3] ~ double"
        let tokens = Lexer.tokenize code
        let result = parse tokens
        
        // Проверяем, что парсинг не вызывает исключений
        Assert.That(result, Is.Not.Null)
        
        // Проверяем структуру результата: double([1, 2, 3])
        match result with
        | Expr.Application(Expr.Identifier "double", [Expr.List([Expr.Number 1.0; Expr.Number 2.0; Expr.Number 3.0])]) -> Assert.Pass()
        | _ -> Assert.Fail("Ожидалась операция над списком double([1, 2, 3])")