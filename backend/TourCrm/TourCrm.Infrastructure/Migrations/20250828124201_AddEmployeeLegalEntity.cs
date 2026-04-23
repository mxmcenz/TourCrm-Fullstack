using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeLegalEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegalEntityId",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_LegalEntityId",
                table: "Employees",
                column: "LegalEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_LegalEntities_LegalEntityId",
                table: "Employees",
                column: "LegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_LegalEntities_LegalEntityId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_LegalEntityId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LegalEntityId",
                table: "Employees");
        }
    }
}
