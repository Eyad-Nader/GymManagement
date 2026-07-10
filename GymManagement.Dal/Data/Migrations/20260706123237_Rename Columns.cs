using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Trainers",
                newName: "HireDate");

            migrationBuilder.RenameColumn(
                name: "Specialty",
                table: "Trainers",
                newName: "Specialization");

            // added
            migrationBuilder.DropCheckConstraint(
            name: "Session_Capacity",
            table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "capacity",
                table: "Sessions",
                newName: "Capacity");

            migrationBuilder.AddCheckConstraint(
                name: "Session_Capacity",
                table: "Sessions",
                sql: "[Capacity] Between 1 and 25");

            migrationBuilder.RenameColumn(
                name: "photo",
                table: "Members",
                newName: "Photo");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Members",
                newName: "JoinDate");

            migrationBuilder.RenameColumn(
                name: "weight",
                table: "HealthRecords",
                newName: "Weight");

            migrationBuilder.RenameColumn(
                name: "bloodtype",
                table: "HealthRecords",
                newName: "Bloodtype");

            migrationBuilder.AlterColumn<DateTime>(
                name: "HireDate",
                table: "Trainers",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinDate",
                table: "Members",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HireDate",
                table: "Trainers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Trainers",
                newName: "Specialty");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Sessions",
                newName: "capacity");

            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Members",
                newName: "photo");

            migrationBuilder.RenameColumn(
                name: "JoinDate",
                table: "Members",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "HealthRecords",
                newName: "weight");

            migrationBuilder.RenameColumn(
                name: "Bloodtype",
                table: "HealthRecords",
                newName: "bloodtype");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Trainers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Members",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
