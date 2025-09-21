using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoatSilencerArchitecture.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutComponentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutComponents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LayoutType = table.Column<string>(type: "TEXT", nullable: false),
                    TextContent = table.Column<string>(type: "TEXT", nullable: false),
                    Image1Path = table.Column<string>(type: "TEXT", nullable: true),
                    Image2Path = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutComponents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutComponents");
        }
    }
}
