using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using WebApplicationTest.Types;
using static System.Net.WebRequestMethods;

namespace WebApplicationTest.Services
{
    public class TokenService:ITokenService
    {

        private readonly HttpClient http;
        private readonly IMemoryCache cache;


        private TokenResponse tokenResponse;

        public TokenService(HttpClient http,IMemoryCache cache)
        {
            this.http = http;
            this.cache = cache;
        }


        public async Task<TokenResponse> GetTokenResponseAsync()
        {
            
             var request = new HttpRequestMessage(HttpMethod.Post, "https://platform.fintacharts.com/identity/realms/fintatech/protocol/openid-connect/token");

            var body = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", "app-cli" },
            { "username", "r_test@fintatech.com" },
            { "password", "kisfiz-vUnvy9-sopnyv" }
        };

            request.Content = new FormUrlEncodedContent(body);

            var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json);

            return tokenResponse;
            
        }
         
        public void SetAuthToken(TokenResponse tokenResponse)
        {
            if (tokenResponse != null)
            {
                cache.Set("AccessToken", tokenResponse.AccessToken,TimeSpan.FromMinutes(30));
            }
           
                
        }

        public void SetRefreshToken(TokenResponse tokenResponse)
        {
            if (tokenResponse != null)
            {
                cache.Set("RefreshToken", tokenResponse.RefreshToken, TimeSpan.FromHours(5));
            }
            
        }

        public async Task<string> RefreshAccessTokenAsync(string refreshToken)
        {
            var body = new Dictionary<string, string>
         {
             { "grant_type", "refresh_token" },
             { "client_id", "app-cli" },
             { "refresh_token", refreshToken }
          };

            var response = await http.PostAsync(
                "identity/realms/fintatech/protocol/openid-connect/token",
                new FormUrlEncodedContent(body)
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenResponse>(json);

            // зберігаємо в кеш
            cache.Set("access_token", token.AccessToken);
            cache.Set("refresh_token", token.RefreshToken); 

            return token.AccessToken;
        }

        public async Task<string> GetAccessToken()
        {
         
            if (cache.TryGetValue("AccessToken", out string accessToken))
            {
                return accessToken;
            }

            
            if (cache.TryGetValue("RefreshToken", out string refreshToken))
            {
                
                accessToken = await RefreshAccessTokenAsync(refreshToken);
                return accessToken;
            }

           
            var tokenResponse = await GetTokenResponseAsync();

           
            SetAuthToken(tokenResponse);

            SetRefreshToken(tokenResponse);

           
            if (cache.TryGetValue("AccessToken", out accessToken))
            {
                return accessToken;
            }


            return null;
        }












    }







}












