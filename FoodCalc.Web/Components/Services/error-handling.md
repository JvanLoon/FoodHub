# Error handling pipeline

Where the clean `ApiResult.Error` string comes from. The `Notify`/`OrDefault`
helpers documented in [README.md](README.md) rely on the error already being a
user-ready message by the time a component sees it — this is the code that makes
that true.

## The choke-point

Every API call goes through `AuthenticatedHttpClientService`. It:

1. Attaches the bearer token.
2. Sends the request.
3. **Never throws** — success or failure always comes back as an `ApiResult`.
4. On a non-success response, reads the body and recovers the server's real
   message (see below).
5. On an exception (network down, timeout, etc.), logs the full stack trace and
   returns a generic message.

```
Component → RecipeService / IngredientService / ... → AuthenticatedHttpClientService → API
   ApiResult ◄────────────────────────────────────────────────────────┘
```

Because the services are thin pass-throughs to the choke-point, none of them do
their own error handling — there is exactly one place that turns an HTTP failure
into a message.

## Turning a failed response into a message

`ExtractErrorAsync` reads the response body and hands it to `ParseServerMessage`,
which tries these in order and returns the first that yields text:

| Source | Shape | Notes |
| --- | --- | --- |
| ProblemDetails | `{ "detail": "..." }` | What `TypedResults.Problem(...)` produces. |
| Validation errors | `{ "errors": { "Field": ["msg", ...] } }` | FastEndpoints validation. All messages are joined with a space. Also accepts `{ "Field": "msg" }`. |
| Generic fallbacks | `{ "title" \| "message" \| "error": "..." }` | First non-empty one wins. |
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
| ≥ 500 | `ServerError` |
| other | `RequestFailed(status)` |

### Exceptions

If the request throws (rather than returning a non-success status), the exception
is logged at **Error** level with the full stack trace, and the caller gets
`WebConstants.Messages.Client.GenericFailure`. Non-success responses are logged at
**Warning** level with the method, URI, status, and resolved message.

## What this means for callers

- Components never parse response bodies or inspect `StatusCode` for messaging —
  `Error` is already the string to show. That is exactly what `Notify` puts on a
  toast.
- To change how a specific failure reads to the user, fix it **at the endpoint**
  (return a ProblemDetails `detail` or a validation message). The frontend picks
  it up automatically.
- The generic/exception path is deliberately vague (`GenericFailure`) so transport
  errors don't leak internals; the real detail is in the server logs.

## Related

- [README.md](README.md) — the `ApiResult` helpers (`Notify`, `OrDefault`,
  `OnSuccess`, `OnFailure`) that consume these results.
