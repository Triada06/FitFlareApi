using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostTableChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Posts",
                newName: "Media");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Media",
                table: "Posts",
                newName: "Image");
        }
    }
}
