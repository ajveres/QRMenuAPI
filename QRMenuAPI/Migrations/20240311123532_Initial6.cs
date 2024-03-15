using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRMenuAPI.Migrations
{
    public partial class Initial6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_RestaurantUsers_Restaurants_RestaurantId",
                table: "RestaurantUsers",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
