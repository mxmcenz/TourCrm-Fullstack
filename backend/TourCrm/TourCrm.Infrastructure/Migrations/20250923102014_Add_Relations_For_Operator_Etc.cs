using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Relations_For_Operator_Etc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citizenships_Companies_CompanyId",
                table: "Citizenships");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerMarks_Companies_CompanyId",
                table: "PartnerMarks");

            migrationBuilder.DropForeignKey(
                name: "FK_TourOperators_Companies_CompanyId",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_TourOperators_CompanyId",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_PartnerMarks_CompanyId",
                table: "PartnerMarks");

            migrationBuilder.DropIndex(
                name: "IX_Citizenships_CompanyId",
                table: "Citizenships");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TourOperators",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartnerMarks",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Citizenships",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_TourOperators_CompanyId_Name",
                table: "TourOperators",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerMarks_CompanyId_Name",
                table: "PartnerMarks",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citizenships_CompanyId_Name",
                table: "Citizenships",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Citizenships_Companies_CompanyId",
                table: "Citizenships",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerMarks_Companies_CompanyId",
                table: "PartnerMarks",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperators_Companies_CompanyId",
                table: "TourOperators",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citizenships_Companies_CompanyId",
                table: "Citizenships");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerMarks_Companies_CompanyId",
                table: "PartnerMarks");

            migrationBuilder.DropForeignKey(
                name: "FK_TourOperators_Companies_CompanyId",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_TourOperators_CompanyId_Name",
                table: "TourOperators");

            migrationBuilder.DropIndex(
                name: "IX_PartnerMarks_CompanyId_Name",
                table: "PartnerMarks");

            migrationBuilder.DropIndex(
                name: "IX_Citizenships_CompanyId_Name",
                table: "Citizenships");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TourOperators",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartnerMarks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Citizenships",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_TourOperators_CompanyId",
                table: "TourOperators",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerMarks_CompanyId",
                table: "PartnerMarks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizenships_CompanyId",
                table: "Citizenships",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citizenships_Companies_CompanyId",
                table: "Citizenships",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerMarks_Companies_CompanyId",
                table: "PartnerMarks",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourOperators_Companies_CompanyId",
                table: "TourOperators",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
