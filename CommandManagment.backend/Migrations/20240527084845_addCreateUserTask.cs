using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addCreateUserTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreateUserTaskId",
                table: "ScrumBoardTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ScrumBoardTasks_CreateUserTaskId",
                table: "ScrumBoardTasks",
                column: "CreateUserTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScrumBoardTasks_Users_CreateUserTaskId",
                table: "ScrumBoardTasks",
                column: "CreateUserTaskId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScrumBoardTasks_Users_CreateUserTaskId",
                table: "ScrumBoardTasks");

            migrationBuilder.DropIndex(
                name: "IX_ScrumBoardTasks_CreateUserTaskId",
                table: "ScrumBoardTasks");

            migrationBuilder.DropColumn(
                name: "CreateUserTaskId",
                table: "ScrumBoardTasks");
        }
    }
}
