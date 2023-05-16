using Azure.Security.KeyVault.Secrets;
using DeliveryProjectAzure.Services;
using DeliveryProjectNuget.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
/*string connectionString = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");*/
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync("DeliveryApi");

string azureKeys = keyVaultSecret.Value;

// Add services to the container.
builder.Services.AddResponseCaching();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("USER",
        policy =>
        policy.RequireRole("user"));
    options.AddPolicy("ADMIN",
        policy =>
        policy.RequireRole("admin"));
});

builder.Services.AddTransient<HelperCallApi>();
builder.Services.AddTransient<ServiceApiDelivery>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme,
    config =>
    {
        config.AccessDeniedPath = "/Managed/Error";
    });

builder.Services.AddControllersWithViews(
    options => options.EnableEndpointRouting = false)
    .AddSessionStateTempDataProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.Run();
