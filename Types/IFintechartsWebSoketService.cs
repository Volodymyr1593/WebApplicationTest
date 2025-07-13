namespace WebApplicationTest.Types
{
    public interface IFintechartsWebSoketService
    {

        public Task StartAsync();
        public Task<ClientWebSocket> SubscribeToWebSocket(List<string> symbols);
        public Task ListenAsync(ClientWebSocket socket);



    }
}
