using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModelRepoBrowser.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    HostNameId = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    TechnicalContact = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.HostNameId);
                });

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    PrecursorVersion = table.Column<string>(type: "text", nullable: true),
                    PublishingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Owner = table.Column<string>(type: "text", nullable: true),
                    File = table.Column<List<string>>(type: "text[]", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    ReferencedModels = table.Column<List<string>>(type: "text[]", nullable: false),
                    RepositoryHostNameId = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalogs_Repositories_RepositoryHostNameId",
                        column: x => x.RepositoryHostNameId,
                        principalTable: "Repositories",
                        principalColumn: "HostNameId");
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MD5 = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SchemaLanguage = table.Column<string>(type: "text", nullable: false),
                    File = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    PublishingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DependsOnModel = table.Column<List<string>>(type: "text[]", nullable: false),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    Issuer = table.Column<string>(type: "text", nullable: true),
                    TechnicalContact = table.Column<string>(type: "text", nullable: true),
                    FurtherInformation = table.Column<string>(type: "text", nullable: true),
                    ModelRepositoryHostNameId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Repositories_ModelRepositoryHostNameId",
                        column: x => x.ModelRepositoryHostNameId,
                        principalTable: "Repositories",
                        principalColumn: "HostNameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RepositoryRepository",
                columns: table => new
                {
                    ParentSitesHostNameId = table.Column<string>(type: "text", nullable: false),
                    SubsidiarySitesHostNameId = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryRepository", x => new { x.ParentSitesHostNameId, x.SubsidiarySitesHostNameId });
                    table.ForeignKey(
                        name: "FK_RepositoryRepository_Repositories_ParentSitesHostNameId",
                        column: x => x.ParentSitesHostNameId,
                        principalTable: "Repositories",
                        principalColumn: "HostNameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RepositoryRepository_Repositories_SubsidiarySitesHostNameId",
                        column: x => x.SubsidiarySitesHostNameId,
                        principalTable: "Repositories",
                        principalColumn: "HostNameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_RepositoryHostNameId",
                table: "Catalogs",
                column: "RepositoryHostNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ModelRepositoryHostNameId",
                table: "Models",
                column: "ModelRepositoryHostNameId");

            migrationBuilder.CreateIndex(
                name: "IX_RepositoryRepository_SubsidiarySitesHostNameId",
                table: "RepositoryRepository",
                column: "SubsidiarySitesHostNameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "RepositoryRepository");

            migrationBuilder.DropTable(
                name: "Repositories");
        }
    }
}
