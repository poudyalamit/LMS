using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Modules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_TypeId",
                table: "Modules",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Types_TypeId",
                table: "Modules",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Types_TypeId",
                table: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Modules_TypeId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Modules");
        }
    }
}
