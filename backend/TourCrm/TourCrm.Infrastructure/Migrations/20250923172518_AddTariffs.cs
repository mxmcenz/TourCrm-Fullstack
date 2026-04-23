using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTariffs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TariffId",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    MinEmployees = table.Column<int>(type: "integer", nullable: false),
                    MaxEmployees = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    HalfYearPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    YearlyPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TariffPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TariffId = table.Column<int>(type: "integer", nullable: false),
                    PermissionKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsGranted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TariffPermissions_Tariffs_TariffId",
                        column: x => x.TariffId,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TariffId",
                table: "Companies",
                column: "TariffId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffPermissions_TariffId_PermissionKey",
                table: "TariffPermissions",
                columns: new[] { "TariffId", "PermissionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_Name",
                table: "Tariffs",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Tariffs_TariffId",
                table: "Companies",
                column: "TariffId",
                principalTable: "Tariffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Tariffs_TariffId",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "TariffPermissions");

            migrationBuilder.DropTable(
                name: "Tariffs");

            migrationBuilder.DropIndex(
                name: "IX_Companies_TariffId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TariffId",
                table: "Companies");
        }
    }
}
