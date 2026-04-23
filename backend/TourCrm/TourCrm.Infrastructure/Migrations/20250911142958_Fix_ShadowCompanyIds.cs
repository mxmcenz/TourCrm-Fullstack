using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_ShadowCompanyIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Companies_CompanyId1",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Companies_CompanyId1",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Companies_CompanyId1",
                table: "Labels");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadHistories_Companies_CompanyId",
                table: "LeadHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadLabels_Companies_CompanyId",
                table: "LeadLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadRequestTypes_Companies_CompanyId1",
                table: "LeadRequestTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Companies_CompanyId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadSelections_Companies_CompanyId",
                table: "LeadSelections");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadSources_Companies_CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_CompanyId1",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CompanyId1",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CompanyId1",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CompanyId1",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadRequestTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Currencies");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadHistories_Companies_CompanyId",
                table: "LeadHistories",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadLabels_Companies_CompanyId",
                table: "LeadLabels",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Companies_CompanyId",
                table: "Leads",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadSelections_Companies_CompanyId",
                table: "LeadSelections",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadHistories_Companies_CompanyId",
                table: "LeadHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadLabels_Companies_CompanyId",
                table: "LeadLabels");

            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Companies_CompanyId",
                table: "Leads");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadSelections_Companies_CompanyId",
                table: "LeadSelections");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "LeadStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "LeadSources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "LeadRequestTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "Labels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "Hotels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "Currencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_CompanyId1",
                table: "LeadStatuses",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_CompanyId1",
                table: "LeadSources",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_CompanyId1",
                table: "LeadRequestTypes",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CompanyId1",
                table: "Labels",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CompanyId1",
                table: "Hotels",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId1",
                table: "Currencies",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Companies_CompanyId1",
                table: "Currencies",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Companies_CompanyId1",
                table: "Hotels",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Companies_CompanyId1",
                table: "Labels",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadHistories_Companies_CompanyId",
                table: "LeadHistories",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadLabels_Companies_CompanyId",
                table: "LeadLabels",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadRequestTypes_Companies_CompanyId1",
                table: "LeadRequestTypes",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Companies_CompanyId",
                table: "Leads",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadSelections_Companies_CompanyId",
                table: "LeadSelections",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadSources_Companies_CompanyId1",
                table: "LeadSources",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId1",
                table: "LeadStatuses",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
