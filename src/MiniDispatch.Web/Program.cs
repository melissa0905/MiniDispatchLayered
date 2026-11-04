using MiniDispatch.Application;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Infrastructure;
using MiniDispatch.Web.Hubs;
using MiniDispatch.Web.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<IShipmentNotifier, SignalRShipmentNotifier>();

builder.Services.AddApplication();

var connectionString =
    builder.Configuration.GetConnectionString("DispatchDb")
    ?? throw new InvalidOperationException(
        "DispatchDb bağlantı bilgisi bulunamadı.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapHub<ShipmentsHub>("/hubs/shipments");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Shipments}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
