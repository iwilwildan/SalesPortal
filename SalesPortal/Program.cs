using SalesPortal.Models;
using SalesPortal.Repositories.Contract;
using SalesPortal.Repositories.Implementation;
using SalesPortal.Utilities.Logger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//inject repository implementations

builder.Services.AddScoped<ISaleRepository<Sale>,SaleRepository>();
builder.Services.AddScoped<IGenericRepository<Country>,CountryRepository>();
builder.Services.AddScoped<IGenericRepository<Region>,RegionRepository>();
builder.Services.AddScoped<IGenericRepository<City>,CityRepository>();
builder.Services.AddScoped<IGenericRepository<Product>,ProductRepository>();
builder.Services.AddSingleton<ICustomLogger, Logger>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
