using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace schoolapp.Migrations
{
    /// <inheritdoc />
    public partial class thirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectScores_Results_ResultId",
                table: "SubjectScores");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResultId",
                table: "SubjectScores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectScores_Results_ResultId",
                table: "SubjectScores",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectScores_Results_ResultId",
                table: "SubjectScores");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResultId",
                table: "SubjectScores",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectScores_Results_ResultId",
                table: "SubjectScores",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id");
        }
    }
}
