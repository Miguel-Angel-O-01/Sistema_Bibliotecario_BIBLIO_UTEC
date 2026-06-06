using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPrecio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Libros",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Libros");
        }
    }
}
