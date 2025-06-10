using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StoryTbaleFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewerIds",
                table: "Stories");

            migrationBuilder.CreateTable(
                name: "StoryViews",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    StoryId = table.Column<string>(type: "text", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryViews", x => new { x.UserId, x.StoryId });
                    table.ForeignKey(
                        name: "FK_StoryViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryViews_Stories_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Stories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryViews_StoryId",
                table: "StoryViews",
                column: "StoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryViews");

            migrationBuilder.AddColumn<string[]>(
                name: "ViewerIds",
                table: "Stories",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }
    }
}
