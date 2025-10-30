using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProjectModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLeftHeading",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageLeftParagraph",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageRightBottomHeading",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageRightBottomParagraph",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageRightTopHeading",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageRightTopParagraph",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MainImageBottomRight",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MainImageLeft",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MainImageTopRight",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Image1AltText",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "Image2AltText",
                table: "Blogs");

            migrationBuilder.CreateTable(
                name: "ImageWithHeadingAndParagraph",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Heading = table.Column<string>(type: "TEXT", nullable: true),
                    Paragraph = table.Column<string>(type: "TEXT", nullable: true),
                    Position = table.Column<string>(type: "TEXT", nullable: true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageWithHeadingAndParagraph", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageWithHeadingAndParagraph_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageWithHeadingAndParagraph_ProjectId",
                table: "ImageWithHeadingAndParagraph",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageWithHeadingAndParagraph");

            migrationBuilder.AddColumn<string>(
                name: "ImageLeftHeading",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageLeftParagraph",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRightBottomHeading",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRightBottomParagraph",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRightTopHeading",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRightTopParagraph",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImageBottomRight",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImageLeft",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImageTopRight",
                table: "Projects",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image1AltText",
                table: "Blogs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2AltText",
                table: "Blogs",
                type: "TEXT",
                nullable: true);
        }
    }
}
