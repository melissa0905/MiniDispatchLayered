# MiniDispatch Layered

A shipment tracking application built with ASP.NET Core MVC, .NET 10, and PostgreSQL. It uses a layered architecture, MediatR commands and queries, and SignalR notifications to keep shipment lists up to date across open browser tabs.

The interface is in Turkish.

## Features

- Create shipments with a unique reference number, origin, destination, and vehicle plate.
- List the latest 100 shipments, optionally filtered by status.
- Move shipments through the `Created → InTransit → Delivered` lifecycle.
- Validate shipment details and reject invalid status transitions.
- Update other open shipment lists without reloading the page.
- Recover the live connection automatically and refresh the table after reconnecting.

## Architecture

| Project | Responsibility |
| --- | --- |
| `MiniDispatch.Domain` | Shipment entity, statuses, and lifecycle rules. |
| `MiniDispatch.Application` | MediatR commands and queries, validation, repository and notification interfaces. |
| `MiniDispatch.Infrastructure` | EF Core persistence, PostgreSQL configuration, migrations, and repository implementation. |
| `MiniDispatch.Web` | MVC controllers, Razor views, SignalR hub, notification implementation, and browser scripts. |

Application depends on Domain. Infrastructure implements Application's persistence interfaces. Web composes the application and provides the SignalR implementation of `IShipmentNotifier`, keeping SignalR out of the Application layer.

## Requirements

- .NET 10 SDK compatible with the EF Core tool version in `.config/dotnet-tools.json`.
- Docker with Docker Compose for the local PostgreSQL instance, or an existing PostgreSQL database.
- Node.js 18 or later to run the JavaScript checks.

## Run locally

Run the following commands from the repository root.

### 1. Start PostgreSQL

```sh
docker compose up -d postgres
docker compose ps
```

Wait for the database to become healthy. Compose runs PostgreSQL 17 on host port `55475` and persists its data in a named volume.

`src/MiniDispatch.Web/appsettings.Development.json` already contains a connection string matching `compose.yaml`. If you use another database, update `ConnectionStrings:DispatchDb` or override it with the `ConnectionStrings__DispatchDb` environment variable. The bundled credentials are for local development.

### 2. Restore dependencies and tools

```sh
dotnet restore MiniDispatch.sln
dotnet tool restore
```

### 3. Apply database migrations

```sh
dotnet ef database update --project src/MiniDispatch.Infrastructure --startup-project src/MiniDispatch.Web -- --environment Development
```

Migrations create the application's tables in the `dispatch` schema. They must be applied before using the shipment list; application startup does not automatically migrate the database.

### 4. Start the web application

```sh
dotnet run --project src/MiniDispatch.Web --launch-profile http
```

Open [http://localhost:5252](http://localhost:5252). The root URL displays the shipment list.

To stop the database while keeping its data:

```sh
docker compose stop postgres
```

## Live updates with SignalR

1. A command handler saves a shipment creation or status change to the database.
2. After a successful save, the handler calls `IShipmentNotifier`.
3. `SignalRShipmentNotifier` uses `IHubContext<ShipmentsHub>` to broadcast `ShipmentsChanged` to connected clients.
4. Each browser receives the notification and requests `/Shipments/Table` with its current status filter.
5. The server renders `_ShipmentsTable.cshtml`, and JavaScript replaces only the table area.

The hub is mapped to `/hubs/shipments`. Notifications indicate that data has changed; shipment data is fetched separately over HTTP. Failed database saves do not trigger notifications. Notification delivery failures are logged without reporting an already committed database change as failed.

The browser retries failed connections and table requests. After reconnecting, it reloads the table to catch up with changes made while disconnected. The SignalR browser client is bundled locally under `wwwroot/lib/signalr`.

### Try it in two tabs

1. Start the application and open the shipment list in two tabs.
2. Confirm that both tabs show **Canlı güncelleme etkin.** (Live updates enabled) below the table.
3. Click **Yeni sevkiyat** (New shipment), complete the form, and click **Kaydet** (Save). Then click **Yola çıkar** (Start transit) in the shipment list to move the shipment to `InTransit`.
4. The other tab should update without a page reload. If it is filtered to `Created`, the shipment should disappear; if filtered to `InTransit`, it should appear.
5. Click **Teslim et** (Mark as delivered) to move an in-transit shipment to `Delivered`.

Tabs opened before the SignalR scripts were added need an initial reload to load those scripts.

## Build and checks

```sh
dotnet build MiniDispatch.sln
dotnet run --project tests/MiniDispatch.NotificationChecks/MiniDispatch.NotificationChecks.csproj
node --test tests/shipments.test.cjs
```

The .NET checks verify save-before-notify ordering and the absence of notifications for invalid commands or failed saves. JavaScript checks simulate browser tabs to verify filter preservation, serialized refreshes, reconnect behavior, and request retries. These checks do not require a running database and do not replace the manual browser check above.

See [tests/README.md](tests/README.md) for more details.

## Scope

The application currently has no authentication or authorization rules for shipment operations. Live broadcasts use the connected clients of a single application instance; running multiple instances would require a shared SignalR service or backplane. Notifications are not stored in a durable outbox.

Generated `bin` and `obj` directories are excluded from version control.
