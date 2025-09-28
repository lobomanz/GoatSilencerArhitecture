using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProjectImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageModel_Blogs_BlogComponentId",
                table: "ImageModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageModel_Projects_ProjectId",
                table: "ImageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageModel",
                table: "ImageModel");

            migrationBuilder.DropIndex(
                name: "IX_ImageModel_ProjectId",
                table: "ImageModel");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ImageModel");

            migrationBuilder.RenameTable(
                name: "ImageModel",
                newName: "BlogImages");

            migrationBuilder.RenameIndex(
                name: "IX_ImageModel_BlogComponentId",
                table: "BlogImages",
                newName: "IX_BlogImages_BlogComponentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogImages",
                table: "BlogImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogImages_Blogs_BlogComponentId",
                table: "BlogImages",
                column: "BlogComponentId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogImages_Blogs_BlogComponentId",
                table: "BlogImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogImages",
                table: "BlogImages");

            migrationBuilder.RenameTable(
                name: "BlogImages",
                newName: "ImageModel");

            migrationBuilder.RenameIndex(
                name: "IX_BlogImages_BlogComponentId",
                table: "ImageModel",
                newName: "IX_ImageModel_BlogComponentId");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ImageModel",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageModel",
                table: "ImageModel",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImageModel_ProjectId",
                table: "ImageModel",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModel_Blogs_BlogComponentId",
                table: "ImageModel",
                column: "BlogComponentId",
                principalTable: "Blogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModel_Projects_ProjectId",
                table: "ImageModel",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
