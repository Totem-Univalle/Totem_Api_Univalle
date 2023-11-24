using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Totem_API.Models;

namespace Totem_API.Data;

public partial class TotemContext : DbContext
{
    public TotemContext()
    {
    }

    public TotemContext(DbContextOptions<TotemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Locacion> Locacions { get; set; }

    public virtual DbSet<Publicidad> Publicidads { get; set; }

    public virtual DbSet<Totem> Totems { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sqlserver,1433;Database=Totem;User Id=sa;Password=TotemUnivalle23;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Locacion>(entity =>
        {
            entity.HasKey(e => e.IdLocacion);

            entity.ToTable("Locacion");

            entity.Property(e => e.IdLocacion).HasColumnName("ID_locacion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdTotem).HasColumnName("id_totem");
            entity.Property(e => e.Keywords)
                .HasColumnType("text")
                .HasColumnName("keywords");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.UrlCarruselImagenes)
                .HasColumnType("text")
                .HasColumnName("urlCarruselImagenes");
            entity.Property(e => e.UrlMapa)
                .IsUnicode(false)
                .HasColumnName("urlMapa");

            entity.HasOne(d => d.IdTotemNavigation).WithMany(p => p.Locacions)
                .HasForeignKey(d => d.IdTotem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Locacion_Totem");
        });

        modelBuilder.Entity<Publicidad>(entity =>
        {
            entity.HasKey(e => e.IdPublicidad);

            entity.ToTable("Publicidad");

            entity.Property(e => e.IdPublicidad).HasColumnName("ID_publicidad");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.IdTotem).HasColumnName("id_totem");
            entity.Property(e => e.UrlPublicidad)
                .IsUnicode(false)
                .HasColumnName("urlPublicidad");

            entity.HasOne(d => d.IdTotemNavigation).WithMany(p => p.Publicidads)
                .HasForeignKey(d => d.IdTotem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Publicidad_Totem");
        });

        modelBuilder.Entity<Totem>(entity =>
        {
            entity.HasKey(e => e.IdTotem);

            entity.ToTable("Totem");

            entity.Property(e => e.IdTotem)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID_totem");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroPlantilla).HasColumnName("numeroPlantilla");
            entity.Property(e => e.UrlLogo)
                .IsUnicode(false)
                .HasColumnName("urlLogo");

            entity.HasOne(d => d.IdTotemNavigation).WithOne(p => p.Totem)
                .HasForeignKey<Totem>(d => d.IdTotem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Totem_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("ID_usuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(75)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.Email)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Rol).HasColumnName("rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
