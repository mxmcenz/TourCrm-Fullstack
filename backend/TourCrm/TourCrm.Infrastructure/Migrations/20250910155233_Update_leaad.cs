using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_leaad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- «Мягкие» удаление FK и старых колонок ---
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP CONSTRAINT IF EXISTS ""FK_Leads_Users_AssignedToUserId"";");

            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""Comment"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""CreatedOn"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""EndDate"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""FullName"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""Price"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""Status"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Leads"" DROP COLUMN IF EXISTS ""TourName"";");

            // --- «Мягкие» переименования колонок (только если исходные существуют) ---
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='TouristId') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""TouristId"" TO ""SourceId""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='StartDate') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""StartDate"" TO ""CreatedAt""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='IsClosed') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""IsClosed"" TO ""PrecontractRequired""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='AssignedToUserId') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""AssignedToUserId"" TO ""RequestTypeId""';
  END IF;
END $$;");

            // --- Переименование индекса (если был) ---
            migrationBuilder.Sql(@"ALTER INDEX IF EXISTS ""IX_Leads_AssignedToUserId"" RENAME TO ""IX_Leads_RequestTypeId"";");

            // --- Приведение типов для Phone/Email (безопасно, колонки есть в обеих схемах) ---
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
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            // --- Хелпер для добавления колонок только если их нет ---
            void AddColumnIfNotExists(string column, string sqlType, string nullability, string? defaultSql = null)
            {
                var defaultClause = defaultSql is null ? "" : $" DEFAULT {defaultSql}";
                migrationBuilder.Sql($@"
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM information_schema.columns 
    WHERE table_schema='public' AND table_name='Leads' AND column_name='{column}'
  ) THEN
    EXECUTE 'ALTER TABLE ""Leads"" ADD COLUMN ""{column}"" {sqlType}{(string.IsNullOrEmpty(nullability) ? "" : " " + nullability)}{defaultClause}';
  END IF;
END $$;");
            }

            // --- Новые/нужные колонки (идемпотентно) ---
            AddColumnIfNotExists("Accommodation",      "character varying(64)",   "NULL");
            AddColumnIfNotExists("Adults",             "integer",                 "NULL");
            AddColumnIfNotExists("Budget",             "numeric(18,2)",           "NULL");
            AddColumnIfNotExists("Children",           "integer",                 "NULL");
            AddColumnIfNotExists("Country",            "character varying(120)",  "NULL");
            AddColumnIfNotExists("CreatedByUserId",    "character varying(64)",   "NOT NULL", "''");
            AddColumnIfNotExists("CustomerFirstName",  "character varying(120)",  "NOT NULL", "''");
            AddColumnIfNotExists("CustomerLastName",   "character varying(120)",  "NOT NULL", "''");
            AddColumnIfNotExists("CustomerMiddleName", "character varying(120)",  "NULL");
            AddColumnIfNotExists("CustomerType",       "character varying(32)",   "NOT NULL", "''");
            AddColumnIfNotExists("DesiredArrival",     "date",                    "NULL");
            AddColumnIfNotExists("DesiredDeparture",   "date",                    "NULL");
            AddColumnIfNotExists("DocsPackageDate",    "date",                    "NULL");
            AddColumnIfNotExists("Infants",            "integer",                 "NULL");
            AddColumnIfNotExists("InvoiceRequired",    "boolean",                 "NOT NULL", "false");
            AddColumnIfNotExists("IsDeleted",          "boolean",                 "NOT NULL", "false");
            AddColumnIfNotExists("LeadNumber",         "character varying(32)",   "NOT NULL", "''");
            AddColumnIfNotExists("LeadStatusId",       "integer",                 "NOT NULL", "0");
            AddColumnIfNotExists("ManagerFullName",    "character varying(200)",  "NULL");
            AddColumnIfNotExists("ManagerId",          "integer",                 "NULL");
            AddColumnIfNotExists("MealPlan",           "character varying(32)",   "NULL");
            AddColumnIfNotExists("NightsFrom",         "integer",                 "NULL");
            AddColumnIfNotExists("NightsTo",           "integer",                 "NULL");
            AddColumnIfNotExists("Note",               "character varying(2000)", "NULL");
            AddColumnIfNotExists("UpdatedAt",          "timestamp with time zone","NULL");

            // --- Таблицы LeadHistories / LeadLabels (если ещё нет) ---
            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadHistories"" (
  ""Id""            integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
  ""LeadId""        integer NOT NULL,
  ""CreatedAt""     timestamp with time zone NOT NULL,
  ""Action""        character varying(32)  NOT NULL,
  ""ActorUserId""   character varying(64)  NOT NULL,
  ""ActorFullName"" character varying(200) NULL,
  ""SnapshotJson""  text                   NOT NULL
);");

            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_LeadHistories_Leads_LeadId') THEN
    ALTER TABLE ""LeadHistories""
      ADD CONSTRAINT ""FK_LeadHistories_Leads_LeadId""
      FOREIGN KEY (""LeadId"") REFERENCES ""Leads""(""Id"") ON DELETE CASCADE;
  END IF;
END $$;");

            migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS ""LeadLabels"" (
  ""LeadId""  integer NOT NULL,
  ""LabelId"" integer NOT NULL,
  CONSTRAINT ""PK_LeadLabels"" PRIMARY KEY (""LeadId"", ""LabelId"")
);");

            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_LeadLabels_Leads_LeadId') THEN
    ALTER TABLE ""LeadLabels""
      ADD CONSTRAINT ""FK_LeadLabels_Leads_LeadId""
      FOREIGN KEY (""LeadId"") REFERENCES ""Leads""(""Id"") ON DELETE CASCADE;
  END IF;
  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_LeadLabels_Labels_LabelId') THEN
    ALTER TABLE ""LeadLabels""
      ADD CONSTRAINT ""FK_LeadLabels_Labels_LabelId""
      FOREIGN KEY (""LabelId"") REFERENCES ""Labels""(""Id"") ON DELETE RESTRICT;
  END IF;
END $$;");

            // --- Индексы (если ещё нет) ---
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Leads_LeadStatusId"" ON ""Leads"" (""LeadStatusId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Leads_ManagerId""    ON ""Leads"" (""ManagerId"");");
            migrationBuilder.Sql(@"CREATE INDEX IF NOT EXISTS ""IX_Leads_SourceId""     ON ""Leads"" (""SourceId"");");

            // --- Внешние ключи (если ещё нет) ---
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Leads_Employees_ManagerId') THEN
    ALTER TABLE ""Leads""
      ADD CONSTRAINT ""FK_Leads_Employees_ManagerId""
      FOREIGN KEY (""ManagerId"") REFERENCES ""Employees""(""Id"") ON DELETE RESTRICT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Leads_LeadRequestTypes_RequestTypeId') THEN
    ALTER TABLE ""Leads""
      ADD CONSTRAINT ""FK_Leads_LeadRequestTypes_RequestTypeId""
      FOREIGN KEY (""RequestTypeId"") REFERENCES ""LeadRequestTypes""(""Id"") ON DELETE RESTRICT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Leads_LeadSources_SourceId') THEN
    ALTER TABLE ""Leads""
      ADD CONSTRAINT ""FK_Leads_LeadSources_SourceId""
      FOREIGN KEY (""SourceId"") REFERENCES ""LeadSources""(""Id"") ON DELETE RESTRICT;
  END IF;

  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Leads_LeadStatuses_LeadStatusId') THEN
    ALTER TABLE ""Leads""
      ADD CONSTRAINT ""FK_Leads_LeadStatuses_LeadStatusId""
      FOREIGN KEY (""LeadStatusId"") REFERENCES ""LeadStatuses""(""Id"") ON DELETE RESTRICT;
  END IF;
END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Leads_Employees_ManagerId", table: "Leads");
            migrationBuilder.DropForeignKey(name: "FK_Leads_LeadRequestTypes_RequestTypeId", table: "Leads");
            migrationBuilder.DropForeignKey(name: "FK_Leads_LeadSources_SourceId", table: "Leads");
            migrationBuilder.DropForeignKey(name: "FK_Leads_LeadStatuses_LeadStatusId", table: "Leads");

            migrationBuilder.DropTable(name: "LeadHistories");
            migrationBuilder.DropTable(name: "LeadLabels");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Leads_LeadStatusId"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Leads_ManagerId"";");
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Leads_SourceId"";");

            migrationBuilder.DropColumn(name: "Accommodation",   table: "Leads");
            migrationBuilder.DropColumn(name: "Adults",          table: "Leads");
            migrationBuilder.DropColumn(name: "Budget",          table: "Leads");
            migrationBuilder.DropColumn(name: "Children",        table: "Leads");
            migrationBuilder.DropColumn(name: "Country",         table: "Leads");
            migrationBuilder.DropColumn(name: "CreatedByUserId", table: "Leads");
            migrationBuilder.DropColumn(name: "CustomerFirstName", table: "Leads");
            migrationBuilder.DropColumn(name: "CustomerLastName",  table: "Leads");
            migrationBuilder.DropColumn(name: "CustomerMiddleName",table: "Leads");
            migrationBuilder.DropColumn(name: "CustomerType",    table: "Leads");
            migrationBuilder.DropColumn(name: "DesiredArrival",  table: "Leads");
            migrationBuilder.DropColumn(name: "DesiredDeparture",table: "Leads");
            migrationBuilder.DropColumn(name: "DocsPackageDate", table: "Leads");
            migrationBuilder.DropColumn(name: "Infants",         table: "Leads");
            migrationBuilder.DropColumn(name: "InvoiceRequired", table: "Leads");
            migrationBuilder.DropColumn(name: "IsDeleted",       table: "Leads");
            migrationBuilder.DropColumn(name: "LeadNumber",      table: "Leads");
            migrationBuilder.DropColumn(name: "LeadStatusId",    table: "Leads");
            migrationBuilder.DropColumn(name: "ManagerFullName", table: "Leads");
            migrationBuilder.DropColumn(name: "ManagerId",       table: "Leads");
            migrationBuilder.DropColumn(name: "MealPlan",        table: "Leads");
            migrationBuilder.DropColumn(name: "NightsFrom",      table: "Leads");
            migrationBuilder.DropColumn(name: "NightsTo",        table: "Leads");
            migrationBuilder.DropColumn(name: "Note",            table: "Leads");
            migrationBuilder.DropColumn(name: "UpdatedAt",       table: "Leads");

            // «Мягкий» откат имён колонок
            migrationBuilder.Sql(@"
DO $$
BEGIN
  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='SourceId') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""SourceId"" TO ""TouristId""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='RequestTypeId') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""RequestTypeId"" TO ""AssignedToUserId""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='PrecontractRequired') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""PrecontractRequired"" TO ""IsClosed""';
  END IF;

  IF EXISTS (SELECT 1 FROM information_schema.columns 
             WHERE table_schema='public' AND table_name='Leads' AND column_name='CreatedAt') THEN
    EXECUTE 'ALTER TABLE ""Leads"" RENAME COLUMN ""CreatedAt"" TO ""StartDate""';
  END IF;
END $$;");

            // Возврат имени индекса (если надо)
            migrationBuilder.Sql(@"ALTER INDEX IF EXISTS ""IX_Leads_RequestTypeId"" RENAME TO ""IX_Leads_AssignedToUserId"";");

            // Типы назад
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
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            // Восстановление старых полей (если делаем полный откат схемы)
            migrationBuilder.AddColumn<string>(name: "Comment",   table: "Leads", type: "text", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<DateTime>(name: "CreatedOn", table: "Leads", type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1,1,1,0,0,0,DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<DateTime>(name: "EndDate", table: "Leads", type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(1,1,1,0,0,0,DateTimeKind.Unspecified));
            migrationBuilder.AddColumn<string>(name: "FullName",  table: "Leads", type: "text", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<decimal>(name: "Price",    table: "Leads", type: "numeric", nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<string>(name: "Status",    table: "Leads", type: "text", nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<string>(name: "TourName",  table: "Leads", type: "text", nullable: false, defaultValue: "");

            // Старая FK (как была изначально)
            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Users_AssignedToUserId",
                table: "Leads",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}