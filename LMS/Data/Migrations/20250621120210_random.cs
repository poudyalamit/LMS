using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class random : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Modules_TypeId",
                table: "Modules");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_TypeId",
                table: "Modules",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Modules_TypeId",
                table: "Modules");

            migrationBuilder.CreateIndex(
                name: "IX_Modules_TypeId",
                table: "Modules",
                column: "TypeId",
                unique: true);
        }
    }
}
