using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Cities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CompanyId",
                table: "Cities",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Companies_CompanyId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CompanyId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Cities");
        }
    }
}
