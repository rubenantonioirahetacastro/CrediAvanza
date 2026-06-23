using CrediAvanzaAPI.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediAvanzaAPI.Migrations
{
    [DbContext(typeof(DbNegocioContext))]
    [Migration("20260623033000_AddCapacidadPagoToSolicitudCredito")]
    public partial class AddCapacidadPagoToSolicitudCredito : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCapacidadPago",
                table: "Creditos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CapacidadPago",
                columns: table => new
                {
                    IdCapacidadPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dGastosEducacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dGastosAlimentacion = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dGastosSalud = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dOtrosGastos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    dOtrosIngresos = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapacidadPago", x => x.IdCapacidadPago);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapacidadPago");

            migrationBuilder.DropColumn(
                name: "IdCapacidadPago",
                table: "Creditos");
        }
    }
}
