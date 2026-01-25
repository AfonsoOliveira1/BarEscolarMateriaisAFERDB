using System;
using APiConsumer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keeps property names as-is (no camelCase conversion)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;

        // Optional: handle DateOnly correctly
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("APIBarEscola", client =>
{
    client.BaseAddress = new Uri("https://localhost:7205/");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<RolesApiClient>();
builder.Services.AddScoped<MenuWeeksApiClient>();
builder.Services.AddScoped<MenuDaysApiClient>();
builder.Services.AddScoped<ProductsApiClient>();
builder.Services.AddScoped<CategoryApiClient>();
builder.Services.AddScoped<UsersApiClient>();
builder.Services.AddScoped<OrdersApiClient>();
builder.Services.AddScoped<OrderItemsApiClient>();
builder.Services.AddScoped<AuthenticationService>();


builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
