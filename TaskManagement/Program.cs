using Microsoft.EntityFrameworkCore;
using SignalRYoutube.SubscribeTableDependencies;
using TaskManagement.Hubs;
using TaskManagement.MiddlewareExtensions;
using TaskManagement.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetService<IConfiguration>();
builder.Services.AddDbContext<TaskManagementContext>(item => item.UseSqlServer(config.GetConnectionString("Database")), ServiceLifetime.Singleton);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<NotificationHub>();
//builder.Services.AddSingleton<SubscribeNotificationTableDependency>();
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
app.UseSession();
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
//app.UseSqlTableDependency<SubscribeNotificationTableDependency>(config.GetConnectionString("Database"));
app.Run();
