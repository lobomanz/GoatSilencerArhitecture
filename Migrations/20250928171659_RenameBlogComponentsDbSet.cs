using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class RenameBlogComponentsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageModel_BlogComponents_BlogComponentId",
                table: "ImageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogComponents",
                table: "BlogComponents");

            migrationBuilder.RenameTable(
                name: "BlogComponents",
                newName: "Blogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModel_Blogs_BlogComponentId",
                table: "ImageModel",
                column: "BlogComponentId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageModel_Blogs_BlogComponentId",
                table: "ImageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "BlogComponents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogComponents",
                table: "BlogComponents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModel_BlogComponents_BlogComponentId",
                table: "ImageModel",
                column: "BlogComponentId",
                principalTable: "BlogComponents",
                principalColumn: "Id");
        }
    }
}
