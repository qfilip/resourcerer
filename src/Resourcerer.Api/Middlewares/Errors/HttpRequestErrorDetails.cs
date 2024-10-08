﻿using System.Text.Json;

namespace Resourcerer.Api.Middlewares.Errors;

public class HttpRequestErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
