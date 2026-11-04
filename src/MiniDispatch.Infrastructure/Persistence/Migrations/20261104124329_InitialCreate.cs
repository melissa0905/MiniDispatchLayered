using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniDispatch.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dispatch");

            migrationBuilder.CreateTable(
                name: "Shipments",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VehiclePlate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "dispatch",
                table: "Shipments",
                columns: new[] { "Id", "CreatedAtUtc", "Destination", "Origin", "ReferenceNumber", "Status", "VehiclePlate" },
                values: new object[] { new Guid("b348a8d5-e8ea-47d7-b9a4-5a4f0308f080"), new DateTime(2026, 11, 4, 9, 0, 0, 0, DateTimeKind.Utc), "Ankara", "İstanbul", "SHP-1001", 0, "34 ABC 123" });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ReferenceNumber",
                schema: "dispatch",
                table: "Shipments",
                column: "ReferenceNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "dispatch");
        }
    }
}
