using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eology.ToolsCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddedResultCountPropToSerpItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResultCount",
                table: "EO_SerpItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultCount",
                table: "EO_SerpItems");
        }
    }
}
