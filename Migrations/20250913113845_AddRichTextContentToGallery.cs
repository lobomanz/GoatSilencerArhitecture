using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddRichTextContentToGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RichTextContent",
                table: "Galleries",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RichTextContent",
                table: "Galleries");
        }
    }
}
