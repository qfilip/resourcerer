using FluentValidation.Results;

namespace Resourcerer.Logic.Utilities;

internal static class Validation
{
    public readonly static ValidationResult Empty = new();

    private static bool ValidateString(string? value, int min, int max) =>
        value != null && value.Length >= min && value.Length <= max;
    public static class Category
    {
        public static bool Name(string? x) => ValidateString(x, 3, 50);
        public const string NameError = "Category name must be between 3 and 50 characters long";
    }

    public static class Company
    {
        public static bool Name(string? x) => ValidateString(x, 3, 50);
        public const string NameError = "Company name must be between 1 and 50 characters long";
    }

    public static class Item
    {
        public static bool Name(string? x) => ValidateString(x, 2, 50);
        public const string NameError = "Item name must be between 2 and 50 characters long";

        public static bool Price(double x) => x >= 0;
        public const string PriceError = "Item price name cannot be negative value";

        public static bool ProductionPrice(double x) => x >= 0;
        public const string ProductionPriceError = "Item production price cannot be negative value";

        public static bool ProductionTimeSeconds(double x) => x >= 0;
        public const string ProductionTimeSecondsError = "Item production time cannot be negative value";

        public static bool ExpirationTimeSeconds(double? x) => x == null ? true : x >= 0;
        public const string ExpirationTimeSecondsError = "Item expiration time cannot be negative value";
    }

    public static class UnitOfMeasure
    {
        public static bool Name(string? x) => ValidateString(x, 2, 50);
        public const string NameError = "Unit of measure name must be between 2 and 50 characters long";

        public static bool Symbol(string? x) => ValidateString(x, 1, 12);
        public const string SymbolError = "Unit of measure symbol must be between 1 and 12 characters long";
    }

    public static class AppUser
    {
        public static bool Name(string? x) => ValidateString(x, 2, 50);
        public const string NameError = "User name must be between 2 and 50 characters long";

        public static bool Password(string? x) => ValidateString(x, 1, 500);
        public const string PasswordError = "User password must be between 1 and 500 characters long";

        public static bool Permissions(Dictionary<string, string[]>? x) => x == null ? false : x.Keys.Count() > 0;
        public const string PermissionsError = "User must have at least one permission section available";
    }
}
