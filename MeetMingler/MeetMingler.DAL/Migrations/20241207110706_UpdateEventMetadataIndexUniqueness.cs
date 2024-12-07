using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetMingler.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventMetadataIndexUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries");

            migrationBuilder.CreateIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries",
                columns: new[] { "EventId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries");

            migrationBuilder.CreateIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries",
                columns: new[] { "EventId", "Key" });
        }
    }
}
