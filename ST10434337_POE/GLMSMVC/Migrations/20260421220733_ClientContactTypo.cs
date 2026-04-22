using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLMSMVC.Migrations
{
    /// <inheritdoc />
    public partial class ClientContactTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractDetails",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "ContactDetails",
                table: "Clients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactDetails",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "ContractDetails",
                table: "Clients",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
