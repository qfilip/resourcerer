using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Resourcerer.Api;

public static class AppStaticData
{
    public static class Auth
    {
        public static void Load(bool enabled) => Enabled = enabled;
        public static bool Enabled { get; private set; }
        public static class Jwt
        {
            public static SymmetricSecurityKey? Key { get; private set; }

            public static void Configure(string secretKey, string issuer, string audience)
            {
                Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                Issuer = issuer;
                Audience = audience;
            }

            public static string Issuer { get; private set; } = string.Empty;
            public static string Audience { get; private set; } = string.Empty;
            public const string UserId = "user_id";
            public const string UserName = "user_name";
            public const string DisplayName = "user_display_name";
            public const string IsAdmin = "is_admin";
            public const string CompanyId = "company_id";
            public const string CompanyName = "company_name";
        }

        public static class AuthorizationPolicy
        {
            public static string Admin { get; } = "admin";
            public static string Jwt { get; } = "JWT";
        }
    }
}

