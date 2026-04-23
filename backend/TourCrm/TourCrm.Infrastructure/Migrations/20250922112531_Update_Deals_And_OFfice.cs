using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Deals_And_OFfice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Offices_OfficeId1",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Tourists_TouristId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_ServiceTypes_CompanyId"";");

            migrationBuilder.DropIndex(
                name: "IX_Deals_OfficeId1",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_TouristId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OfficeId1",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "TouristId",
                table: "Deals");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "DealClientPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealClientPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealClientPayments_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealCustomers",
                columns: table => new
                {
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealCustomers", x => new { x.DealId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_DealCustomers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealCustomers_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealPartnerPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealPartnerPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealPartnerPayments_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealServices_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealTourists",
                columns: table => new
                {
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealTourists", x => new { x.DealId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_DealTourists_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealTourists_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealClientPayments_DealId",
                table: "DealClientPayments",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealCustomers_ClientId",
                table: "DealCustomers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DealPartnerPayments_DealId",
                table: "DealPartnerPayments",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealServices_DealId",
                table: "DealServices",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealTourists_ClientId",
                table: "DealTourists",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "DealClientPayments");

            migrationBuilder.DropTable(
                name: "DealCustomers");

            migrationBuilder.DropTable(
                name: "DealPartnerPayments");

            migrationBuilder.DropTable(
                name: "DealServices");

            migrationBuilder.DropTable(
                name: "DealTourists");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<int>(
                name: "OfficeId1",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouristId",
                table: "Deals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_CompanyId",
                table: "ServiceTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_OfficeId1",
                table: "Deals",
                column: "OfficeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_TouristId",
                table: "Deals",
                column: "TouristId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Offices_OfficeId1",
                table: "Deals",
                column: "OfficeId1",
                principalTable: "Offices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Tourists_TouristId",
                table: "Deals",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
