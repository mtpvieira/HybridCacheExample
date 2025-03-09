using HybridCacheExample;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();

builder.Services.AddSingleton<AddressService>();

builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(options => options.Duration = TimeSpan.FromMinutes(10))
    .WithSerializer(new FusionCacheSystemTextJsonSerializer())
    .WithDistributedCache(new RedisCache(new RedisCacheOptions { Configuration = "localhost:7070" }))
    .AsHybridCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Map("Address/Cep/{cep}",
    async (string cep, AddressService addressService) =>
    {
        var address = await addressService.GetCurrentCEP(cep);

        return address is null ? Results.NotFound() : Results.Ok(address);
    });

app.Map("AddressCached/Cep/{cep}",
    async (string cep, AddressService addressService) =>
    {
        var address = await addressService.GetCurrentCEPCached(cep);

        return address is null ? Results.NotFound() : Results.Ok(address);
    });

app.Run();
