using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Expand_LegalEntity_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "LegalEntities");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "LegalEntities",
                newName: "NameRu");

            migrationBuilder.RenameIndex(
                name: "IX_LegalEntities_CompanyId_Name",
                table: "LegalEntities",
                newName: "IX_LegalEntities_CompanyId_NameRu");

            migrationBuilder.AddColumn<string>(
                name: "ActualAddress",
                table: "LegalEntities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CfoFio",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "DirectorBasis",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DirectorFio",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DirectorFioGen",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DirectorPost",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DirectorPostGen",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LegalAddress",
                table: "LegalEntities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameFull",
                table: "LegalEntities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phones",
                table: "LegalEntities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "LegalEntities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualAddress",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "CfoFio",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "City",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "DirectorBasis",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "DirectorFio",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "DirectorFioGen",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "DirectorPost",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "DirectorPostGen",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "LegalAddress",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "NameFull",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "Phones",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "LegalEntities");

            migrationBuilder.RenameColumn(
                name: "NameRu",
                table: "LegalEntities",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_LegalEntities_CompanyId_NameRu",
                table: "LegalEntities",
                newName: "IX_LegalEntities_CompanyId_Name");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "LegalEntities",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }
    }
}
