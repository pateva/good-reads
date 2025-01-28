using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodReads.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookStatusWithUserAndBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "BookStatuses");

            migrationBuilder.AddColumn<long>(
                name: "BookId",
                table: "BookStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BookStatuses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BookStatuses_BookId",
                table: "BookStatuses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookStatuses_UserId",
                table: "BookStatuses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatuses_AspNetUsers_UserId",
                table: "BookStatuses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookStatuses_Books_BookId",
                table: "BookStatuses",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookStatuses_AspNetUsers_UserId",
                table: "BookStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_BookStatuses_Books_BookId",
                table: "BookStatuses");

            migrationBuilder.DropIndex(
                name: "IX_BookStatuses_BookId",
                table: "BookStatuses");

            migrationBuilder.DropIndex(
                name: "IX_BookStatuses_UserId",
                table: "BookStatuses");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookStatuses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BookStatuses");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BookStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
