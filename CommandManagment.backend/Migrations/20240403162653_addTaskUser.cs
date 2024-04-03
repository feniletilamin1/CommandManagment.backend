using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addTaskUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponsibleUserId",
                table: "ScrumBoardTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoardTasks_ResponsibleUserId",
                table: "ScrumBoardTasks",
                column: "ResponsibleUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScrumBoardTasks_Users_ResponsibleUserId",
                table: "ScrumBoardTasks",
                column: "ResponsibleUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScrumBoardTasks_Users_ResponsibleUserId",
                table: "ScrumBoardTasks");

            migrationBuilder.DropIndex(
                name: "IX_ScrumBoardTasks_ResponsibleUserId",
                table: "ScrumBoardTasks");

            migrationBuilder.DropColumn(
                name: "ResponsibleUserId",
                table: "ScrumBoardTasks");
        }
    }
}
