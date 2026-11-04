# SignalR checks

Run from the repository root:

```powershell
dotnet run --project tests/MiniDispatch.NotificationChecks/MiniDispatch.NotificationChecks.csproj
node --test tests/shipments.test.cjs
```

The .NET checks verify that successful commands save before notifying, and that
invalid commands and failed database commits do not send notifications.
The JavaScript checks use isolated simulated tabs to verify filter preservation,
queued refreshes, reconnect refreshes, and retry after a failed table request.

For an end-to-end check with the configured database, open `/Shipments` in one
browser tab and `/Shipments?status=Created` in another. Move a created shipment
to InTransit in the first tab. The other tab should remove that shipment without
a page reload. A tab filtered to InTransit should show it. Create a shipment and
confirm that the all-shipments and Created lists also update. Disconnect and
reconnect the network and confirm that the table catches up after reconnecting.
