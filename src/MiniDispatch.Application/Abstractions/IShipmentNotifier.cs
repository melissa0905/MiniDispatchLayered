namespace MiniDispatch.Application.Abstractions;

public interface IShipmentNotifier
{
    Task ShipmentsChangedAsync(CancellationToken cancellationToken);
}
