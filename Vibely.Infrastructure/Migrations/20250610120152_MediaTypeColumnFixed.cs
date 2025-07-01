using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MediaTypeColumnFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediType",
                table: "Stories",
                newName: "MediaType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MediaType",
                table: "Stories",
                newName: "MediType");
        }
    }
}
