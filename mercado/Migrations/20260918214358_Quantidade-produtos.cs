using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mercado.Migrations
{
    /// <inheritdoc />
    public partial class Quantidadeprodutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Produtos",
                newName: "QuantidadeAtual");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeAtual",
                table: "Produtos",
                newName: "Quantidade");
        }
    }
}
