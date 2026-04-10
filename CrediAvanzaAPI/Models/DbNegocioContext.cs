using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CrediAvanzaAPI.Models;

public partial class DbNegocioContext : DbContext
{
    public DbNegocioContext()
    {
    }

    public DbNegocioContext(DbContextOptions<DbNegocioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agencia> Agencias { get; set; }

    public virtual DbSet<CatalogoCodigo> CatalogoCodigos { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Conyuge> Conyuges { get; set; }

    public virtual DbSet<CredCalenGasto> CredCalenGastos { get; set; }

    public virtual DbSet<CredCalendCond> CredCalendConds { get; set; }

    public virtual DbSet<CredCalendario> CredCalendarios { get; set; }

    public virtual DbSet<CredCambioGasto> CredCambioGastos { get; set; }

    public virtual DbSet<CredFeriado> CredFeriados { get; set; }

    public virtual DbSet<CredFeriadoAge> CredFeriadoAges { get; set; }

    public virtual DbSet<CredGasto> CredGastos { get; set; }

    public virtual DbSet<CredLineaCredito> CredLineaCreditos { get; set; }

    public virtual DbSet<Credito> Creditos { get; set; }

    public virtual DbSet<Documentacion> Documentacions { get; set; }

    public virtual DbSet<Fiador> Fiadors { get; set; }

    public virtual DbSet<FotoDocumentacion> FotoDocumentacions { get; set; }

    public virtual DbSet<FotoId> FotoIds { get; set; }

    public virtual DbSet<FotoNegocio> FotoNegocios { get; set; }

    public virtual DbSet<Garantium> Garantia { get; set; }

    public virtual DbSet<LogErrore> LogErrores { get; set; }

    public virtual DbSet<Negocio> Negocios { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<VerNegocio> VerNegocios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-KTHL7K7\\SQLEXPRESS;Database=DbNegocio;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agencia>(entity =>
        {
            entity.HasKey(e => e.NCodAge).HasName("PK__Agencias__771BAD3ED57349AF");

            entity.Property(e => e.NCodAge)
                .ValueGeneratedNever()
                .HasColumnName("nCodAge");
            entity.Property(e => e.CCorreoElectronico)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cCorreoELectronico");
            entity.Property(e => e.CDirecAge)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("cDirecAge");
            entity.Property(e => e.CNomAge)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cNomAge");
            entity.Property(e => e.CTelefAge)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cTelefAge");
        });

        modelBuilder.Entity<CatalogoCodigo>(entity =>
        {
            entity.HasKey(e => new { e.NCodigo, e.NValor });

            entity.Property(e => e.NCodigo).HasColumnName("nCodigo");
            entity.Property(e => e.NValor).HasColumnName("nValor");
            entity.Property(e => e.CNomCod)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cNomCod");
            entity.Property(e => e.NEstados).HasColumnName("nEstados");
            entity.Property(e => e.NTipoCodigo).HasColumnName("nTipoCodigo");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK_CompraDetalle");

            entity.Property(e => e.CProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cProducto");
            entity.Property(e => e.NCantidadCompra).HasColumnName("nCantidadCompra");
            entity.Property(e => e.NPrecioTotal)
                .HasColumnType("money")
                .HasColumnName("nPrecioTotal");
            entity.Property(e => e.NPrecioXunidad)
                .HasColumnType("money")
                .HasColumnName("nPrecioXUnidad");
            entity.Property(e => e.NUnidadMedida).HasColumnName("nUnidadMedida");
        });

        modelBuilder.Entity<Conyuge>(entity =>
        {
            entity.HasKey(e => e.IdConyuge);

            entity.ToTable("Conyuge");

            entity.Property(e => e.CCelular)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cCelular");
            entity.Property(e => e.CDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cDocumento");
            entity.Property(e => e.CNombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cNombres");
            entity.Property(e => e.CPrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cPrimerApellido");
            entity.Property(e => e.CSegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cSegundoApellido");
            entity.Property(e => e.CTelefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cTelefono");
            entity.Property(e => e.NTipoDocumento).HasColumnName("nTipoDocumento");
        });

        modelBuilder.Entity<CredCalenGasto>(entity =>
        {
            entity.HasKey(e => e.IdCalenGasto);

            entity.Property(e => e.DFecAsig)
                .HasColumnType("datetime")
                .HasColumnName("dFecAsig");
            entity.Property(e => e.NCodAgePersAsig).HasColumnName("nCodAgePersAsig");
            entity.Property(e => e.NCodGasto).HasColumnName("nCodGasto");
            entity.Property(e => e.NMonto)
                .HasColumnType("money")
                .HasColumnName("nMonto");
            entity.Property(e => e.NMontoIgv)
                .HasColumnType("money")
                .HasColumnName("nMontoIGV");
            entity.Property(e => e.NMontoIgvpag)
                .HasColumnType("money")
                .HasColumnName("nMontoIGVPag");
            entity.Property(e => e.NMontoPag)
                .HasColumnType("money")
                .HasColumnName("nMontoPag");
            entity.Property(e => e.NMontoSinIgv)
                .HasColumnType("money")
                .HasColumnName("nMontoSinIGV");
            entity.Property(e => e.NMontoSinIgvpag)
                .HasColumnType("money")
                .HasColumnName("nMontoSinIGVPag");
            entity.Property(e => e.NNroCalen).HasColumnName("nNroCalen");
            entity.Property(e => e.NNroCuota).HasColumnName("nNroCuota");
        });

        modelBuilder.Entity<CredCalendCond>(entity =>
        {
            entity.HasKey(e => e.IdCredCalendCond);

            entity.ToTable("CredCalendCond");

            entity.Property(e => e.BCobroDom).HasColumnName("bCobroDom");
            entity.Property(e => e.BCobroFer).HasColumnName("bCobroFer");
            entity.Property(e => e.BCobroSab).HasColumnName("bCobroSab");
            entity.Property(e => e.BCuotaDoble).HasColumnName("bCuotaDoble");
            entity.Property(e => e.NCodLineaSecundario).HasColumnName("nCodLineaSecundario");
            entity.Property(e => e.NCuotas).HasColumnName("nCuotas");
            entity.Property(e => e.NDiaFijo).HasColumnName("nDiaFijo");
            entity.Property(e => e.NGasto)
                .HasColumnType("money")
                .HasColumnName("nGasto");
            entity.Property(e => e.NNroCalen).HasColumnName("nNroCalen");
            entity.Property(e => e.NPlazo).HasColumnName("nPlazo");
            entity.Property(e => e.NTipoCargo).HasColumnName("nTipoCargo");
        });

        modelBuilder.Entity<CredCalendario>(entity =>
        {
            entity.HasKey(e => e.IdCalendario).HasName("PK_CredCalendario_1");

            entity.ToTable("CredCalendario");

            entity.Property(e => e.DFecPago)
                .HasColumnType("datetime")
                .HasColumnName("dFecPago");
            entity.Property(e => e.DFecVenc)
                .HasColumnType("datetime")
                .HasColumnName("dFecVenc");
            entity.Property(e => e.DFecpro)
                .HasColumnType("datetime")
                .HasColumnName("dFecpro");
            entity.Property(e => e.NCapPag)
                .HasColumnType("money")
                .HasColumnName("nCapPag");
            entity.Property(e => e.NCapital)
                .HasColumnType("money")
                .HasColumnName("nCapital");
            entity.Property(e => e.NEstado).HasColumnName("nEstado");
            entity.Property(e => e.NIgv)
                .HasColumnType("money")
                .HasColumnName("nIgv");
            entity.Property(e => e.NIgvPag)
                .HasColumnType("money")
                .HasColumnName("nIgvPag");
            entity.Property(e => e.NIntComp)
                .HasColumnType("money")
                .HasColumnName("nIntComp");
            entity.Property(e => e.NIntMor)
                .HasColumnType("money")
                .HasColumnName("nIntMor");
            entity.Property(e => e.NIntMorPag)
                .HasColumnType("money")
                .HasColumnName("nIntMorPag");
            entity.Property(e => e.NIntPag)
                .HasColumnType("money")
                .HasColumnName("nIntPag");
            entity.Property(e => e.NNroCalen).HasColumnName("nNroCalen");
            entity.Property(e => e.NNroCuota).HasColumnName("nNroCuota");
            entity.Property(e => e.NTipoCargo).HasColumnName("nTipoCargo");
        });

        modelBuilder.Entity<CredCambioGasto>(entity =>
        {
            entity.HasKey(e => e.NIdCambio);

            entity.ToTable("CredCambioGasto");

            entity.Property(e => e.NIdCambio).HasColumnName("nIdCambio");
            entity.Property(e => e.DFechaCambio).HasColumnName("dFechaCambio");
            entity.Property(e => e.NCodCred).HasColumnName("nCodCred");
            entity.Property(e => e.NMontoNuevo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nMontoNuevo");
            entity.Property(e => e.NMontoOriginal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nMontoOriginal");
        });

        modelBuilder.Entity<CredFeriado>(entity =>
        {
            entity.HasKey(e => e.NIdFeriado);

            entity.ToTable("CredFeriado");

            entity.Property(e => e.NIdFeriado).HasColumnName("nIdFeriado");
            entity.Property(e => e.BEstado).HasColumnName("bEstado");
            entity.Property(e => e.CDescripcion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("cDescripcion");
            entity.Property(e => e.DFecha)
                .HasColumnType("datetime")
                .HasColumnName("dFecha");
        });

        modelBuilder.Entity<CredFeriadoAge>(entity =>
        {
            entity.HasKey(e => e.IdCredFeriadoAge);

            entity.ToTable("CredFeriadoAge");

            entity.Property(e => e.DFecha)
                .HasColumnType("datetime")
                .HasColumnName("dFecha");
            entity.Property(e => e.NCodAge).HasColumnName("nCodAge");
            entity.Property(e => e.NIdFeriado).HasColumnName("nIdFeriado");
        });

        modelBuilder.Entity<CredGasto>(entity =>
        {
            entity.HasKey(e => e.IdGasto);

            entity.Property(e => e.CDescripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cDescripcion");
            entity.Property(e => e.NPeriodo).HasColumnName("nPeriodo");
            entity.Property(e => e.NProd).HasColumnName("nProd");
            entity.Property(e => e.NRangoFinal)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nRangoFinal");
            entity.Property(e => e.NRangoInicial)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("nRangoInicial");
            entity.Property(e => e.NSubProd).HasColumnName("nSubProd");
            entity.Property(e => e.NTipoCargo).HasColumnName("nTipoCargo");
            entity.Property(e => e.NTipoGasto).HasColumnName("nTipoGasto");
            entity.Property(e => e.NValor)
                .HasColumnType("money")
                .HasColumnName("nValor");
        });

        modelBuilder.Entity<CredLineaCredito>(entity =>
        {
            entity.HasKey(e => e.NCodLinea).HasName("PK__CredLine__50A988C3ABA5478A");

            entity.ToTable("CredLineaCredito");

            entity.Property(e => e.NCodLinea).HasColumnName("nCodLinea");
            entity.Property(e => e.BEstado)
                .HasDefaultValue(true)
                .HasColumnName("bEstado");
            entity.Property(e => e.CDescLinea)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("cDescLinea");
            entity.Property(e => e.NMontoMax)
                .HasColumnType("money")
                .HasColumnName("nMontoMax");
            entity.Property(e => e.NMontoMin)
                .HasColumnType("money")
                .HasColumnName("nMontoMin");
            entity.Property(e => e.NPlazoMax).HasColumnName("nPlazoMax");
            entity.Property(e => e.NPlazoMin).HasColumnName("nPlazoMin");
            entity.Property(e => e.NProd).HasColumnName("nProd");
            entity.Property(e => e.NSubProd).HasColumnName("nSubProd");
            entity.Property(e => e.NTasaCom)
                .HasColumnType("money")
                .HasColumnName("nTasaCom");
        });

        modelBuilder.Entity<Credito>(entity =>
        {
            entity.HasKey(e => e.NCodCred).HasName("PK_Creditos_1");

            entity.Property(e => e.NCodCred).HasColumnName("nCodCred");
            entity.Property(e => e.DFecVig)
                .HasColumnType("datetime")
                .HasColumnName("dFecVig");
            entity.Property(e => e.NAceptaTerminos).HasColumnName("nAceptaTerminos");
            entity.Property(e => e.NCiclo).HasColumnName("nCiclo");
            entity.Property(e => e.NCobroEnAgencia).HasColumnName("nCobroEnAgencia");
            entity.Property(e => e.NCodAge).HasColumnName("nCodAge");
            entity.Property(e => e.NCodLinea).HasColumnName("nCodLinea");
            entity.Property(e => e.NDiasAtraso).HasColumnName("nDiasAtraso");
            entity.Property(e => e.NEstado).HasColumnName("nEstado");
            entity.Property(e => e.NMontoCuota)
                .HasColumnType("money")
                .HasColumnName("nMontoCuota");
            entity.Property(e => e.NMora)
                .HasColumnType("money")
                .HasColumnName("nMora");
            entity.Property(e => e.NNroCuotas).HasColumnName("nNroCuotas");
            entity.Property(e => e.NNroProxCuota).HasColumnName("nNroProxCuota");
            entity.Property(e => e.NPeriodo).HasColumnName("nPeriodo");
            entity.Property(e => e.NPrestamo)
                .HasColumnType("money")
                .HasColumnName("nPrestamo");
            entity.Property(e => e.NProd).HasColumnName("nProd");
            entity.Property(e => e.NSaldoK)
                .HasColumnType("money")
                .HasColumnName("nSaldoK");
            entity.Property(e => e.NSubProd).HasColumnName("nSubProd");
            entity.Property(e => e.NTasaComision)
                .HasColumnType("money")
                .HasColumnName("nTasaComision");
            entity.Property(e => e.NTasaComp)
                .HasColumnType("money")
                .HasColumnName("nTasaComp");
            entity.Property(e => e.NTasaMor)
                .HasColumnType("money")
                .HasColumnName("nTasaMor");
        });

        modelBuilder.Entity<Documentacion>(entity =>
        {
            entity.HasKey(e => e.IdDocumentacion);

            entity.ToTable("Documentacion");
        });

        modelBuilder.Entity<Fiador>(entity =>
        {
            entity.HasKey(e => e.IdFiador);

            entity.ToTable("Fiador");

            entity.Property(e => e.CCelular)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cCelular");
            entity.Property(e => e.CDireccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cDireccion");
            entity.Property(e => e.CDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cDocumento");
            entity.Property(e => e.CNombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cNombres");
            entity.Property(e => e.CPrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cPrimerApellido");
            entity.Property(e => e.CSegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cSegundoApellido");
            entity.Property(e => e.CTelefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cTelefono");
            entity.Property(e => e.NTipoDocumento).HasColumnName("nTipoDocumento");
        });

        modelBuilder.Entity<FotoDocumentacion>(entity =>
        {
            entity.HasKey(e => e.IdFoto).HasName("PK_FotoDocumentacion_1");

            entity.ToTable("FotoDocumentacion");

            entity.Property(e => e.VFoto)
                .IsUnicode(false)
                .HasColumnName("vFoto");
        });

        modelBuilder.Entity<FotoId>(entity =>
        {
            entity.HasKey(e => e.IdFoto).HasName("PK_Foto");

            entity.ToTable("FotoID");

            entity.Property(e => e.NTipoFoto).HasColumnName("nTipoFoto");
            entity.Property(e => e.VFoto)
                .IsUnicode(false)
                .HasColumnName("vFoto");
        });

        modelBuilder.Entity<FotoNegocio>(entity =>
        {
            entity.HasKey(e => e.IdFoto);

            entity.ToTable("FotoNegocio");

            entity.Property(e => e.NTipoFoto).HasColumnName("nTipoFoto");
            entity.Property(e => e.VFoto)
                .IsUnicode(false)
                .HasColumnName("vFoto");
        });

        modelBuilder.Entity<Garantium>(entity =>
        {
            entity.HasKey(e => e.IdGarantia);

            entity.Property(e => e.NIdArticuloGarantia).HasColumnName("nIdArticuloGarantia");
            entity.Property(e => e.NIdFotoGarantia).HasColumnName("nIdFotoGarantia");
            entity.Property(e => e.NValor)
                .HasColumnType("money")
                .HasColumnName("nValor");
        });

        modelBuilder.Entity<LogErrore>(entity =>
        {
            entity.HasKey(e => e.IdLogError).HasName("PK__LogError__7B1F940EA93C6869");

            entity.Property(e => e.FechaError)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Ip)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("IP");
            entity.Property(e => e.Origen)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TipoExcepcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Usuario)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Negocio>(entity =>
        {
            entity.HasKey(e => e.IdNegocio);

            entity.ToTable("Negocio");

            entity.Property(e => e.CDireccion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cDireccion");
            entity.Property(e => e.CGeolocalizacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cGeolocalizacion");
            entity.Property(e => e.CNombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cNombre");
            entity.Property(e => e.CSector).HasColumnName("cSector");
            entity.Property(e => e.CTelefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cTelefono");
            entity.Property(e => e.THoraCierre).HasColumnName("tHoraCierre");
            entity.Property(e => e.THoraInicio).HasColumnName("tHoraInicio");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona);

            entity.ToTable("Persona");

            entity.Property(e => e.CCorreo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cCorreo");
            entity.Property(e => e.CDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cDocumento");
            entity.Property(e => e.CNombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cNombres");
            entity.Property(e => e.CPrimerApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cPrimerApellido");
            entity.Property(e => e.CSegundoApellido)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cSegundoApellido");
            entity.Property(e => e.DFechaExpedicion).HasColumnName("dFechaExpedicion");
            entity.Property(e => e.DFechaNacimiento).HasColumnName("dFechaNacimiento");
            entity.Property(e => e.DFechaVencimiento).HasColumnName("dFechaVencimiento");
            entity.Property(e => e.NDepartamentoDoc).HasColumnName("nDepartamentoDoc");
            entity.Property(e => e.NDepartamentoNacimiento).HasColumnName("nDepartamentoNacimiento");
            entity.Property(e => e.NEscolaridad).HasColumnName("nEscolaridad");
            entity.Property(e => e.NEstadoCivil).HasColumnName("nEstadoCivil");
            entity.Property(e => e.NMunicipioDoc).HasColumnName("nMunicipioDoc");
            entity.Property(e => e.NMunicipioNacimiento).HasColumnName("nMunicipioNacimiento");
            entity.Property(e => e.NNacionalidad).HasColumnName("nNacionalidad");
            entity.Property(e => e.NProfesion).HasColumnName("nProfesion");
            entity.Property(e => e.NSexo).HasColumnName("nSexo");
            entity.Property(e => e.NTipoDocumento).HasColumnName("nTipoDocumento");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta);

            entity.Property(e => e.CProducto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cProducto");
            entity.Property(e => e.NCantidadVenta).HasColumnName("nCantidadVenta");
            entity.Property(e => e.NPrecioTotal)
                .HasColumnType("money")
                .HasColumnName("nPrecioTotal");
            entity.Property(e => e.NPrecioXunidad)
                .HasColumnType("money")
                .HasColumnName("nPrecioXUnidad");
            entity.Property(e => e.NUnidadMedida).HasColumnName("nUnidadMedida");
        });

        modelBuilder.Entity<VerNegocio>(entity =>
        {
            entity.HasKey(e => e.NCodVar);

            entity.ToTable("VerNegocio");

            entity.Property(e => e.NCodVar).HasColumnName("nCodVar");
            entity.Property(e => e.CNomVar)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("cNomVar");
            entity.Property(e => e.CValorVar)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cValorVar");
            entity.Property(e => e.NTipoVar).HasColumnName("nTipoVar");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
