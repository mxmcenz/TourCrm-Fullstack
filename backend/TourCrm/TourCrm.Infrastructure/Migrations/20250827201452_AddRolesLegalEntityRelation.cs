using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesLegalEntityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.AddColumn<int>(
                name: "LegalEntityId",
                table: "Roles",
                type: "integer",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_LegalEntities_LegalEntityId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_LegalEntityId_Name",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "LegalEntityId",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name");
        }
    }
}
