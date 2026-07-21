# Error handling pipeline

Where the clean `ApiResult.Errors` list comes from. The `OnFailure`/`OrDefault`
helpers documented in [README.md](README.md) rely on the errors already being
user-ready messages by the time a component sees them — this is the code that
makes that true.

`Errors` is a list, not a single string: the API reports *every* error it found,
and the client shows all of them (one toast each). Most responses still carry
exactly one.

## The choke-point

Every API call goes through `AuthenticatedHttpClientService`. It:

1. Attaches the bearer token.
2. Sends the request.
3. **Never throws** — success or failure always comes back as an `ApiResult`.
4. On a non-success response, reads the body and recovers the server's real
   messages (see below).
5. On an exception (network down, timeout, etc.), logs the full stack trace and
   returns a generic message.

```
Component → RecipeService / IngredientService / ... → AuthenticatedHttpClientService → API
   ApiResult ◄────────────────────────────────────────────────────────┘
```

Because the services are thin pass-throughs to the choke-point, none of them do
their own error handling — there is exactly one place that turns an HTTP failure
into a message.

## Turning a failed response into messages

`ExtractErrorsAsync` reads the response body and hands it to
`ParseServerMessages`, which tries these in order and returns the first that
yields text:

| Source | Shape | Notes |
| --- | --- | --- |
| ProblemDetails errors | `{ "errors": [{ "name": .., "reason": ".." }] }` | **The API's normal shape.** Every `reason` is kept as its own message. |
| Model-state errors | `{ "errors": { "Field": ["msg", ...] } }` | Legacy/third-party shape. All messages kept. Also accepts `{ "Field": "msg" }`. |
| Single-message shapes | `{ "detail" \| "title" \| "message" \| "error": "..." }` | First non-empty one wins. |
| Plain text | not JSON | Used directly (trimmed to 500 chars). |

If none of those produce anything, it falls back to a status-based message.

### Status fallbacks

When the body has no usable message, `StatusFallback` maps the HTTP status to a
constant from `WebConstants.Messages.Client`:

| Status | Message |
| --- | --- |
| 401 | `Unauthorized` |
| 403 | `Forbidden` |
| 404 | `NotFound` |
| 400 | `BadRequest` |
| 409 | `Conflict` |
| 500 | `ServerError` |
| other | `RequestFailed(status)` |

Note that only 500 exactly maps to `ServerError` — 502/503 and friends fall
through to `RequestFailed(status)`.

### Exceptions

If the request throws (rather than returning a non-success status), the exception
is logged at **Error** level with the full stack trace, and the caller gets
`WebConstants.Messages.Client.GenericFailure`. Non-success responses are logged at
**Warning** level — one entry per resolved message, each with the method, URI and
status.

## Where the API side produces this

`FoodCalc.Api/Common/ErrorResultExtensions.cs` gives endpoints
`this.SendErrorsAsync(errors, ct: ct)`, which pushes every `ErrorOr` error (or any
set of message strings) through FastEndpoints' validation-failure collection.
`Program.cs` configures `c.Errors.UseProblemDetails(p => p.AllowDuplicateErrors = true)`,
so validator failures and domain failures leave the API in the same RFC9457 shape.

`AllowDuplicateErrors` matters: domain errors all carry the same field name, and
without it only the first would reach the client — the exact problem this
replaced.

## What this means for callers

- Components never parse response bodies or inspect `StatusCode` for messaging —
  `Errors` already holds the strings to show. That is exactly what `OnFailure`
  puts on toasts, one per entry.
- To change how a specific failure reads to the user, fix it **at the endpoint**
  (the `Error.Failure(...)` description in the handler). The frontend picks it up
  automatically.
- To surface more than one problem at once, return multiple errors from the
  handler — nothing downstream truncates the list.
- The generic/exception path is deliberately vague (`GenericFailure`) so transport
  errors don't leak internals; the real detail is in the server logs.

## Related

- [README.md](README.md) — the `ApiResult` helpers (`OnSuccess`, `OnFailure`,
  `OrDefault`) that consume these results.
