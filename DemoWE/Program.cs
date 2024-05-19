using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DemoWE.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DemoWEContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DemoWEContext") ?? throw new InvalidOperationException("Connection string 'DemoWEContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=login}/{id?}");

app.Run();