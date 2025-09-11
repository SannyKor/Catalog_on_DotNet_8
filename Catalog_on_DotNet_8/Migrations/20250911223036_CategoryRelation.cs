using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog_on_DotNet_8.Migrations
{
    /// <inheritdoc />
    public partial class CategoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Units_UnitId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UnitId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryUnit",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryUnit", x => new { x.CategoriesId, x.UnitsId });
                    table.ForeignKey(
                        name: "FK_CategoryUnit_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryUnit_Units_UnitsId",
                        column: x => x.UnitsId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryUnit_UnitsId",
                table: "CategoryUnit",
                column: "UnitsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryUnit");

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "Categories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UnitId",
                table: "Categories",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Units_UnitId",
                table: "Categories",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }
    }
}
