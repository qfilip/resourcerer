using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Resourcerer.Api;

public static class AppStaticData
{
    public static class Jwt
    {
        public static SymmetricSecurityKey? Key { get; private set; }

        public static void SetJwtSecretKey(string secret)
        {
            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public static string Issuer { get; } = "Resourcerer.Api";
        public static string Audience { get; } = "Resourcerer.Api";
    }

    public static class AuthPolicy
    {
        public static string Admin { get; } = "admin";
        public static string User { get; } = "user";
    }
}

