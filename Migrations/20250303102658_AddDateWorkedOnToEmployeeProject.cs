using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformanceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddDateWorkedOnToEmployeeProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateWorkedOn",
                table: "EmployeeProjects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateWorkedOn",
                table: "EmployeeProjects");
        }
    }
}
