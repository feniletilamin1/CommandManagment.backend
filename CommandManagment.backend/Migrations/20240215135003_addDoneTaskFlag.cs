using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addDoneTaskFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "ScrumBoardTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "ScrumBoardTasks");
        }
    }
}
