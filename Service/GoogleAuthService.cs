using Google.Apis.Auth;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Service
{
    public class GoogleAuthService
    {
        private readonly string _clientId;

        public GoogleAuthService(IConfiguration configuration)
        {
            _clientId = configuration["ApiSettings:GoogleClientId"] ?? string.Empty;
        }

        public async Task<GoogleJsonWebSignature.Payload?> ValidarTokenGoogleAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                };

                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                return null; // Token inválido, manipulado o expirado
            }
        }
    }
}
