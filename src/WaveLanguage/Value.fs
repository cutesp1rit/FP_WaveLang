namespace WaveLanguage

type Value =
    | Number of float
    | List of Value list
    | Fun of Env * string list * Expr
    | BuiltinFun of (Value list -> Value)

and Env = Map<string, Value>