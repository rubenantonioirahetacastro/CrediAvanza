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

    public virtual DbSet<CatalogoCodigo> CatalogoCodigos { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<CredCalenGasto> CredCalenGastos { get; set; }

    public virtual DbSet<CredCalendCond> CredCalendConds { get; set; }

    public virtual DbSet<CredCalendario> CredCalendarios { get; set; }

    public virtual DbSet<Credito> Creditos { get; set; }

    public virtual DbSet<Documentacion> Documentacions { get; set; }

    public virtual DbSet<Fiador> Fiadors { get; set; }

    public virtual DbSet<Foto> Fotos { get; set; }

    public virtual DbSet<Garantium> Garantia { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=crediavanzasv.database.windows.net;Database=DbNegocio;User Id=CloudAdmin;Password=Pepe2024;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogoCodigo>(entity =>
        {
            entity.HasKey(e => new { e.NCodigo, e.NValor });

            entity.Property(e => e.NCodigo).HasColumnName("nCodigo");
            entity.Property(e => e.NValor)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nValor");
            entity.Property(e => e.CNomCod)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cNomCod");
            entity.Property(e => e.NEstados).HasColumnName("nEstados");
            entity.Property(e => e.NTipoCodigo).HasColumnName("nTipoCodigo");
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra);

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

        modelBuilder.Entity<Credito>(entity =>
        {
            entity.HasKey(e => new { e.NCodCred, e.NCodAge });

            entity.Property(e => e.NCodCred).HasColumnName("nCodCred");
            entity.Property(e => e.NCodAge).HasColumnName("nCodAge");
            entity.Property(e => e.DFecVig)
                .HasColumnType("datetime")
                .HasColumnName("dFecVig");
            entity.Property(e => e.NCiclo).HasColumnName("nCiclo");
            entity.Property(e => e.NCobroEnAgencia).HasColumnName("nCobroEnAgencia");
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
            entity.HasKey(e => e.IdDocumento);

            entity.ToTable("Documentacion");

            entity.Property(e => e.NTipoDocumento).HasColumnName("nTipoDocumento");
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

        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.IdFoto);

            entity.ToTable("Foto");

            entity.Property(e => e.IdFoto).ValueGeneratedNever();
            entity.Property(e => e.NTipoFoto)
                .ValueGeneratedOnAdd()
                .HasColumnName("nTipoFoto");
            entity.Property(e => e.VFoto)
                .IsUnicode(false)
                .HasColumnName("vFoto");
        });

        modelBuilder.Entity<Garantium>(entity =>
        {
            entity.HasKey(e => e.IdGarantia);

            entity.Property(e => e.IdGarantia).ValueGeneratedNever();
            entity.Property(e => e.NIdArticuloGarantia).HasColumnName("nIdArticuloGarantia");
            entity.Property(e => e.NIdFotoGarantia)
                .ValueGeneratedOnAdd()
                .HasColumnName("nIdFotoGarantia");
            entity.Property(e => e.NValor)
                .HasColumnType("money")
                .HasColumnName("nValor");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona);

            entity.ToTable("Persona");

            entity.Property(e => e.IdPersona).ValueGeneratedNever();
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
