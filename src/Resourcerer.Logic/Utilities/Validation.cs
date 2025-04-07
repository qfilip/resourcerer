using FluentValidation.Results;

namespace Resourcerer.Logic.Utilities;

internal static class Validation
{
    public readonly static ValidationResult Empty = new();

    private static bool ValidateString(string? value, int min, int max) =>
        value != null && value.Length >= min && value.Length <= max;
    public static class Example
    {
        public static bool Text(string? x) => ValidateString(x, 3, 50);
        public const string TextError = "Text must be between 3 and 50 characters long";
    }
}
