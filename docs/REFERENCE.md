# Wave Language Reference

This document provides a complete reference for the Wave programming language.

## Lexical Structure

### Tokens

Wave programs consist of the following token types:

- **Numbers**: `42`, `3.14`, `-17`
- **Identifiers**: `x`, `factorial`, `my_func`
- **Keywords**: None (Wave has no reserved keywords)
- **Operators**: `~`, `~~`, `~>`, `|~`, `+`, `-`, `*`, `/`, `=`, `>`, `<`
- **Delimiters**: `(`, `)`, `[`, `]`, `,`
- **Whitespace**: Spaces, tabs, newlines (ignored except for indentation)

### Comments

Wave currently does not support comments in the language syntax itself, but you can use the `print` function for documentation.

### Identifiers

Identifiers must start with a letter or underscore, followed by letters, digits, or underscores:

- Valid: `x`, `func1`, `my_function`, `_private`
- Invalid: `1func`, `my-func`, `func!`

---

## Type System

Wave is dynamically typed. Values have types at runtime:

### Value Types

- **Number**: Floating-point numbers (e.g., `42`, `3.14`)
- **List**: Ordered sequences of values (e.g., `[1, 2, 3]`)
- **Function**: First-class functions
- **BuiltinFun**: Built-in functions provided by the runtime

---

## Expressions

### Literals

#### Number Literals
```wave
42
3.14
-17
0.5
```

#### List Literals
```wave
[]              // Empty list
[1, 2, 3]       // List of numbers
[x, y, z]       // List of variables
[[1, 2], [3, 4]]  // Nested lists
```

### Identifiers

```wave
x
factorial
my_variable
```

### Binary Operations

#### Arithmetic

```wave
a + b    // Addition
a - b    // Subtraction
a * b    // Multiplication
a / b    // Division
a % b    // Modulo
```

**Precedence** (highest to lowest):
1. `*`, `/`, `%`
2. `+`, `-`

#### Comparison

```wave
a = b    // Equality
a > b    // Greater than
a < b    // Less than
a >= b   // Greater or equal
a <= b   // Less or equal
```

### Function Application

```wave
func(arg)           // Single argument
func(arg1, arg2)    // Multiple arguments
```

### Lambda Expressions

Lambdas are created using the `~` operator:

```wave
~ x ~ x * 2         // Anonymous function
```

### Let Expressions

```wave
let x = 5 in x * 2
```

### Pattern Matching

```wave
func ~ param
  pattern1 ~ result1
  pattern2 ~ result2
  _ ~ default_result
```

### Composition

#### Basic Composition
```wave
value ~ func1 ~ func2
```

Equivalent to `func2(func1(value))`.

#### Strict Composition
```wave
value ~~ func1 ~~ func2
```

#### Lazy Composition
```wave
value ~> func1 ~> func2
```

#### Parallel Composition
```wave
list |~ func1 |~ func2
```

---

## Statements

Wave is expression-based; there are no statements. Every construct returns a value.

### Sequences

Multiple expressions can be sequenced:

```wave
expr1
expr2
expr3
```

The value of the last expression is returned.

---

## Functions

### Function Definition

```wave
name ~ param ~ body
```

**Example:**
```wave
double ~ x ~ x * 2
```

### Multi-Parameter Functions

```wave
name ~ param1 ~ param2 ~ body
```

**Example:**
```wave
add ~ a ~ b ~ a + b
```

### Pattern Matching Functions

```wave
name ~ param
  pattern1 ~ result1
  pattern2 ~ result2
  _ ~ default
```

**Example:**
```wave
factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)
```

### Recursive Functions

Functions can call themselves:

```wave
sum_list ~ xs
  [] ~ 0
  _ ~ head(xs) + sum_list(tail(xs))
```

---

## Built-in Functions

### Arithmetic

- `inc(x)` — Increment by 1
- `dec(x)` — Decrement by 1
- `square(x)` — Square of x

### List Operations

- `head(list)` — First element
- `tail(list)` — All but first element
- `sum(list)` — Sum of all elements

### I/O

- `print(value)` — Print value to console

---

## Automatic Mapping

When a function is applied to a list, it's automatically mapped:

```wave
square ~ x ~ x * x

[1, 2, 3] ~ square  // [1, 4, 9]
```

This is equivalent to:

```wave
[square(1), square(2), square(3)]
```

---

## Scoping and Environments

### Lexical Scoping

Wave uses lexical (static) scoping. Variables are resolved based on where they're defined in the code.

### Closures

Functions capture their defining environment:

```wave
make_adder ~ n
  ~ x ~ x + n

add5 ~ make_adder(5)
add5(10)  // 15
```

---

## Evaluation Strategy

### Eager Evaluation

By default, Wave uses eager (strict) evaluation. Arguments are evaluated before being passed to functions.

### Composition Strategies

- **Strict (`~~`)**: Eager evaluation
- **Lazy (`~>`)**: Deferred evaluation
- **Parallel (`|~`)**: Parallel mapping

---

## Error Handling

Wave uses exceptions for error handling. Common errors:

- **Unbound variable**: Referencing an undefined variable
- **Type errors**: Incorrect types for operations (e.g., adding a number to a list)
- **Empty list errors**: Operations like `head` or `tail` on empty lists
- **Pattern match failure**: No matching pattern in pattern matching

---

## Grammar

```ebnf
<program>   ::= <expr>*

<expr>      ::= <number>
              | <identifier>
              | <list>
              | <lambda>
              | <application>
              | <let>
              | <match>
              | <binary_op>
              | <compose>

<number>    ::= ['-'] <digit>+ ['.' <digit>+]

<identifier> ::= <letter> (<letter> | <digit> | '_')*

<list>      ::= '[' [<expr> (',' <expr>)*] ']'

<lambda>    ::= '~' <identifier> '~' <expr>

<application> ::= <expr> '(' [<expr> (',' <expr>)*] ')'

<let>       ::= 'let' <identifier> '=' <expr> 'in' <expr>

<match>     ::= <identifier> '~' <identifier> (<pattern> '~' <expr>)+

<pattern>   ::= <expr> '=' <expr>
              | '_'

<binary_op> ::= <expr> <op> <expr>

<op>        ::= '+' | '-' | '*' | '/' | '%' | '=' | '>' | '<'

<compose>   ::= <expr> ('~' | '~~' | '~>' | '|~') <expr>
```

---

## Examples

### Factorial

```wave
factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)

factorial(5)  // 120
```

### Fibonacci

```wave
fib ~ n
  n = 0 ~ 0
  n = 1 ~ 1
  _ ~ fib(n - 1) + fib(n - 2)

fib(10)  // 55
```

### List Processing

```wave
double ~ x ~ x * 2
square ~ x ~ x * x

[1, 2, 3, 4, 5] ~ double    // [2, 4, 6, 8, 10]
[1, 2, 3, 4, 5] ~ square    // [1, 4, 9, 16, 25]
```

### Higher-Order Functions

```wave
apply_twice ~ f ~ x ~ f(f(x))

inc ~ x ~ x + 1
apply_twice(inc, 5)  // 7
```

---

## Implementation Details

Wave is implemented in F# and consists of:

- **Lexer** (`Lexer.fs`): Tokenization
- **Parser** (`Parser.fs`): AST construction
- **AST** (`AST.fs`): Abstract syntax tree definition
- **Evaluator** (`Evaluator.fs`): Interpretation
- **Builtins** (`Builtin.fs`): Standard library
- **REPL** (`REPL.fs`): Interactive environment

---

## Future Extensions

Possible future features:

- String type and string operations
- Tuple destructuring
- Module system
- Type annotations (optional)
- Tail call optimization
- Lazy sequences
- Exception handling constructs


