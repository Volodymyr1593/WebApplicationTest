namespace WebApplicationTest.Models
{
    public class AsssetResponse
    {


        [JsonPropertyName("data")]
        public List<Asset> Data { get; set; }

    }
}
