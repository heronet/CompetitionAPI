using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionAPI.Data.Migrations
{
    public partial class TSCUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TscCollection_CompetitionId",
                table: "TscCollection",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TscCollection_StudentId",
                table: "TscCollection",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TscCollection_TeacherId",
                table: "TscCollection",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TscCollection_AspNetUsers_TeacherId",
                table: "TscCollection",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TscCollection_Competitions_CompetitionId",
                table: "TscCollection",
                column: "CompetitionId",
                principalTable: "Competitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TscCollection_Students_StudentId",
                table: "TscCollection",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TscCollection_AspNetUsers_TeacherId",
                table: "TscCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_TscCollection_Competitions_CompetitionId",
                table: "TscCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_TscCollection_Students_StudentId",
                table: "TscCollection");

            migrationBuilder.DropIndex(
                name: "IX_TscCollection_CompetitionId",
                table: "TscCollection");

            migrationBuilder.DropIndex(
                name: "IX_TscCollection_StudentId",
                table: "TscCollection");

            migrationBuilder.DropIndex(
                name: "IX_TscCollection_TeacherId",
                table: "TscCollection");
        }
    }
}
