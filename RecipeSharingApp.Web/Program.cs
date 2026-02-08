using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using RecipeSharingApp.Domain;
using RecipeSharingApp.Domain.Identity;
using RecipeSharingApp.Repository;
using RecipeSharingApp.Repository.Impl;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Impl;
using RecipeSharingApp.Service.Implementation;
using RecipeSharingApp.Service.Interface;
using SQLitePCL;
using Stripe;

Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<RecipeAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
     .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<IRecipeService, RecipeService>();
builder.Services.AddTransient<IRecipeRatingService, RecipeRatingService>();
builder.Services.AddTransient<IRecipeCollectionService, RecipeCollectionService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();