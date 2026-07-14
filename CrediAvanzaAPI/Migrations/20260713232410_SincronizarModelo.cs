using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediAvanzaAPI.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Creditos_1",
                table: "Creditos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CredCalendario_1",
                table: "CredCalendario");

            migrationBuilder.DropColumn(
                name: "nIdArticuloGarantia",
                table: "Garantia");

            migrationBuilder.DropColumn(
                name: "nIdFotoGarantia",
                table: "Garantia");

            migrationBuilder.DropColumn(
                name: "nValor",
                table: "Garantia");

            migrationBuilder.DropColumn(
                name: "IdCalendario",
                table: "Creditos");

            migrationBuilder.DropColumn(
                name: "nNroProxCuota",
                table: "Creditos");

            migrationBuilder.DropColumn(
                name: "IdCalendario",
                table: "CredCalendCond");

            migrationBuilder.DropColumn(
                name: "nCodLineaSecundario",
                table: "CredCalendCond");

            migrationBuilder.DropColumn(
                name: "nGasto",
                table: "CredCalendCond");

            migrationBuilder.DropColumn(
                name: "nTipoCargo",
                table: "CredCalendCond");

            migrationBuilder.DropColumn(
                name: "dFecpro",
                table: "CredCalendario");

            migrationBuilder.RenameColumn(
                name: "nCiclo",
                table: "Creditos",
                newName: "IdCapacidadPago");

            migrationBuilder.RenameColumn(
                name: "nTipoCargo",
                table: "CredCalendario",
                newName: "nCodCred");

            migrationBuilder.RenameColumn(
                name: "IdCalendario",
                table: "CredCalendario",
                newName: "Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "nCantidadVenta",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "cCelular",
                table: "Persona",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cTelefono",
                table: "Persona",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "vFoto",
                table: "FotoNegocio",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "vFoto",
                table: "FotoDocumentacion",
                type: "varchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NGasto",
                table: "CredCalendario",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "nCodAge",
                table: "CredCalendario",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "nTotalCuota",
                table: "CredCalendario",
                type: "money",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "nCantidadCompra",
                table: "Compras",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creditos_1_Tmp",
                table: "Creditos",
                column: "nCodCred");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredCalendario",
                table: "CredCalendario",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Agencias",
                columns: table => new
                {
                    nCodAge = table.Column<int>(type: "int", nullable: false),
                    cNomAge = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    cDirecAge = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                    cTelefAge = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    cCorreoELectronico = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Agencias__771BAD3EF24D8FB8", x => x.nCodAge);
                });

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

            migrationBuilder.CreateTable(
                name: "CatSegmentoUsura",
                columns: table => new
                {
                    nCodSegmento = table.Column<int>(type: "int", nullable: false),
                    cDescripcion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    nMultSMMin = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nMultSMMax = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    bEstado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CatSegme__70B6321C501B9212", x => x.nCodSegmento);
                });

            migrationBuilder.CreateTable(
                name: "CredLineaCredito",
                columns: table => new
                {
                    nCodLinea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cLinea = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    nTasaCom = table.Column<decimal>(type: "money", nullable: false),
                    nProd = table.Column<int>(type: "int", nullable: false),
                    nSubProd = table.Column<int>(type: "int", nullable: false),
                    Periodicidad = table.Column<int>(type: "int", nullable: true),
                    nPlazoMin = table.Column<int>(type: "int", nullable: false),
                    nPlazoMax = table.Column<int>(type: "int", nullable: false),
                    nMontoMin = table.Column<decimal>(type: "money", nullable: false),
                    nMontoMax = table.Column<decimal>(type: "money", nullable: false),
                    bEstado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    cDescripcion = table.Column<string>(type: "varchar(400)", unicode: false, maxLength: 400, nullable: true),
                    bAplicaSegmentacionUsura = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CredLine__50A988C3CC0EBAA0", x => x.nCodLinea);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    IdDepartamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cNombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departam__787A433DF74032C7", x => x.IdDepartamento);
                });

            migrationBuilder.CreateTable(
                name: "GarantiaFoto",
                columns: table => new
                {
                    IdFoto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vFoto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    nValor = table.Column<decimal>(type: "money", nullable: false),
                    IdArticuloGarantia = table.Column<int>(type: "int", nullable: false),
                    IdGarantia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoGarantia", x => x.IdFoto);
                });

            migrationBuilder.CreateTable(
                name: "LineaCatalogoAuxiliar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cDescripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    nCatalogoCodigo = table.Column<int>(type: "int", nullable: true),
                    nProd = table.Column<int>(type: "int", nullable: true),
                    nSubProd = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineaCreditosTemp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordChangeAudits",
                columns: table => new
                {
                    IdAudit = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    IdPersona = table.Column<int>(type: "int", nullable: true),
                    Usuario = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Exito = table.Column<bool>(type: "bit", nullable: false),
                    FechaAttempt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    Ip = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: true),
                    Bloqueado = table.Column<bool>(type: "bit", nullable: true),
                    FechaBloqueo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoBloqueo = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Observacion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Password__C87E13DD960906D2", x => x.IdAudit);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__2A49584C591AAB31", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "SalarioMinimoVigente",
                columns: table => new
                {
                    nCodSalMin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cSector = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Comercio y Servicios"),
                    nMontoMensual = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    dFecInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    dFecFin = table.Column<DateOnly>(type: "date", nullable: true),
                    bEstado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SalarioM__24E830BDC3C7C532", x => x.nCodSalMin);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioLogin",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    cDocumento = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    cCorreo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Token = table.Column<int>(type: "int", nullable: true),
                    TokenTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    TokenCheck = table.Column<bool>(type: "bit", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: true),
                    IntentosFallidos = table.Column<int>(type: "int", nullable: true),
                    Bloqueado = table.Column<int>(type: "int", nullable: true),
                    FechaBloqueo = table.Column<int>(type: "int", nullable: true),
                    UltimoLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    bContrasenaTemporal = table.Column<bool>(type: "bit", nullable: true),
                    dFechaContrasenaTemporal = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuaroLogin", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "TasaMaximaBCR",
                columns: table => new
                {
                    nCodTasaMax = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nCodSegmento = table.Column<int>(type: "int", nullable: false),
                    nTasaMaxima = table.Column<decimal>(type: "decimal(7,4)", nullable: false),
                    dFecInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    dFecFin = table.Column<DateOnly>(type: "date", nullable: false),
                    dFecPublicacion = table.Column<DateOnly>(type: "date", nullable: false),
                    bEstado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TasaMaxi__650D6D6E667849EF", x => x.nCodTasaMax);
                    table.ForeignKey(
                        name: "FK__TasaMaxim__nCodS__57A801BA",
                        column: x => x.nCodSegmento,
                        principalTable: "CatSegmentoUsura",
                        principalColumn: "nCodSegmento");
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    IdMunicipio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDepartamento = table.Column<int>(type: "int", nullable: false),
                    cNombre = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Municipi__610059786D824024", x => x.IdMunicipio);
                    table.ForeignKey(
                        name: "FK_Municipios_Departamentos",
                        column: x => x.IdDepartamento,
                        principalTable: "Departamentos",
                        principalColumn: "IdDepartamento");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    IdUsuarioRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UsuarioR__6806BF4AC5289D17", x => x.IdUsuarioRol);
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Roles",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                    table.ForeignKey(
                        name: "FK_UsuarioRoles_Usuario",
                        column: x => x.IdUsuario,
                        principalTable: "UsuarioLogin",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_IdDepartamento",
                table: "Municipios",
                column: "IdDepartamento");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeAudits_Fecha",
                table: "PasswordChangeAudits",
                column: "FechaAttempt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeAudits_IdUsuario",
                table: "PasswordChangeAudits",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChangeAudits_Usuario",
                table: "PasswordChangeAudits",
                column: "Usuario");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__75E3EFCF90476A50",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TasaMaximaBCR_nCodSegmento",
                table: "TasaMaximaBCR",
                column: "nCodSegmento");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRoles_IdRol",
                table: "UsuarioRoles",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "UQ_UsuarioRoles",
                table: "UsuarioRoles",
                columns: new[] { "IdUsuario", "IdRol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agencias");

            migrationBuilder.DropTable(
                name: "CapacidadPago");

            migrationBuilder.DropTable(
                name: "CredLineaCredito");

            migrationBuilder.DropTable(
                name: "GarantiaFoto");

            migrationBuilder.DropTable(
                name: "LineaCatalogoAuxiliar");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "PasswordChangeAudits");

            migrationBuilder.DropTable(
                name: "SalarioMinimoVigente");

            migrationBuilder.DropTable(
                name: "TasaMaximaBCR");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "CatSegmentoUsura");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UsuarioLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Creditos_1_Tmp",
                table: "Creditos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CredCalendario",
                table: "CredCalendario");

            migrationBuilder.DropColumn(
                name: "cCelular",
                table: "Persona");

            migrationBuilder.DropColumn(
                name: "cTelefono",
                table: "Persona");

            migrationBuilder.DropColumn(
                name: "NGasto",
                table: "CredCalendario");

            migrationBuilder.DropColumn(
                name: "nCodAge",
                table: "CredCalendario");

            migrationBuilder.DropColumn(
                name: "nTotalCuota",
                table: "CredCalendario");

            migrationBuilder.RenameColumn(
                name: "IdCapacidadPago",
                table: "Creditos",
                newName: "nCiclo");

            migrationBuilder.RenameColumn(
                name: "nCodCred",
                table: "CredCalendario",
                newName: "nTipoCargo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CredCalendario",
                newName: "IdCalendario");

            migrationBuilder.AlterColumn<int>(
                name: "nCantidadVenta",
                table: "Ventas",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "nIdArticuloGarantia",
                table: "Garantia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "nIdFotoGarantia",
                table: "Garantia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "nValor",
                table: "Garantia",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "vFoto",
                table: "FotoNegocio",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "vFoto",
                table: "FotoDocumentacion",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCalendario",
                table: "Creditos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "nNroProxCuota",
                table: "Creditos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCalendario",
                table: "CredCalendCond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "nCodLineaSecundario",
                table: "CredCalendCond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "nGasto",
                table: "CredCalendCond",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "nTipoCargo",
                table: "CredCalendCond",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "dFecpro",
                table: "CredCalendario",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "nCantidadCompra",
                table: "Compras",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Creditos_1",
                table: "Creditos",
                column: "nCodCred");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredCalendario_1",
                table: "CredCalendario",
                column: "IdCalendario");
        }
    }
}
