namespace WaveLanguage.Tests

open NUnit.Framework
open WaveLanguage
open WaveLanguage.Lexer
open WaveLanguage.Parser
open WaveLanguage.Evaluator
open WaveLanguage.Builtins

module TestHelpers =
    let evalCode code =
        let tokens = Lexer.tokenize code
        let ast = Parser.parse tokens
        Evaluator.eval (builtins |> Map.ofList) ast

[<TestFixture>]
module EvaluatorTests =

    [<Test>]
    let ``Test simple arithmetic`` () =
        let v = TestHelpers.evalCode "2 + 3 * 4"
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(14.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number, но было %A" x)

    [<Test>]
    let ``Test function definition and application`` () =
        let code = """
        double ~ x ~ x * 2
        5 ~ double
        """
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(10.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 10.0, но было %A" x)

    [<Test>]
    let ``Test function composition`` () =
        let code = """
        double ~ x ~ x * 2
        inc ~ x ~ x + 1
        5 ~ double ~ inc
        """
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(11.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 11.0, но было %A" x)

    [<Test>]
    let ``Test list creation and map`` () =
        let code = """
        square ~ x ~ x * x
        [1, 2, 3] ~ square
        """
        let v = TestHelpers.evalCode code
        match v with
        | Value.List [Value.Number a; Value.Number b; Value.Number c] ->
            Assert.That(a, Is.EqualTo(1.0))
            Assert.That(b, Is.EqualTo(4.0))
            Assert.That(c, Is.EqualTo(9.0))
        | x -> Assert.Fail(sprintf "Ожидался список чисел, но получено: %A" x)

    [<Test>]
    let ``Test pattern matching factorial`` () =
        let code = """
        factorial ~ n
            n = 0 ~ 1
            _ ~ n * factorial(n - 1)
        factorial(5)
        """
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(120.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 120.0, но было %A" x)

    [<Test>]
    let ``Test empty list`` () =
        let v = TestHelpers.evalCode "[]"
        match v with
        | Value.List xs -> Assert.That(xs.IsEmpty)
        | x -> Assert.Fail(sprintf "Ожидался пустой список, но было %A" x)

    [<Test>]
    let ``Test nested lets and sequence`` () =
        let code = """
        double ~ x ~ x * 2
        triple ~ x ~ x * 3
        y ~ x ~ x + 10
        (2 ~ double ~ y) + (3 ~ triple ~ y)
        """
        let v = TestHelpers.evalCode code
        // (2*2+10) + (3*3+10) = 14 + 19 = 33
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(33.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 33.0, но было %A" x)

    [<Test>]
    let ``Test sum builtin`` () =
        let code = "[1, 2, 3, 4, 5] ~ sum"
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(15.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 15.0, но было %A" x)

    [<Test>]
    let ``Test head builtin`` () =
        let code = "[42, 100] ~ head"
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(42.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 42.0, но было %A" x)

    [<Test>]
    let ``Test tail builtin`` () =
        let code = "[42, 100, 7] ~ tail"
        let v = TestHelpers.evalCode code
        match v with
        | Value.List [Value.Number a; Value.Number b] ->
            Assert.That(a, Is.EqualTo(100.0))
            Assert.That(b, Is.EqualTo(7.0))
        | x -> Assert.Fail(sprintf "Ожидался [100.0, 7.0], но было %A" x)

    [<Test>]
    let ``Test inc builtin`` () =
        let code = "inc(5)"
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(6.0))
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 6.0, но было %A" x)

    [<Test>]
    let ``Test square builtin in composition`` () =
        let code = "3 ~ square ~ inc"
        let v = TestHelpers.evalCode code
        match v with
        | Value.Number n -> Assert.That(n, Is.EqualTo(10.0)) // 3*3 = 9, +1 = 10
        | x -> Assert.Fail(sprintf "Ожидалось Value.Number 10.0, но было %A" x)