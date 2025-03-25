using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopily.Migrations
{
    /// <inheritdoc />
    public partial class ekek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsSites");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "NewsArticles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePublished",
                table: "NewsArticles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceWebsite",
                table: "NewsArticles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NewsArticles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "DatePublished",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "SourceWebsite",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NewsArticles");

            migrationBuilder.CreateTable(
                name: "NewsSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsSites", x => x.Id);
                });
        }
    }
}
