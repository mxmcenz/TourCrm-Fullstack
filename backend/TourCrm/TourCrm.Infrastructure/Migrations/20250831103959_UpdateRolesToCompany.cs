using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolesToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_LegalEntities_LegalEntityId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_LegalEntityId_Name",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "LegalEntityId",
                table: "Roles",
                newName: "CompanyId1");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Roles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CompanyId",
                table: "Roles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CompanyId1",
                table: "Roles",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyId",
                table: "Roles",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyId1",
                table: "Roles",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyId1",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CompanyId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_CompanyId1",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Roles");

            migrationBuilder.RenameColumn(
                name: "CompanyId1",
                table: "Roles",
                newName: "LegalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_LegalEntityId_Name",
                table: "Roles",
                columns: new[] { "LegalEntityId", "Name" });

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_LegalEntities_LegalEntityId",
                table: "Roles",
                column: "LegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
