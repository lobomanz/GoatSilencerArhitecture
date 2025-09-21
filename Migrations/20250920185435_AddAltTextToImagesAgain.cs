using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddAltTextToImagesAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "GalleryImages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainImageAltText",
                table: "Galleries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image1AltText",
                table: "AboutComponents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2AltText",
                table: "AboutComponents",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltText",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "MainImageAltText",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "Image1AltText",
                table: "AboutComponents");

            migrationBuilder.DropColumn(
                name: "Image2AltText",
                table: "AboutComponents");
        }
    }
}
