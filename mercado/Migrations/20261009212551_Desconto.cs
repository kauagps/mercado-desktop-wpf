using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mercado.Migrations
{
    /// <inheritdoc />
    public partial class Desconto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Desconto",
                table: "ItensVenda",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "ItensVenda");
        }
    }
}
