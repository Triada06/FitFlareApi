using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfilePrivacyInsertedToUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "AspNetUsers");
        }
    }
}
