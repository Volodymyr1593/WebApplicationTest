using Microsoft.Extensions.Caching.Memory;
using System;
using WebApplicationTest.Types;

namespace WebApplicationTest.Services
{
    public class FintechartsWebSoketService : IFintechartsWebSoketService
    {

        private readonly IMemoryCache cache;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ITokenService token;
        public FintechartsWebSoketService(IMemoryCache cache,IServiceScopeFactory scopeFactory, ITokenService token)
        {
            this.scopeFactory = scopeFactory;
            this.cache = cache;
            this.token = token;
        }


        public async Task StartAsync()
        {
            
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var symbols = await context.assets.Select(a => a.Symbol).ToListAsync();

            var soket = await SubscribeToWebSocket(symbols);

             await ListenAsync(soket);

        }

        public async Task<ClientWebSocket> SubscribeToWebSocket(List<string> symbols)
        {
            ClientWebSocket webSocket = new ClientWebSocket();

            var     webtoken = await token.GetAccessToken();

            if (! string.IsNullOrEmpty(webtoken)) {

                webSocket.Options.SetRequestHeader("Authorization", $"Bearer {webtoken}");
                await webSocket.ConnectAsync(Consts.WebSocketUri, CancellationToken.None);


                var subscribeMessage = new
                {
                    type = "subscribe",
                    symbols = symbols
                };

                var json = JsonSerializer.Serialize(subscribeMessage);
                var buffer = Encoding.UTF8.GetBytes(json);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                return webSocket;

            }
            else
            {
                throw new Exception("WebSocket token is missing");
            }

        }


        public async Task ListenAsync(ClientWebSocket socket)
        {
            var buffer = new byte[4096];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine($"Received: {json}");

                try
                {
                    var data = JsonSerializer.Deserialize <PriceData>(json);

                    if (!string.IsNullOrEmpty(data?.Symbol))
                    {
                        var cacheEntry = new PriceCacheEntry
                        {
                            Price = data,
                            LastUpdate = DateTime.UtcNow
                        };

                        cache.Set(data.Symbol, cacheEntry,TimeSpan.FromMinutes(10)); // приклад TTL
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ JSON parse error: {ex.Message}");

                }




            }

        }











    }
}
