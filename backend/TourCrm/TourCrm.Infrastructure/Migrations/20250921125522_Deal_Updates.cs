using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Deal_Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Users_ManagerId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Users_TouristId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CancelPenalty",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Deals");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Deals",
                newName: "StatusId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "AddStampAndSign",
                table: "Deals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BookingNumbers",
                table: "Deals",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClientPaymentDeadline",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Deals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Deals",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DealNumber",
                table: "Deals",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DocsPackageDate",
                table: "Deals",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeCostCalc",
                table: "Deals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeTourCalc",
                table: "Deals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InternalNumber",
                table: "Deals",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Deals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IssuerLegalEntityId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Deals",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfficeId1",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PartnerPaymentDeadline",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestTypeId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TourOperatorId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisaTypeId",
                table: "Deals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DealHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DealId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ActorUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ActorFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealHistories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DealHistories_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealStatuses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_CompanyId_DealNumber",
                table: "Deals",
                columns: new[] { "CompanyId", "DealNumber" },
                unique: true,
                filter: "\"IsDeleted\" = false AND \"DealNumber\" IS NOT NULL AND \"DealNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_IssuerLegalEntityId",
                table: "Deals",
                column: "IssuerLegalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_LeadId",
                table: "Deals",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_OfficeId1",
                table: "Deals",
                column: "OfficeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_RequestTypeId",
                table: "Deals",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_SourceId",
                table: "Deals",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_StatusId",
                table: "Deals",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_TourOperatorId",
                table: "Deals",
                column: "TourOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DealHistories_CompanyId",
                table: "DealHistories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DealHistories_DealId",
                table: "DealHistories",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealStatuses_CompanyId_Name",
                table: "DealStatuses",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Companies_CompanyId",
                table: "Deals",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_DealStatuses_StatusId",
                table: "Deals",
                column: "StatusId",
                principalTable: "DealStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Employees_ManagerId",
                table: "Deals",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_LeadRequestTypes_RequestTypeId",
                table: "Deals",
                column: "RequestTypeId",
                principalTable: "LeadRequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_LeadSources_SourceId",
                table: "Deals",
                column: "SourceId",
                principalTable: "LeadSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Leads_LeadId",
                table: "Deals",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_LegalEntities_IssuerLegalEntityId",
                table: "Deals",
                column: "IssuerLegalEntityId",
                principalTable: "LegalEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Offices_OfficeId1",
                table: "Deals",
                column: "OfficeId1",
                principalTable: "Offices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_TourOperators_TourOperatorId",
                table: "Deals",
                column: "TourOperatorId",
                principalTable: "TourOperators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Tourists_TouristId",
                table: "Deals",
                column: "TouristId",
                principalTable: "Tourists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Companies_CompanyId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_DealStatuses_StatusId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Employees_ManagerId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_LeadRequestTypes_RequestTypeId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_LeadSources_SourceId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Leads_LeadId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_LegalEntities_IssuerLegalEntityId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Offices_OfficeId1",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_TourOperators_TourOperatorId",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_Tourists_TouristId",
                table: "Deals");

            migrationBuilder.DropTable(
                name: "DealHistories");

            migrationBuilder.DropTable(
                name: "DealStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Deals_CompanyId_DealNumber",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_IssuerLegalEntityId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_LeadId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_OfficeId1",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_RequestTypeId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_SourceId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_StatusId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_TourOperatorId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "AddStampAndSign",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "BookingNumbers",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ClientPaymentDeadline",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "DealNumber",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "DocsPackageDate",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "IncludeCostCalc",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "IncludeTourCalc",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "InternalNumber",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "IssuerLegalEntityId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OfficeId1",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "PartnerPaymentDeadline",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "RequestTypeId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "TourOperatorId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "VisaTypeId",
                table: "Deals");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Deals",
                newName: "Status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CancelPenalty",
                table: "Deals",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Deals",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Users_ManagerId",
                table: "Deals",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_Users_TouristId",
                table: "Deals",
                column: "TouristId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
