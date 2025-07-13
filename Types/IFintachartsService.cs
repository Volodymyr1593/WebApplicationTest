namespace WebApplicationTest.Types
{
    public interface IFintachartsService
    {


        public Task<List<Asset>> GetAssets();
        public  List<Asset> ParceAsset(string json);
        public  Task RefreshAssets(List<Asset> assets);
        public Dictionary<string, PriceCacheEntry> GetPricesRealTime(string[] symbols);







     }
}
