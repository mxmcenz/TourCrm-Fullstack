using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_LegalEntity_With_Companies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LegalEntities");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "LegalEntities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "LegalEntities",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_CityId",
                table: "LegalEntities",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_CountryId",
                table: "LegalEntities",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalEntities_Cities_CityId",
                table: "LegalEntities",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalEntities_Countries_CountryId",
                table: "LegalEntities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalEntities_Cities_CityId",
                table: "LegalEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalEntities_Countries_CountryId",
                table: "LegalEntities");

            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_CityId",
                table: "LegalEntities");

            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_CountryId",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "LegalEntities");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
