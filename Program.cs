

using WebApplicationTest.Services;
using WebApplicationTest.Types;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");





// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(

  options =>
  {
      options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
  }


  );
builder.Services.AddMvc(); 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IFintachartsService,FintachartsService>();
builder.Services.AddSingleton<IFintechartsWebSoketService,FintechartsWebSoketService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
    var tokenResponse = await tokenService.GetTokenResponseAsync();
    tokenService.SetAuthToken(tokenResponse);
    tokenService.SetRefreshToken(tokenResponse);
}
using(var scope = app.Services.CreateScope())
{
    var fintacharts = scope.ServiceProvider.GetRequiredService<IFintachartsService>();
    var assets =  await fintacharts.GetAssets();
     await fintacharts.RefreshAssets(assets);
}
using (var scope = app.Services.CreateScope())
{
    var soketService = scope.ServiceProvider.GetRequiredService<IFintechartsWebSoketService>();
    await soketService.StartAsync();
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
