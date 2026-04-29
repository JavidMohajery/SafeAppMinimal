module Demos.AuthDemo

open System
open System.Security.Cryptography
open System.Text
open System.Text.Json
open Giraffe

[<CLIMutable>]
type LoginDto = { Username: string; Password: string }

let private users =
    [ "admin", ("password123", "admin")
      "user",  ("letmein",     "member") ]
    |> Map.ofList

let private secretBytes = Encoding.UTF8.GetBytes("demo-jwt-secret-key-do-not-use-in-prod")

let private b64url (bytes: byte[]) =
    Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')

let private b64urlStr (s: string) = b64url (Encoding.UTF8.GetBytes(s))

let private makeJwt (username: string) (role: string) =
    let header   = """{"alg":"HS256","typ":"JWT"}"""
    let now      = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    let payload  =
        sprintf """{"sub":"%s","name":"Demo User","role":"%s","iat":%d,"exp":%d}"""
            username role now (now + 3600L)
    let unsigned = b64urlStr header + "." + b64urlStr payload
    use hmac     = new HMACSHA256(secretBytes)
    let hmacSig  = b64url (hmac.ComputeHash(Encoding.UTF8.GetBytes(unsigned)))
    unsigned + "." + hmacSig

let private verifyJwt (token: string) =
    let parts = token.Split('.')
    if parts.Length <> 3 then Error "Invalid token format"
    else
        let unsigned = parts.[0] + "." + parts.[1]
        use hmac     = new HMACSHA256(secretBytes)
        let expected = b64url (hmac.ComputeHash(Encoding.UTF8.GetBytes(unsigned)))
        if expected <> parts.[2] then Error "Invalid signature"
        else
            try
                let padding = (4 - parts.[1].Length % 4) % 4
                let padded  = parts.[1].Replace('-', '+').Replace('_', '/') + String('=', padding)
                let doc     = JsonDocument.Parse(Convert.FromBase64String(padded))
                let root    = doc.RootElement
                let exp     = root.GetProperty("exp").GetInt64()
                if DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp then Error "Token expired"
                else
                    Ok {| sub  = root.GetProperty("sub").GetString()
                          role = root.GetProperty("role").GetString() |}
            with ex -> Error ex.Message

// ── Handlers ──────────────────────────────────────────────────────────────────

let login : HttpHandler = fun next ctx ->
    task {
        let! dto = ctx.BindJsonAsync<LoginDto>()
        let username = if isNull dto.Username then "" else dto.Username
        match users |> Map.tryFind username with
        | Some (pw, role) when pw = dto.Password ->
            let token = makeJwt username role
            return! json {| token = token; tokenType = "Bearer"; expiresIn = 3600 |} next ctx
        | _ ->
            return!
                (setStatusCode 401 >=>
                    json {| error = "Invalid credentials"; hint = "Try admin / password123" |})
                    next ctx
    }

let whoami : HttpHandler = fun next ctx ->
    let authHeader =
        match ctx.Request.Headers.TryGetValue("Authorization") with
        | true, v -> v.ToString()
        | _ -> ""
    if authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) then
        let token = authHeader.Substring("Bearer ".Length).Trim()
        match verifyJwt token with
        | Ok claims ->
            json {| username = claims.sub; role = claims.role; message = "Token is valid" |} next ctx
        | Error reason ->
            (setStatusCode 401 >=> json {| error = reason |}) next ctx
    else
        (setStatusCode 401 >=>
            json {| error = "Missing Authorization header"; expected = "Bearer <token>" |})
            next ctx
