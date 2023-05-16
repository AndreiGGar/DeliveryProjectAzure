using Azure.Security.KeyVault.Secrets;
using DeliveryProjectAzure.Services;
using DeliveryProjectNuget.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync("DeliveryApi");
KeyVaultSecret keyVaultSecretCacheRedis = await secretClient.GetSecretAsync("DeliveryProjectCacheRedis");

string azureKeys = keyVaultSecret.Value;
string azureKeyCacheRedis = keyVaultSecretCacheRedis.Value;
/*string redisCacheConnectionString = builder.Configuration.GetSection("RedisCache:ConnectionString").Value;*/
builder.Services.AddResponseCaching();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = azureKeyCacheRedis;
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(azureKeyCacheRedis));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("USER",
        policy => policy.RequireRole("user"));
    options.AddPolicy("ADMIN",
        policy => policy.RequireRole("admin"));
});

builder.Services.AddTransient<HelperCallApi>();
builder.Services.AddTransient<ServiceApiDelivery>();
builder.Services.AddTransient<ServiceCart>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
{
    config.AccessDeniedPath = "/Managed/Error";
});

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "Default",
        template: "{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();