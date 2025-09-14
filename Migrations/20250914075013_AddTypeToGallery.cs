using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Galleries",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Galleries");
        }
    }
}
