using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompaniesAndRelationToLegalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_Name",
                table: "LegalEntities");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "LegalEntities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    LegalEntityId = table.Column<int>(type: "integer", nullable: true),
                    OwnerUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_LegalEntities_LegalEntityId",
                        column: x => x.LegalEntityId,
                        principalTable: "LegalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_CompanyId_Name",
                table: "LegalEntities",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_LegalEntityId",
                table: "Companies",
                column: "LegalEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_OwnerUserId",
                table: "Companies",
                column: "OwnerUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalEntities_Companies_CompanyId",
                table: "LegalEntities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalEntities_Companies_CompanyId",
                table: "LegalEntities");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_LegalEntities_CompanyId_Name",
                table: "LegalEntities");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LegalEntities");

            migrationBuilder.CreateIndex(
                name: "IX_LegalEntities_Name",
                table: "LegalEntities",
                column: "Name",
                unique: true);
        }
    }
}
