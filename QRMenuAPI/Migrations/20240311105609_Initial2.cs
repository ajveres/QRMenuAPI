using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QRMenuAPI.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PostalCode = table.Column<string>(type: "char(5)", maxLength: 5, nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegisterationDate = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurants_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Restaurants_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_CompanyId",
                table: "Restaurants",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_StateId",
                table: "Restaurants",
                column: "StateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Restaurants");
        }
    }
}
