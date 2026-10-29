using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mercado.Migrations
{
    /// <inheritdoc />
    public partial class ForcarTrocaSenhaAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeveTrocarSenha",
                table: "Usuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "DeveTrocarSenha",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeveTrocarSenha",
                table: "Usuarios");
        }
    }
}
