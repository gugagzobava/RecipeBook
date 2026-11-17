using System.Text.Json.Serialization;

namespace RecipeBook.Api.Common;

/// <summary>Unified error body used for every 4xx response.</summary>
public class ApiError
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Errors { get; set; }

    public ApiError(int status, string message, Dictionary<string, string[]>? errors = null)
    {
        Status = status;
        Message = message;
        Errors = errors;
    }
}

public static class ErrorKeys
{
    /// <summary>"Steps[0].Instruction" -> "steps[0].instruction"; "$.difficulty" -> "difficulty".</summary>
    public static string Normalize(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return "body";
        if (key.StartsWith("$.")) key = key[2..];
        return string.Join('.', key.Split('.').Select(s => s.Length > 0 ? char.ToLowerInvariant(s[0]) + s[1..] : s));
    }
}
