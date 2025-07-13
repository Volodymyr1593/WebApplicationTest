using Microsoft.Extensions.Caching.Memory;

namespace WebApplicationTest.Types
{
    public interface ITokenService
    {


        public  Task<TokenResponse> GetTokenResponseAsync();
        public void SetAuthToken(TokenResponse tokenResponse);
        public void SetRefreshToken(TokenResponse tokenResponse);

        public  Task<string> RefreshAccessTokenAsync(string refreshToken);
        public  Task<string> GetAccessToken();














    }
}
