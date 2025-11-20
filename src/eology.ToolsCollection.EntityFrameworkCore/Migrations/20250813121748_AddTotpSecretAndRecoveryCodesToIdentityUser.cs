using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eology.ToolsCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddTotpSecretAndRecoveryCodesToIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecoveryCodes",
                table: "AbpUsers",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TotpSecret",
                table: "AbpUsers",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryCodes",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "TotpSecret",
                table: "AbpUsers");
        }
    }
}
