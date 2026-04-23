using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_LegalEntity_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_CompanyId_NameRu",
                table: "LegalEntities");

            migrationBuilder.AlterColumn<string>(
                name: "NameRu",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "NameFull",
                table: "LegalEntities",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DirectorPostGen",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "Директора",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DirectorPost",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "Директор",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DirectorBasis",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: "Устава",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_CompanyId_Name",
                table: "LegalEntities",
                columns: new[] { "CompanyId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Name\" IS NOT NULL AND \"Name\" <> ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_CompanyId_Name",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LegalEntities");

            migrationBuilder.AlterColumn<string>(
                name: "NameRu",
                table: "LegalEntities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameFull",
                table: "LegalEntities",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DirectorPostGen",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "Директора");

            migrationBuilder.AlterColumn<string>(
                name: "DirectorPost",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "Директор");

            migrationBuilder.AlterColumn<string>(
                name: "DirectorBasis",
                table: "LegalEntities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldDefaultValue: "Устава");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_CompanyId_NameRu",
                table: "LegalEntities",
                columns: new[] { "CompanyId", "NameRu" },
                unique: true);
        }
    }
}
