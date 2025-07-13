using System.ComponentModel.DataAnnotations;

namespace WebApplicationTest.Models
{
    public class Asset
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("symbol")]
        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
        [Column("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [Column("kind")]
        [JsonPropertyName("kind")]
        public string? Kind { get; set; }
        [Column("currency")]
        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
        [Column("baseCurrency")]
        [JsonPropertyName("baseCurrency")]
        public string? BaseCurrency { get; set; }

    }
}
