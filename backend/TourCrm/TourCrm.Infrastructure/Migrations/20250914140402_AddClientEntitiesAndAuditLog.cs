using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TourCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientEntitiesAndAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    AtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientType = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<int>(type: "integer", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstNameGenitive = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastNameGenitive = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    MiddleNameGenitive = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BirthDay = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    PhoneE164 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    IsSubscribedToMailing = table.Column<bool>(type: "boolean", nullable: false),
                    IsEmailNotificationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ReferredBy = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IsTourist = table.Column<bool>(type: "boolean", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BirthCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IssuedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BirthCertificates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CitizenshipCountryId = table.Column<int>(type: "integer", nullable: true),
                    ResidenceCountryId = table.Column<int>(type: "integer", nullable: true),
                    ResidenceCityId = table.Column<int>(type: "integer", nullable: true),
                    BirthPlace = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IssuedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DocumentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonalNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RegistrationAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ResidentialAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MotherFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FatherFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContactInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityDocuments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstNameLatin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastNameLatin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IssuingAuthority = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passports_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisaRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    IsSchengen = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisaRecords_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CompanyId_Entity_EntityId_AtUtc",
                table: "AuditLogs",
                columns: new[] { "CompanyId", "Entity", "EntityId", "AtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BirthCertificates_ClientId",
                table: "BirthCertificates",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BirthCertificates_CompanyId_SerialNumber",
                table: "BirthCertificates",
                columns: new[] { "CompanyId", "SerialNumber" },
                unique: true,
                filter: "\"SerialNumber\" IS NOT NULL AND \"SerialNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_Email",
                table: "Clients",
                columns: new[] { "CompanyId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId_PhoneE164",
                table: "Clients",
                columns: new[] { "CompanyId", "PhoneE164" },
                unique: true,
                filter: "\"PhoneE164\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDocuments_ClientId",
                table: "IdentityDocuments",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDocuments_CompanyId_DocumentNumber",
                table: "IdentityDocuments",
                columns: new[] { "CompanyId", "DocumentNumber" },
                unique: true,
                filter: "\"DocumentNumber\" IS NOT NULL AND \"DocumentNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityDocuments_CompanyId_PersonalNumber",
                table: "IdentityDocuments",
                columns: new[] { "CompanyId", "PersonalNumber" },
                unique: true,
                filter: "\"PersonalNumber\" IS NOT NULL AND \"PersonalNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_ClientId",
                table: "InsurancePolicies",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_CompanyId_ClientId_ExpireDate",
                table: "InsurancePolicies",
                columns: new[] { "CompanyId", "ClientId", "ExpireDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Passports_ClientId",
                table: "Passports",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passports_CompanyId_SerialNumber",
                table: "Passports",
                columns: new[] { "CompanyId", "SerialNumber" },
                unique: true,
                filter: "\"SerialNumber\" IS NOT NULL AND \"SerialNumber\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_VisaRecords_ClientId",
                table: "VisaRecords",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_VisaRecords_CompanyId_ClientId_ExpireDate",
                table: "VisaRecords",
                columns: new[] { "CompanyId", "ClientId", "ExpireDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.DropTable(name: "BirthCertificates");
            migrationBuilder.DropTable(name: "IdentityDocuments");
            migrationBuilder.DropTable(name: "InsurancePolicies");
            migrationBuilder.DropTable(name: "Passports");
            migrationBuilder.DropTable(name: "VisaRecords");
            migrationBuilder.DropTable(name: "Clients");
        }
    }
}
