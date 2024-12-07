using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetMingler.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEventMetadataIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventMetadataEntries_EventId",
                table: "EventMetadataEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "EventMetadataEntries",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries",
                columns: new[] { "EventId", "Key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventMetadataEntries_EventId_Key",
                table: "EventMetadataEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "EventMetadataEntries",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_EventMetadataEntries_EventId",
                table: "EventMetadataEntries",
                column: "EventId");
        }
    }
}
