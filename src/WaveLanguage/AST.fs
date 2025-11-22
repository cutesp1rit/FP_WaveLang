namespace WaveLanguage

/// Определение типов для абстрактного синтаксического дерева (AST)
type Expr =
    | Number of float
    | Identifier of string
    | Lambda of string list * Expr
    | Application of Expr * Expr list
    | Let of string * Expr * Expr
    | Match of Expr * (Expr * Expr) list
    | List of Expr list
    | BinaryOp of string * Expr * Expr
    | Sequence of Expr list