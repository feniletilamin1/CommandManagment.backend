using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addNewRelship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScrumBoards_ProjectId",
                table: "ScrumBoards");

            migrationBuilder.AddColumn<int>(
                name: "BoardId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoards_ProjectId",
                table: "ScrumBoards",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ScrumBoards_ProjectId",
                table: "ScrumBoards");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoards_ProjectId",
                table: "ScrumBoards",
                column: "ProjectId");
        }
    }
}
