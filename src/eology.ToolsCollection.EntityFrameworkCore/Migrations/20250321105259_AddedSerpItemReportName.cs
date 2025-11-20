using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eology.ToolsCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddedSerpItemReportName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportName",
                table: "EO_SerpItems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportName",
                table: "EO_SerpItems");
        }
    }
}
