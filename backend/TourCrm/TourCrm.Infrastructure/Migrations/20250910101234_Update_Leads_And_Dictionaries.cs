using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Leads_And_Dictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- подготовка к переименованию таблицы с опечаткой ---
            migrationBuilder.DropForeignKey(
                name: "FK_AccomodaionTypes_Companies_CompanyId",
                table: "AccomodaionTypes");

            // старый индекс по Cities мог отличаться — убираем, чтобы завести составной
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccomodaionTypes",
                table: "AccomodaionTypes");

            // --- переименование таблицы и индекса ---
            migrationBuilder.RenameTable(
                name: "AccomodaionTypes",
                newName: "AccommodationTypes");

            migrationBuilder.RenameIndex(
                name: "IX_AccomodaionTypes_CompanyId",
                table: "AccommodationTypes",
                newName: "IX_AccommodationTypes_CompanyId");

            // --- изменения длин полей ---
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NumberTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccommodationTypes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccommodationTypes",
                table: "AccommodationTypes",
                column: "Id");

            // --- новые таблицы (обычные) ---
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    Stars = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // --- ИДЕМПОТЕНТНО: словари, которые могли существовать ранее ---
            // Labels
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""Labels"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    ""Code"" character varying(64) NOT NULL,
    ""Name"" character varying(128) NOT NULL,
    ""IsActive"" boolean NOT NULL
);");
            migrationBuilder.Sql(@"ALTER TABLE ""Labels"" ADD COLUMN IF NOT EXISTS ""IsActive"" boolean NOT NULL DEFAULT true;");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Labels_Code"" ON ""Labels"" (""Code"");");

            // LeadStatuses
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadStatuses"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    ""Code"" character varying(64) NOT NULL,
    ""Name"" character varying(128) NOT NULL,
    ""IsActive"" boolean NOT NULL
);");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_LeadStatuses_Code"" ON ""LeadStatuses"" (""Code"");");

            // LeadSources
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadSources"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    ""Code"" character varying(64) NOT NULL,
    ""Name"" character varying(128) NOT NULL,
    ""IsActive"" boolean NOT NULL
);");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_LeadSources_Code"" ON ""LeadSources"" (""Code"");");

            // LeadRequestTypes
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadRequestTypes"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    ""Code"" character varying(64) NOT NULL,
    ""Name"" character varying(128) NOT NULL,
    ""IsActive"" boolean NOT NULL
);");
            migrationBuilder.Sql(@"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_LeadRequestTypes_Code"" ON ""LeadRequestTypes"" (""Code"");");

            // LeadSelections
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadSelections"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
    ""LeadId"" integer NOT NULL,
    ""DepartureCity"" character varying(120) NOT NULL,
    ""Country"" character varying(120) NOT NULL,
    ""City"" character varying(120) NOT NULL,
    ""Hotel"" character varying(200),
    ""RoomType"" character varying(120),
    ""Accommodation"" character varying(64) NOT NULL,
    ""MealPlan"" character varying(32) NOT NULL,
    ""StartDate"" date NULL,
    ""Nights"" integer NULL,
    ""Adults"" integer NULL,
    ""Children"" integer NULL,
    ""Infants"" integer NULL,
    ""Link"" character varying(500),
    ""Note"" character varying(2000),
    ""PartnerId"" integer NULL,
    ""PartnerName"" character varying(200),
    ""Price"" numeric(18,2) NULL,
    ""Currency"" character varying(8) NOT NULL,
    ""CreatedByUserId"" character varying(64) NOT NULL,
    ""CreatedAt"" timestamp with time zone NOT NULL,
    ""UpdatedAt"" timestamp with time zone NULL
);");

            // FK к Leads (если вдруг уже есть — сначала пробуем создать, иначе игнор)
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_LeadSelections_Leads_LeadId'
    ) THEN
        ALTER TABLE ""LeadSelections""
        ADD CONSTRAINT ""FK_LeadSelections_Leads_LeadId""
        FOREIGN KEY (""LeadId"") REFERENCES ""Leads"" (""Id"") ON DELETE CASCADE;
    END IF;
END
$$;");

            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_LeadSelections_LeadId"" ON ""LeadSelections"" (""LeadId"");");

            // --- индексы для новых таблиц и изменённых сущностей ---
            migrationBuilder.CreateIndex(
                name: "IX_NumberTypes_Name",
                table: "NumberTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId_Name",
                table: "Cities",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId_Name",
                table: "Hotels",
                columns: new[] { "CityId", "Name" },
                unique: true);

            // --- возвращаем FK с нужным поведением ---
            migrationBuilder.AddForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Безопасно убираем только то, что точно создавали здесь

            migrationBuilder.DropForeignKey(
                name: "FK_AccommodationTypes_Companies_CompanyId",
                table: "AccommodationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            // индексы
            migrationBuilder.DropIndex(
                name: "IX_NumberTypes_Name",
                table: "NumberTypes");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId_Name",
                table: "Cities");

            // Таблицы, созданные только этой миграцией обычным способом
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Hotels");

            // Откаты длин полей
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "NumberTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MealTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Countries",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AccommodationTypes",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            // Возврат названия таблицы с опечаткой
            migrationBuilder.DropPrimaryKey(
                name: "PK_AccommodationTypes",
                table: "AccommodationTypes");

            migrationBuilder.RenameTable(
                name: "AccommodationTypes",
                newName: "AccomodaionTypes");

            migrationBuilder.RenameIndex(
                name: "IX_AccommodationTypes_CompanyId",
                table: "AccomodaionTypes",
                newName: "IX_AccomodaionTypes_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccomodaionTypes",
                table: "AccomodaionTypes",
                column: "Id");

            // Возврат старого индекса городов
            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            // Возврат старых FK
            migrationBuilder.AddForeignKey(
                name: "FK_AccomodaionTypes_Companies_CompanyId",
                table: "AccomodaionTypes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Индексы, созданные через SQL (Labels/Lead*), тут не трогаем.
            // Таблицы Labels/LeadSelections/LeadStatuses/LeadSources/LeadRequestTypes тоже не дропаем,
            // т.к. они могли существовать до этой миграции.
        }
    }
}