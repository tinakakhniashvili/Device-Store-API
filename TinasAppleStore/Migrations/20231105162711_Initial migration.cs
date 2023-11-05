using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TinasAppleStore.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                   // productId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.productId);
                    /* table.ForeignKey(
                         name: "FK_Products_Products_productId1",
                         column: x => x.productId1,
                         principalTable: "Products",
                         principalColumn: "productId"); */
                });
        }

         /*   migrationBuilder.CreateIndex(
                name: "IX_Products_productId1",
                table: "Products",
                column: "productId1");
        } */

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
