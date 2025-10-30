using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddMainImageToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "Projects",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MainImageId",
                table: "Projects",
                column: "MainImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_BlogImages_MainImageId",
                table: "Projects",
                column: "MainImageId",
                principalTable: "BlogImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_BlogImages_MainImageId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_MainImageId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Projects");
        }
    }
}
