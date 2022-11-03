using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OData.Sample.WebApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorldRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ISO = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameGER = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ISO = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameGER = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    WorldRegionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CountryRegions_WorldRegions_WorldRegionId",
                        column: x => x.WorldRegionId,
                        principalTable: "WorldRegions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DisplayNameGER = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EU = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    EWR = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    ISO2 = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ISO3 = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameGER = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CountryRegionId = table.Column<int>(type: "INTEGER", nullable: true),
                    WorldRegionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_CountryRegions_CountryRegionId",
                        column: x => x.CountryRegionId,
                        principalTable: "CountryRegions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Countries_WorldRegions_WorldRegionId",
                        column: x => x.WorldRegionId,
                        principalTable: "WorldRegions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CountryRegionId",
                table: "Countries",
                column: "CountryRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_WorldRegionId",
                table: "Countries",
                column: "WorldRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryRegions_WorldRegionId",
                table: "CountryRegions",
                column: "WorldRegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "CountryRegions");

            migrationBuilder.DropTable(
                name: "WorldRegions");
        }
    }
}
