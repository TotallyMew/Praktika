using Microsoft.EntityFrameworkCore;
using Praktia_WEB_API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ShirtStoreManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MVC services to support views
builder.Services.AddControllersWithViews();

var app = builder.Build();

    app.UseDeveloperExceptionPage();



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=NotificationSmsBillingInformation}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
