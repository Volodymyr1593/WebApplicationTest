

namespace WebApplicationTest.Db
{
    public class ApplicationDbContext:DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Asset> assets { get; set; }
        //public DbSet<Price> Prices { get; set; }
       // public DbSet<AssetPriceDateTime> AssetPriceDateTime { get; set; }






    }

}

