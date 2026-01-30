using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueTelemetryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelemetryRecords_DeviceId_Timestamp",
                table: "TelemetryRecords");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryRecords_DeviceId_Timestamp",
                table: "TelemetryRecords",
                columns: new[] { "DeviceId", "Timestamp" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TelemetryRecords_DeviceId_Timestamp",
                table: "TelemetryRecords");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryRecords_DeviceId_Timestamp",
                table: "TelemetryRecords",
                columns: new[] { "DeviceId", "Timestamp" });
        }
    }
}
