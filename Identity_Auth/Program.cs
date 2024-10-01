using Identity_Auth.Data;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("default");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
    }
).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add UserDbContextFactory


// Add Session
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".YourApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout süresi
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Varsayýlan süre 30 dakika
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "YourAppCookie";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Authentication middleware'ini ekleyin
app.UseAuthorization();

// Use Session
app.UseSession();
app.MapWhen(context => context.User.Identity.IsAuthenticated, appBuilder =>
{
    appBuilder.UseRouting();
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "authenticated",
            pattern: "{controller=Home}/{action=Help}/{id?}");
    });
});

app.MapWhen(context => !context.User.Identity.IsAuthenticated, appBuilder =>
{
    appBuilder.UseRouting();
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
});


app.Run();
