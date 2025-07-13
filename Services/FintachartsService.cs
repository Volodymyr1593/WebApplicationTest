using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;
using WebApplicationTest.Types;

namespace WebApplicationTest.Services
{
    public class FintachartsService:IFintachartsService
    {
     private readonly ApplicationDbContext context;
     private readonly HttpClient http;
     private readonly IMemoryCache cache;
     private readonly ITokenService tokens;
        public FintachartsService( ApplicationDbContext context,HttpClient http, IMemoryCache cache,ITokenService tokens )
        {
            this.context = context;
            this.http = http;
            this.cache = cache; 
            this.tokens = tokens;
        }

         public async Task<List<Asset>> GetAssets()


        {
           
            var request = new HttpRequestMessage(HttpMethod.Get,Consts.assetsUrl);
            var token = await tokens.GetAccessToken();


            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await http.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return ParceAsset(json);
            }
            else
            {
                throw new UnauthorizedAccessException("Token is missing");
            }





        }
        public List<Asset> ParceAsset(string json)
        {

            var parsed = JsonSerializer.Deserialize<AsssetResponse>(json);
            return parsed.Data ?? new List<Asset>();

        }








         public async Task RefreshAssets(List<Asset>assets)
        {
            if (assets != null&& assets.Any())
            {
                await context.Database.ExecuteSqlRawAsync("DELETE FROM assets");
                await context.SaveChangesAsync();
                await context.assets.AddRangeAsync(assets);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Seved assets fail");
            }

        }

        public Dictionary<string, PriceCacheEntry> GetPricesRealTime(string[] symbols)
        {
            var result = new Dictionary<string, PriceCacheEntry>();

            foreach (var symbol in symbols)
            {
                if (cache.TryGetValue(symbol, out PriceCacheEntry entry))
                {
                    result[symbol] = entry;
                }
            }

            return result;
        }











    }
}
