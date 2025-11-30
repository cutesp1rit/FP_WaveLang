# Wave Language Tutorial

Welcome to the Wave programming language! This tutorial will guide you through the basics of Wave and help you write your first programs.

## Table of Contents

1. [Hello World](#hello-world)
2. [Basic Arithmetic](#basic-arithmetic)
3. [Defining Functions](#defining-functions)
4. [Working with Lists](#working-with-lists)
5. [Pattern Matching](#pattern-matching)
6. [Composition](#composition)
7. [Recursion](#recursion)
8. [Advanced Topics](#advanced-topics)

---

## Hello World

Let's start with the simplest program:

```wave
print("Hello, Wave!")
```

This uses the built-in `print` function to display a message.

---

## Basic Arithmetic

Wave supports basic arithmetic operations:

```wave
2 + 3       // 5
10 - 4      // 6
5 * 6       // 30
20 / 4      // 5
```

You can use parentheses for grouping:

```wave
(2 + 3) * 4  // 20
```

---

## Defining Functions

Functions are the heart of Wave. Define them using the `~` operator:

```wave
double ~ x ~ x * 2
```

Now you can call the function:

```wave
double(5)  // 10
```

### Multi-parameter Functions

Functions can have multiple parameters:

```wave
add ~ a ~ b ~ a + b

add(3, 7)  // 10
```

---

## Working with Lists

Lists are created with square brackets:

```wave
[1, 2, 3, 4, 5]
```

### List Operations

Wave provides several built-in functions for lists:

```wave
head([1, 2, 3])     // 1
tail([1, 2, 3])     // [2, 3]
sum([1, 2, 3, 4])   // 10
```

### Mapping Functions

Apply a function to each element of a list:

```wave
double ~ x ~ x * 2

[1, 2, 3] ~ double  // [2, 4, 6]
```

This is automatic mapping — when you apply a function to a list, it's applied to each element!

---

## Pattern Matching

Pattern matching allows you to define functions with multiple cases:

```wave
factorial ~ n
  n = 0 ~ 1
  _ ~ n * factorial(n - 1)
```

The `_` pattern matches anything. Patterns are checked from top to bottom.

### More Examples

```wave
sign ~ n
  n > 0 ~ 1
  n = 0 ~ 0
  _ ~ -1

sign(5)   // 1
sign(0)   // 0
sign(-3)  // -1
```

---

## Composition

The `~` operator is used for function composition:

```wave
double ~ x ~ x * 2
addOne ~ x ~ x + 1

3 ~ double ~ addOne  // (3 * 2) + 1 = 7
```

### Composition Operators

Wave offers different composition strategies:

#### Basic Composition (`~`)
```wave
5 ~ double ~ addOne
```

#### Strict Composition (`~~`)
Evaluates eagerly:
```wave
5 ~~ double ~~ addOne
```

#### Lazy Composition (`~>`)
Defers evaluation:
```wave
5 ~> double ~> addOne
```

#### Parallel Composition (`|~`)
Applies to list elements:
```wave
[1, 2, 3] |~ double |~ addOne  // [3, 5, 7]
```

---

## Recursion

Recursion is the primary way to iterate in Wave:

### Example 1: Sum of List

```wave
sum_list ~ xs
  [] ~ 0
  _ ~ head(xs) + sum_list(tail(xs))

sum_list([1, 2, 3, 4, 5])  // 15
```

### Example 2: Fibonacci

```wave
fib ~ n
  n = 0 ~ 0
  n = 1 ~ 1
  _ ~ fib(n - 1) + fib(n - 2)

fib(10)  // 55
```

### Example 3: List Length

```wave
length ~ xs
  [] ~ 0
  _ ~ 1 + length(tail(xs))

length([1, 2, 3, 4])  // 4
```

---

## Advanced Topics

### Higher-Order Functions

Functions that take other functions as parameters:

```wave
apply_twice ~ f ~ x ~ f(f(x))

inc ~ x ~ x + 1

apply_twice(inc, 5)  // inc(inc(5)) = 7
```

### Closures

Functions capture their environment:

```wave
make_multiplier ~ n ~ x ~ x * n

times3 ~ make_multiplier(3)
times3(10)  // 30
```

### Composing Multiple Functions

```wave
double ~ x ~ x * 2
square ~ x ~ x * x
addTen ~ x ~ x + 10

5 ~ double ~ square ~ addTen  // ((5 * 2)²) + 10 = 110
```

### Chaining List Operations

```wave
square ~ x ~ x * x
inc ~ x ~ x + 1

[1, 2, 3] ~ square ~ inc  // [2, 5, 10]
```

---

## Practice Exercises

### Exercise 1: Power Function

Write a function that calculates `x` to the power of `n`:

```wave
power ~ x ~ n
  n = 0 ~ 1
  _ ~ x * power(x, n - 1)

power(2, 5)  // 32
```

### Exercise 2: Filter Positives

Write a function to filter positive numbers from a list:

```wave
is_positive ~ x ~ x > 0

filter_positives ~ xs
  [] ~ []
  is_positive(head(xs)) ~ [head(xs)] ++ filter_positives(tail(xs))
  _ ~ filter_positives(tail(xs))

filter_positives([1, -2, 3, -4, 5])  // [1, 3, 5]
```

### Exercise 3: Maximum of List

Find the maximum element in a list:

```wave
max_list ~ xs
  [x] ~ x
  head(xs) > head(tail(xs)) ~ max_list([head(xs)] ++ tail(tail(xs)))
  _ ~ max_list(tail(xs))

max_list([3, 7, 2, 9, 4])  // 9
```

---

## Next Steps

Now that you've learned the basics of Wave, try:

1. Exploring the examples in the `examples/` folder
2. Writing your own programs
3. Experimenting with the REPL
4. Reading the [Language Guide](LANGUAGE_GUIDE.md) for more details
