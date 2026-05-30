using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevAssistAI.Migrations
{
    /// <inheritdoc />
    public partial class AddHashAndIsActiveToDocumentChunk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "DocumentChunk",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DocumentChunk",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "DocumentChunk");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DocumentChunk");
        }
    }
}
