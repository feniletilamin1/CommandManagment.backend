using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addscrumboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrumBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrumBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrumBoards_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScrumBoards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScrumBoardColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ScrumBoardId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrumBoardColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrumBoardColumns_ScrumBoards_ScrumBoardId",
                        column: x => x.ScrumBoardId,
                        principalTable: "ScrumBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrumBoardTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScrumBoardId = table.Column<int>(type: "int", nullable: false),
                    ScrumBoardColumnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrumBoardTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrumBoardTasks_ScrumBoardColumns_ScrumBoardColumnId",
                        column: x => x.ScrumBoardColumnId,
                        principalTable: "ScrumBoardColumns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ScrumBoardTasks_ScrumBoards_ScrumBoardId",
                        column: x => x.ScrumBoardId,
                        principalTable: "ScrumBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoardColumns_ScrumBoardId",
                table: "ScrumBoardColumns",
                column: "ScrumBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoards_ProjectId",
                table: "ScrumBoards",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoards_UserId",
                table: "ScrumBoards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoardTasks_ScrumBoardColumnId",
                table: "ScrumBoardTasks",
                column: "ScrumBoardColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoardTasks_ScrumBoardId",
                table: "ScrumBoardTasks",
                column: "ScrumBoardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrumBoardTasks");

            migrationBuilder.DropTable(
                name: "ScrumBoardColumns");

            migrationBuilder.DropTable(
                name: "ScrumBoards");
        }
    }
}
