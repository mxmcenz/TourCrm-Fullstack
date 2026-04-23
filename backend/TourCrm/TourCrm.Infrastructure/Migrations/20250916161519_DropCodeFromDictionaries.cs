using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropCodeFromDictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_CompanyId_Code",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_CompanyId_Code",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_CompanyId_Code",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CompanyId_Code",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CompanyId_Code",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "LeadStatuses");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "LeadSources");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "LeadRequestTypes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Currencies");

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_CompanyId_Name",
                table: "LeadStatuses",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_CompanyId_Name",
                table: "LeadSources",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_CompanyId_Name",
                table: "LeadRequestTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId_Name",
                table: "Currencies",
                columns: new[] { "CompanyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_CompanyId_Name",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_CompanyId_Name",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_CompanyId_Name",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CompanyId_Name",
                table: "Currencies");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "LeadStatuses",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "LeadSources",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "LeadRequestTypes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Labels",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_CompanyId_Code",
                table: "LeadStatuses",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_CompanyId_Code",
                table: "LeadSources",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_CompanyId_Code",
                table: "LeadRequestTypes",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CompanyId_Code",
                table: "Labels",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId_Code",
                table: "Currencies",
                columns: new[] { "CompanyId", "Code" },
                unique: true);
        }
    }
}
