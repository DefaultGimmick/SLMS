using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SLMS.EntityFrameworkCore.Migrations
{
    public partial class SLMSMigration20230613 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNumber = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Book",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    ISBN = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Publisher = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    BookshelfNumber = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Book", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_Book_TB_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "TB_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_Book_TB_User_UserId",
                        column: x => x.UserId,
                        principalTable: "TB_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_BorrowRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    BorrowDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_BorrowRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_BorrowRecord_TB_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "TB_Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_BorrowRecord_TB_User_UserId",
                        column: x => x.UserId,
                        principalTable: "TB_User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TB_Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemainingQuantity = table.Column<int>(nullable: false),
                    TotalQuantity = table.Column<int>(nullable: false),
                    BookId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_Inventory_TB_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "TB_Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Book_CategoryId",
                table: "TB_Book",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Book_UserId",
                table: "TB_Book",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_BorrowRecord_BookId",
                table: "TB_BorrowRecord",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_BorrowRecord_UserId",
                table: "TB_BorrowRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Inventory_BookId",
                table: "TB_Inventory",
                column: "BookId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_BorrowRecord");

            migrationBuilder.DropTable(
                name: "TB_Inventory");

            migrationBuilder.DropTable(
                name: "TB_Book");

            migrationBuilder.DropTable(
                name: "TB_Category");

            migrationBuilder.DropTable(
                name: "TB_User");
        }
    }
}
