using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRepliesCountToCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepliesCount",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepliesCount",
                table: "Comments");
        }
    }
}
