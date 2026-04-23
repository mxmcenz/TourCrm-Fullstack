using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Company_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_MealTypes_Companies_CompanyId",
                table: "MealTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberTypes_Companies_CompanyId",
                table: "NumberTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Companies_CompanyId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_CompanyId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_NumberTypes_CompanyId",
                table: "NumberTypes");

            migrationBuilder.DropIndex(
                name: "IX_NumberTypes_Name",
                table: "NumberTypes");

            migrationBuilder.DropIndex(
                name: "IX_MealTypes_CompanyId",
                table: "MealTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_Code",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_Code",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_Code",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_Labels_Code",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CityId_Name",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Code",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CompanyId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId_Name",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_AccommodationTypes_CompanyId",
                table: "AccommodationTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Partners",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "LeadStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId1",
                table: "LeadStatuses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "LeadSources",
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
                name: "CompanyId",
                table: "LeadSelections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "LeadRequestTypes",
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
                name: "CompanyId",
                table: "LeadLabels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "LeadHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Labels",
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
                name: "CompanyId",
                table: "Hotels",
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
                name: "CompanyId",
                table: "Currencies",
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
                name: "IX_Partners_CompanyId_Name",
                table: "Partners",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NumberTypes_CompanyId_Name",
                table: "NumberTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealTypes_CompanyId_Name",
                table: "MealTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_CompanyId_Code",
                table: "LeadStatuses",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_CompanyId1",
                table: "LeadStatuses",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_CompanyId_Code",
                table: "LeadSources",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_CompanyId1",
                table: "LeadSources",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadSelections_CompanyId",
                table: "LeadSelections",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_CompanyId",
                table: "Leads",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_CompanyId_Code",
                table: "LeadRequestTypes",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_CompanyId1",
                table: "LeadRequestTypes",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_LeadLabels_CompanyId",
                table: "LeadLabels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadHistories_CompanyId",
                table: "LeadHistories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CompanyId_Code",
                table: "Labels",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CompanyId_Name",
                table: "Labels",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_CompanyId1",
                table: "Labels",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CompanyId_CityId_Name",
                table: "Hotels",
                columns: new[] { "CompanyId", "CityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CompanyId1",
                table: "Hotels",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId_Code",
                table: "Currencies",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CompanyId1",
                table: "Currencies",
                column: "CompanyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CompanyId_CountryId_Name",
                table: "Cities",
                columns: new[] { "CompanyId", "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationTypes_CompanyId_Name",
                table: "AccommodationTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Companies_CompanyId",
                table: "Currencies",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Companies_CompanyId1",
                table: "Currencies",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Companies_CompanyId",
                table: "Hotels",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Companies_CompanyId1",
                table: "Hotels",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Companies_CompanyId",
                table: "Labels",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_LeadRequestTypes_Companies_CompanyId",
                table: "LeadRequestTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_LeadSources_Companies_CompanyId",
                table: "LeadSources",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadSources_Companies_CompanyId1",
                table: "LeadSources",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId",
                table: "LeadStatuses",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId1",
                table: "LeadStatuses",
                column: "CompanyId1",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealTypes_Companies_CompanyId",
                table: "MealTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NumberTypes_Companies_CompanyId",
                table: "NumberTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Companies_CompanyId",
                table: "Partners",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Companies_CompanyId",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Companies_CompanyId1",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Companies_CompanyId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Companies_CompanyId1",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Companies_CompanyId",
                table: "Labels");

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
                name: "FK_LeadRequestTypes_Companies_CompanyId",
                table: "LeadRequestTypes");

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
                name: "FK_LeadSources_Companies_CompanyId",
                table: "LeadSources");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadSources_Companies_CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId",
                table: "LeadStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_LeadStatuses_Companies_CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_MealTypes_Companies_CompanyId",
                table: "MealTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_NumberTypes_Companies_CompanyId",
                table: "NumberTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Companies_CompanyId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_CompanyId_Name",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_NumberTypes_CompanyId_Name",
                table: "NumberTypes");

            migrationBuilder.DropIndex(
                name: "IX_MealTypes_CompanyId_Name",
                table: "MealTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_CompanyId_Code",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadStatuses_CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_CompanyId_Code",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadSources_CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropIndex(
                name: "IX_LeadSelections_CompanyId",
                table: "LeadSelections");

            migrationBuilder.DropIndex(
                name: "IX_Leads_CompanyId",
                table: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_CompanyId_Code",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeadRequestTypes_CompanyId1",
                table: "LeadRequestTypes");

            migrationBuilder.DropIndex(
                name: "IX_LeadLabels_CompanyId",
                table: "LeadLabels");

            migrationBuilder.DropIndex(
                name: "IX_LeadHistories_CompanyId",
                table: "LeadHistories");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CompanyId_Code",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CompanyId_Name",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_CompanyId1",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CompanyId_CityId_Name",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_CompanyId1",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CompanyId_Code",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CompanyId1",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CompanyId_CountryId_Name",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_AccommodationTypes_CompanyId_Name",
                table: "AccommodationTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadStatuses");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadStatuses");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadSources");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadSources");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadSelections");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadRequestTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "LeadRequestTypes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadLabels");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "LeadHistories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "Currencies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Partners",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CompanyId",
                table: "Partners",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberTypes_CompanyId",
                table: "NumberTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberTypes_Name",
                table: "NumberTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealTypes_CompanyId",
                table: "MealTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadStatuses_Code",
                table: "LeadStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadSources_Code",
                table: "LeadSources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeadRequestTypes_Code",
                table: "LeadRequestTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_Code",
                table: "Labels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId_Name",
                table: "Hotels",
                columns: new[] { "CityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CompanyId",
                table: "Cities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId_Name",
                table: "Cities",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationTypes_CompanyId",
                table: "AccommodationTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealTypes_Companies_CompanyId",
                table: "MealTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NumberTypes_Companies_CompanyId",
                table: "NumberTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Companies_CompanyId",
                table: "Partners",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
