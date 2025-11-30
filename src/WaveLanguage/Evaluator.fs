namespace WaveLanguage

module Evaluator =
    open WaveLanguage

    let rec eval (env: Env) (expr: Expr) : Value =
        
        let makeRecursiveFunc name paramNames body =
            let rec impl argVals =
                match argVals with
                | [Value.List xs] when List.length paramNames = 1 ->
                    Value.List (xs |> List.map (fun x -> impl [x]))
                
                | _ ->
                    if paramNames.Length <> argVals.Length then
                        failwithf "Function '%s' expected %d ingredients, got %d" name paramNames.Length argVals.Length
                    
                    let envWithArgs = 
                        List.zip paramNames argVals 
                        |> List.fold (fun e (k,v) -> Map.add k v e) env
                    
                    let envWithSelf = Map.add name (Value.BuiltinFun impl) envWithArgs
                    
                    eval envWithSelf body
            
            Value.BuiltinFun impl

        match expr with
        | Expr.Number n -> Value.Number n
        | Expr.Identifier x ->
            match Map.tryFind x env with
            | Some v -> v
            | None -> failwithf "Unbound variable: %s" x
        | Expr.List xs ->
            Value.List (List.map (eval env) xs)
        | StrictCompose (arg, f) ->
            let v = eval env arg
            let fv = eval env f
            apply fv [v]
        | LazyCompose (arg, f) ->
            let v = eval env arg
            let fv = eval env f
            apply fv [v]
        | ParallelCompose (arg, f) ->
            match eval env arg with
            | Value.List xs ->
                let fv = eval env f
                Value.List (xs |> List.map (fun v -> apply fv [v]))
            | v ->
                let fv = eval env f
                apply fv [v]
        | BinaryOp (op, a, b) ->
            let va, vb = eval env a, eval env b
            match op, va, vb with
            | "+", Value.Number x, Value.Number y -> Value.Number (x + y)
            | "-", Value.Number x, Value.Number y -> Value.Number (x - y)
            | "*", Value.Number x, Value.Number y -> Value.Number (x * y)
            | "/", Value.Number x, Value.Number y -> Value.Number (x / y)
            | "=", Value.Number x, Value.Number y -> if x = y then Value.Number 1.0 else Value.Number 0.0
            | _ -> failwithf "Operator '%s' error" op

        | Lambda (args, body) -> 
            Value.Fun (env, args, body)

        | Application (fexpr, argExprs) ->
            let fval = eval env fexpr
            let argVals = argExprs |> List.map (eval env)
            apply fval argVals

        | Expr.Let(name, valueExpr, bodyExpr) ->
            let v =
                match valueExpr with
                | Expr.Lambda(args, lambdaBody) ->
                    makeRecursiveFunc name args lambdaBody
                | _ ->
                    eval env valueExpr
            
            let env' = Map.add name v env
            eval env' bodyExpr

        | Match(exprToMatch, branches) ->
            let v = eval env exprToMatch
            let rec matchBranch branches =
                match branches with
                | [] -> failwith "No branch matched in 'match'"
                | (pattern, resultExpr) :: rest ->
                    match pattern, v with
                    | Expr.BinaryOp("=", Expr.Identifier param, Expr.Number n), Value.Number x when x = n ->
                        let env' = Map.add param (Value.Number x) env 
                        eval env' resultExpr
                    | Expr.Identifier "_", _ ->
                        eval env resultExpr
                    | _ -> matchBranch rest
            matchBranch branches

        | Sequence exprs ->
            let rec evalSeq env = function
                | [] -> Value.Number 0.0
                | [e] -> eval env e
                | e::rest ->
                    match e with
                    | Expr.Let(name, valueExpr, _) ->
                        let v = 
                            match valueExpr with
                            | Expr.Lambda(args, lambdaBody) ->
                                makeRecursiveFunc name args lambdaBody
                            | _ ->
                                eval env valueExpr
                        
                        let newEnv = Map.add name v env
                        evalSeq newEnv rest
                    
                    | _ -> 
                        ignore (eval env e)
                        evalSeq env rest
            evalSeq env exprs

    and apply f argVals =
        match f, argVals with
        | Value.Fun (closureEnv, paramNames, body), [Value.List xs] when paramNames.Length = 1 ->
            Value.List (xs |> List.map (fun v -> eval (Map.add paramNames.Head v closureEnv) body))
        
        | Value.Fun (closureEnv, paramNames, body), _ ->
            if paramNames.Length <> argVals.Length then failwithf "Function params mismatch"
            let env' = List.zip paramNames argVals |> List.fold (fun env (n,v)-> Map.add n v env) closureEnv
            eval env' body
        
        | Value.BuiltinFun fn, _ -> fn argVals
        
        | _ -> failwith "Not a function"
        
    let tryListMapCompose ast =
        match ast with
        | Expr.Application (Expr.Identifier fname, [Expr.List elems]) -> 
            Expr.List (List.map (fun el -> Expr.Application(Expr.Identifier fname, [el])) elems)
        | _ -> ast

    let rec evalWithEnv (env: Env) (expr: Expr) : Value * Env =
            match expr with
            | Expr.Let(name, valueExpr, bodyExpr) ->
                let value =
                    match valueExpr with
                    | Expr.Lambda(args, lambdaBody) ->
                        Value.Fun(env, args, lambdaBody)
                    | _ ->
                        eval env valueExpr
                let env' = Map.add name value env
                (value, env')
            | _ -> (eval env expr, env)
