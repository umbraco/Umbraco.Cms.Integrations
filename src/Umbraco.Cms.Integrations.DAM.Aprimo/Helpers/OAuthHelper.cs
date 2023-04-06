using System.Security.Cryptography;
using System.Text;

using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Extensions
{
    public class OAuthHelper
    {
        public static OAuthCodeExchange GenerateKeys()
        {
            var random = new Random();
            var state = random.Next(1000000).ToString("D6");

            var generator = RandomNumberGenerator.Create();

            var bytes = new byte[32];

            generator.GetBytes(bytes);

            var codeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            using var sh256 = SHA256.Create();

            var challengeBytes = sh256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

            var codeChallenge = Convert.ToBase64String(challengeBytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            return new OAuthCodeExchange
            {
                State = state,
                CodeVerifier = codeVerifier,
                CodeChallenge = codeChallenge
            };
        }
    }
}
