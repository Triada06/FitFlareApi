using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitFlare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GeneralNotificationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TriggeredById",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PostId",
                table: "Notifications",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TriggeredById",
                table: "Notifications",
                column: "TriggeredById");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_TriggeredById",
                table: "Notifications",
                column: "TriggeredById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Posts_PostId",
                table: "Notifications",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_TriggeredById",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Posts_PostId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PostId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TriggeredById",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TriggeredById",
                table: "Notifications");
        }
    }
}
