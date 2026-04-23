using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Lead_Function : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Users_AssignedToUserId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "TourName",
                table: "Leads");

            migrationBuilder.RenameColumn(
                name: "TouristId",
                table: "Leads",
                newName: "SourceId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Leads",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "IsClosed",
                table: "Leads",
                newName: "PrecontractRequired");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "Leads",
                newName: "RequestTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Leads_AssignedToUserId",
                table: "Leads",
                newName: "IX_Leads_RequestTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Accommodation",
                table: "Leads",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Adults",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Leads",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Leads",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerFirstName",
                table: "Leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerLastName",
                table: "Leads",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                table: "Leads",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "person");

            migrationBuilder.AddColumn<DateTime>(
                name: "DesiredArrival",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DesiredDeparture",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DocsPackageDate",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Infants",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InvoiceRequired",
                table: "Leads",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Leads",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LeadNumber",
                table: "Leads",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LeadStatusId",
                table: "Leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ManagerFullName",
                table: "Leads",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MealPlan",
                table: "Leads",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NightsFrom",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NightsTo",
                table: "Leads",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Leads",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Labels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeadId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ActorUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ActorFullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SnapshotJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadHistory_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeadRequestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRequestTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadLabels",
                columns: table => new
                {
                    LeadId = table.Column<int>(type: "integer", nullable: false),
                    LabelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadLabels", x => new { x.LeadId, x.LabelId });
                    table.ForeignKey(
                        name: "FK_LeadLabels_Labels_LabelId",
                        column: x => x.LabelId,
                        principalTable: "Labels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadLabels_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_LeadNumber",
                table: "Leads",
                column: "LeadNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leads_LeadStatusId",
                table: "Leads",
                column: "LeadStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_SourceId",
                table: "Leads",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_Code",
                table: "Labels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistory_LeadId",
                table: "LeadHistory",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLabels_LabelId",
                table: "LeadLabels",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_Code",
                table: "LeadRequestTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_Code",
                table: "LeadSources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_Code",
                table: "LeadStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_LeadRequestTypes_RequestTypeId",
                table: "Leads",
                column: "RequestTypeId",
                principalTable: "LeadRequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_LeadSources_SourceId",
                table: "Leads",
                column: "SourceId",
                principalTable: "LeadSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_LeadStatuses_LeadStatusId",
                table: "Leads",
                column: "LeadStatusId",
                principalTable: "LeadStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_LeadRequestTypes_RequestTypeId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_LeadSources_SourceId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_LeadStatuses_LeadStatusId",
                table: "Leads");

            migrationBuilder.DropTable(
                name: "LeadHistory");

            migrationBuilder.DropTable(
                name: "LeadLabels");

            migrationBuilder.DropTable(
                name: "LeadRequestTypes");

            migrationBuilder.DropTable(
                name: "LeadSources");

            migrationBuilder.DropTable(
                name: "LeadStatuses");

            migrationBuilder.DropTable(
                name: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Leads_LeadNumber",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_LeadStatusId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Leads_SourceId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Accommodation",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Adults",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CustomerFirstName",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CustomerLastName",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "DesiredArrival",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "DesiredDeparture",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "DocsPackageDate",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Infants",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "InvoiceRequired",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "LeadNumber",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "LeadStatusId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ManagerFullName",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "MealPlan",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "NightsFrom",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "NightsTo",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Leads");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "Leads",
                newName: "TouristId");

            migrationBuilder.RenameColumn(
                name: "RequestTypeId",
                table: "Leads",
                newName: "AssignedToUserId");

            migrationBuilder.RenameColumn(
                name: "PrecontractRequired",
                table: "Leads",
                newName: "IsClosed");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Leads",
                newName: "StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_Leads_RequestTypeId",
                table: "Leads",
                newName: "IX_Leads_AssignedToUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Leads",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Leads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TourName",
                table: "Leads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Users_AssignedToUserId",
                table: "Leads",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
