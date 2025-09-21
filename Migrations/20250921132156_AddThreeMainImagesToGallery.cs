using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddThreeMainImagesToGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainImageAltText",
                table: "Galleries",
                newName: "MainImageTopRight");

            migrationBuilder.RenameColumn(
                name: "MainImage",
                table: "Galleries",
                newName: "MainImageLeft");

            migrationBuilder.AddColumn<bool>(
                name: "IsMainImage",
                table: "GalleryImages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MainImageBottomRight",
                table: "Galleries",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMainImage",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "MainImageBottomRight",
                table: "Galleries");

            migrationBuilder.RenameColumn(
                name: "MainImageTopRight",
                table: "Galleries",
                newName: "MainImageAltText");

            migrationBuilder.RenameColumn(
                name: "MainImageLeft",
                table: "Galleries",
                newName: "MainImage");
        }
    }
}
