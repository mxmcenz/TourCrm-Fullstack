using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyId1",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CompanyId1",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "Roles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CompanyId1",
                table: "Roles",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyId1",
                table: "Roles",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
