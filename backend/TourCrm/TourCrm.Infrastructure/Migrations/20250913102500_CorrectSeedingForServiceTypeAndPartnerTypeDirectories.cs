using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectSeedingForServiceTypeAndPartnerTypeDirectories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerTypes_Companies_CompanyId",
                table: "PartnerTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTypes_CompanyId",
                table: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_PartnerTypes_CompanyId",
                table: "PartnerTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartnerTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_CompanyId_Name",
                table: "ServiceTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerTypes_CompanyId_Name",
                table: "PartnerTypes",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerTypes_Companies_CompanyId",
                table: "PartnerTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_PartnerTypes_Companies_CompanyId",
                table: "PartnerTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypes_Companies_CompanyId",
                table: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTypes_CompanyId_Name",
                table: "ServiceTypes");

            migrationBuilder.DropIndex(
                name: "IX_PartnerTypes_CompanyId_Name",
                table: "PartnerTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ServiceTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PartnerTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_CompanyId",
                table: "ServiceTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerTypes_CompanyId",
                table: "PartnerTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerTypes_Companies_CompanyId",
                table: "PartnerTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
