using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLMSMVC.Migrations
{
    /// <inheritdoc />
    public partial class SR_UsedAPI_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedAPI",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedAPI",
                table: "ServiceRequests");
        }
    }
}
