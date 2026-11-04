using MiniDispatch.Application;
using MiniDispatch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddApplication();

var connectionString =
    builder.Configuration.GetConnectionString("DispatchDb")
    ?? throw new InvalidOperationException(
        "DispatchDb bağlantı bilgisi bulunamadı.");

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Shipments}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();