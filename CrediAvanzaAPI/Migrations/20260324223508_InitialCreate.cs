
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediAvanzaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogoCodigos",
                columns: table => new
                {
                    nCodigo = table.Column<int>(type: "int", nullable: false),
                    nValor = table.Column<int>(type: "int", nullable: false),
                    cNomCod = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    nEstados = table.Column<int>(type: "int", nullable: true),
                    nTipoCodigo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoCodigos", x => new { x.nCodigo, x.nValor });
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    IdCompra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cProducto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    nCantidadCompra = table.Column<int>(type: "int", nullable: false),
                    nUnidadMedida = table.Column<int>(type: "int", nullable: false),
                    nPrecioXUnidad = table.Column<decimal>(type: "money", nullable: false),
                    nPrecioTotal = table.Column<decimal>(type: "money", nullable: false),
                    IdNegocio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraDetalle", x => x.IdCompra);
                });

            migrationBuilder.CreateTable(
                name: "Conyuge",
                columns: table => new
                {
                    IdConyuge = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cNombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cPrimerApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cSegundoApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    nTipoDocumento = table.Column<int>(type: "int", nullable: false),
                    cDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cTelefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    cCelular = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conyuge", x => x.IdConyuge);
                });

            migrationBuilder.CreateTable(
                name: "CredCalendario",
                columns: table => new
                {
                    IdCalendario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nNroCuota = table.Column<int>(type: "int", nullable: false),
                    dFecVenc = table.Column<DateTime>(type: "datetime", nullable: false),
                    dFecPago = table.Column<DateTime>(type: "datetime", nullable: true),
                    nCapital = table.Column<decimal>(type: "money", nullable: false),
                    nIntComp = table.Column<decimal>(type: "money", nullable: false),
                    nIntMor = table.Column<decimal>(type: "money", nullable: false),
                    nIgv = table.Column<decimal>(type: "money", nullable: false),
                    nCapPag = table.Column<decimal>(type: "money", nullable: false),
                    nIntPag = table.Column<decimal>(type: "money", nullable: false),
                    nIntMorPag = table.Column<decimal>(type: "money", nullable: false),
                    nIgvPag = table.Column<decimal>(type: "money", nullable: false),
                    nEstado = table.Column<int>(type: "int", nullable: false),
                    nNroCalen = table.Column<int>(type: "int", nullable: false),
                    dFecpro = table.Column<DateTime>(type: "datetime", nullable: true),
                    nTipoCargo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredCalendario_1", x => x.IdCalendario);
                });

            migrationBuilder.CreateTable(
                name: "CredCalendCond",
                columns: table => new
                {
                    IdCredCalendCond = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nDiaFijo = table.Column<int>(type: "int", nullable: true),
                    nCuotas = table.Column<int>(type: "int", nullable: false),
                    nPlazo = table.Column<int>(type: "int", nullable: false),
                    nNroCalen = table.Column<int>(type: "int", nullable: false),
                    nGasto = table.Column<decimal>(type: "money", nullable: false),
                    bCobroSab = table.Column<bool>(type: "bit", nullable: true),
                    bCobroDom = table.Column<bool>(type: "bit", nullable: true),
                    bCobroFer = table.Column<bool>(type: "bit", nullable: true),
                    bCuotaDoble = table.Column<bool>(type: "bit", nullable: true),
                    nCodLineaSecundario = table.Column<int>(type: "int", nullable: true),
                    nTipoCargo = table.Column<int>(type: "int", nullable: true),
                    IdCalendario = table.Column<int>(type: "int", nullable: true),
                    IdCalenGasto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredCalendCond", x => x.IdCredCalendCond);
                });

            migrationBuilder.CreateTable(
                name: "CredCalenGastos",
                columns: table => new
                {
                    IdCalenGasto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nNroCuota = table.Column<int>(type: "int", nullable: false),
                    nCodGasto = table.Column<int>(type: "int", nullable: false),
                    nMonto = table.Column<decimal>(type: "money", nullable: false),
                    nMontoPag = table.Column<decimal>(type: "money", nullable: false),
                    dFecAsig = table.Column<DateTime>(type: "datetime", nullable: false),
                    nCodAgePersAsig = table.Column<int>(type: "int", nullable: true),
                    nNroCalen = table.Column<int>(type: "int", nullable: false),
                    nMontoIGV = table.Column<decimal>(type: "money", nullable: true),
                    nMontoIGVPag = table.Column<decimal>(type: "money", nullable: true),
                    nMontoSinIGV = table.Column<decimal>(type: "money", nullable: true),
                    nMontoSinIGVPag = table.Column<decimal>(type: "money", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredCalenGastos", x => x.IdCalenGasto);
                });

            migrationBuilder.CreateTable(
                name: "CredCambioGasto",
                columns: table => new
                {
                    nIdCambio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nCodCred = table.Column<int>(type: "int", nullable: false),
                    dFechaCambio = table.Column<DateOnly>(type: "date", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    nMontoOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nMontoNuevo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredCambioGasto", x => x.nIdCambio);
                });

            migrationBuilder.CreateTable(
                name: "CredFeriado",
                columns: table => new
                {
                    nIdFeriado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dFecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    cDescripcion = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    bEstado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredFeriado", x => x.nIdFeriado);
                });

            migrationBuilder.CreateTable(
                name: "CredFeriadoAge",
                columns: table => new
                {
                    IdCredFeriadoAge = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nIdFeriado = table.Column<int>(type: "int", nullable: false),
                    nCodAge = table.Column<int>(type: "int", nullable: false),
                    dFecha = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredFeriadoAge", x => x.IdCredFeriadoAge);
                });

            migrationBuilder.CreateTable(
                name: "CredGastos",
                columns: table => new
                {
                    IdGasto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nProd = table.Column<int>(type: "int", nullable: false),
                    nSubProd = table.Column<int>(type: "int", nullable: false),
                    cDescripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    nTipoGasto = table.Column<int>(type: "int", nullable: false),
                    nValor = table.Column<decimal>(type: "money", nullable: false),
                    nRangoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nRangoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    nPeriodo = table.Column<int>(type: "int", nullable: false),
                    nTipoCargo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredGastos", x => x.IdGasto);
                });

            migrationBuilder.CreateTable(
                name: "Creditos",
                columns: table => new
                {
                    nCodCred = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nCodAge = table.Column<int>(type: "int", nullable: false),
                    nProd = table.Column<int>(type: "int", nullable: false),
                    nSubProd = table.Column<int>(type: "int", nullable: false),
                    nSaldoK = table.Column<decimal>(type: "money", nullable: false),
                    nPrestamo = table.Column<decimal>(type: "money", nullable: false),
                    dFecVig = table.Column<DateTime>(type: "datetime", nullable: false),
                    nCodLinea = table.Column<int>(type: "int", nullable: true),
                    nEstado = table.Column<int>(type: "int", nullable: false),
                    nPeriodo = table.Column<int>(type: "int", nullable: false),
                    nMontoCuota = table.Column<decimal>(type: "money", nullable: true),
                    nTasaComp = table.Column<decimal>(type: "money", nullable: true),
                    nTasaMor = table.Column<decimal>(type: "money", nullable: true),
                    nNroCuotas = table.Column<int>(type: "int", nullable: false),
                    nNroProxCuota = table.Column<int>(type: "int", nullable: false),
                    nMora = table.Column<decimal>(type: "money", nullable: true),
                    nDiasAtraso = table.Column<int>(type: "int", nullable: true),
                    nCiclo = table.Column<int>(type: "int", nullable: true),
                    nTasaComision = table.Column<decimal>(type: "money", nullable: true),
                    nCobroEnAgencia = table.Column<int>(type: "int", nullable: true),
                    nAceptaTerminos = table.Column<int>(type: "int", nullable: true),
                    IdPersona = table.Column<int>(type: "int", nullable: true),
                    IdConyuge = table.Column<int>(type: "int", nullable: true),
                    IdDocumentacion = table.Column<int>(type: "int", nullable: true),
                    IdGarantia = table.Column<int>(type: "int", nullable: true),
                    IdFiador = table.Column<int>(type: "int", nullable: true),
                    IdCalendario = table.Column<int>(type: "int", nullable: true),
                    IdCredCalendCond = table.Column<int>(type: "int", nullable: true),
                    IdNegocio = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditos_1", x => x.nCodCred);
                });

            migrationBuilder.CreateTable(
                name: "Documentacion",
                columns: table => new
                {
                    IdDocumentacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentacion", x => x.IdDocumentacion);
                });

            migrationBuilder.CreateTable(
                name: "Fiador",
                columns: table => new
                {
                    IdFiador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cNombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cPrimerApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cSegundoApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    nTipoDocumento = table.Column<int>(type: "int", nullable: false),
                    cDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cDireccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cTelefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    cCelular = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiador", x => x.IdFiador);
                });

            migrationBuilder.CreateTable(
                name: "FotoDocumentacion",
                columns: table => new
                {
                    IdFoto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vFoto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    IdTipoDocumentacion = table.Column<int>(type: "int", nullable: false),
                    IdDocumentacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoDocumentacion_1", x => x.IdFoto);
                });

            migrationBuilder.CreateTable(
                name: "FotoID",
                columns: table => new
                {
                    IdFoto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vFoto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    nTipoFoto = table.Column<int>(type: "int", nullable: false),
                    IdPersona = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foto", x => x.IdFoto);
                });

            migrationBuilder.CreateTable(
                name: "FotoNegocio",
                columns: table => new
                {
                    IdFoto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vFoto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    nTipoFoto = table.Column<int>(type: "int", nullable: false),
                    IdNegocio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoNegocio", x => x.IdFoto);
                });

            migrationBuilder.CreateTable(
                name: "Garantia",
                columns: table => new
                {
                    IdGarantia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nIdArticuloGarantia = table.Column<int>(type: "int", nullable: false),
                    nValor = table.Column<decimal>(type: "money", nullable: false),
                    nIdFotoGarantia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Garantia", x => x.IdGarantia);
                });

            migrationBuilder.CreateTable(
                name: "LogErrores",
                columns: table => new
                {
                    IdLogError = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origen = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoExcepcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    DatosAdicionales = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Usuario = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    IP = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    FechaError = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LogError__7B1F940EA93C6869", x => x.IdLogError);
                });

            migrationBuilder.CreateTable(
                name: "Negocio",
                columns: table => new
                {
                    IdNegocio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cNombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cDireccion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cSector = table.Column<int>(type: "int", nullable: false),
                    tHoraInicio = table.Column<TimeOnly>(type: "time", nullable: true),
                    tHoraCierre = table.Column<TimeOnly>(type: "time", nullable: true),
                    cTelefono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cGeolocalizacion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negocio", x => x.IdNegocio);
                });

            migrationBuilder.CreateTable(
                name: "Persona",
                columns: table => new
                {
                    IdPersona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nTipoDocumento = table.Column<int>(type: "int", nullable: false),
                    cDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    dFechaExpedicion = table.Column<DateOnly>(type: "date", nullable: false),
                    dFechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    nDepartamentoDoc = table.Column<int>(type: "int", nullable: false),
                    nMunicipioDoc = table.Column<int>(type: "int", nullable: false),
                    cNombres = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cPrimerApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    cSegundoApellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    nSexo = table.Column<int>(type: "int", nullable: false),
                    nNacionalidad = table.Column<int>(type: "int", nullable: false),
                    dFechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    nDepartamentoNacimiento = table.Column<int>(type: "int", nullable: false),
                    nMunicipioNacimiento = table.Column<int>(type: "int", nullable: false),
                    nEstadoCivil = table.Column<int>(type: "int", nullable: false),
                    nProfesion = table.Column<int>(type: "int", nullable: false),
                    nEscolaridad = table.Column<int>(type: "int", nullable: false),
                    cCorreo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persona", x => x.IdPersona);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    IdVenta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cProducto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    nCantidadVenta = table.Column<int>(type: "int", nullable: false),
                    nUnidadMedida = table.Column<int>(type: "int", nullable: false),
                    nPrecioXUnidad = table.Column<decimal>(type: "money", nullable: false),
                    nPrecioTotal = table.Column<decimal>(type: "money", nullable: false),
                    IdNegocio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.IdVenta);
                });

            migrationBuilder.CreateTable(
                name: "VerNegocio",
                columns: table => new
                {
                    nCodVar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cNomVar = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    cValorVar = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    nTipoVar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerNegocio", x => x.nCodVar);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogoCodigos");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Conyuge");

            migrationBuilder.DropTable(
                name: "CredCalendario");

            migrationBuilder.DropTable(
                name: "CredCalendCond");

            migrationBuilder.DropTable(
                name: "CredCalenGastos");

            migrationBuilder.DropTable(
                name: "CredCambioGasto");

            migrationBuilder.DropTable(
                name: "CredFeriado");

            migrationBuilder.DropTable(
                name: "CredFeriadoAge");

            migrationBuilder.DropTable(
                name: "CredGastos");

            migrationBuilder.DropTable(
                name: "Creditos");

            migrationBuilder.DropTable(
                name: "Documentacion");

            migrationBuilder.DropTable(
                name: "Fiador");

            migrationBuilder.DropTable(
                name: "FotoDocumentacion");

            migrationBuilder.DropTable(
                name: "FotoID");

            migrationBuilder.DropTable(
                name: "FotoNegocio");

            migrationBuilder.DropTable(
                name: "Garantia");

            migrationBuilder.DropTable(
                name: "LogErrores");

            migrationBuilder.DropTable(
                name: "Negocio");

            migrationBuilder.DropTable(
                name: "Persona");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "VerNegocio");
        }
    }
}
