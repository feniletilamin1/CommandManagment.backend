using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommandManagment.backend
{
    public class AuthOptions
    {
        public const string ISSUER = "https://localhost:7138"; // издатель токена
        public const string AUDIENCE = "http://localhost:3000"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123uiwehduiwejdewjkdjewi";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
