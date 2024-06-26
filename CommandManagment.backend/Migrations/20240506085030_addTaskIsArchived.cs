﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    /// <inheritdoc />
    public partial class addTaskIsArchived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ScrumBoardTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ScrumBoardTasks");
        }
    }
}
