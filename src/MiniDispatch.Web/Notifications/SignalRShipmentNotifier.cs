using Microsoft.AspNetCore.SignalR;
using MiniDispatch.Application.Abstractions;
using MiniDispatch.Web.Hubs;

namespace MiniDispatch.Web.Notifications;

public sealed class SignalRShipmentNotifier(
    IHubContext<ShipmentsHub> hubContext,
    ILogger<SignalRShipmentNotifier> logger) : IShipmentNotifier
{
    public async Task ShipmentsChangedAsync(CancellationToken cancellationToken)
    {
        try
        {
            await hubContext.Clients.All.SendAsync("ShipmentsChanged", cancellationToken);
        }
        catch (Exception exception)
        {
            // The database commit has already succeeded; a notification failure
            // must not make the user retry the saved command.
            logger.LogWarning(exception, "Sevkiyat değişikliği istemcilere bildirilemedi.");
        }
    }
}
