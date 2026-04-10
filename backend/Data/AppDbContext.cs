using CoahuilaRelacion.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CoahuilaRelacion.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Registro> Registros => Set<Registro>();
    public DbSet<Declaracion> Declaraciones => Set<Declaracion>();
    public DbSet<DeclaracionCoordenada> DeclaracionCoordenadas => Set<DeclaracionCoordenada>();
    public DbSet<DeclaracionFecha> DeclaracionFechas => Set<DeclaracionFecha>();
    public DbSet<DeclaracionImagen> DeclaracionImagenes => Set<DeclaracionImagen>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Registro>()
            .HasMany(r => r.Declaraciones)
            .WithOne(d => d.Registro)
            .HasForeignKey(d => d.RegistroId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Coordenadas)
            .WithOne(c => c.Declaracion)
            .HasForeignKey(c => c.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Fechas)
            .WithOne(f => f.Declaracion)
            .HasForeignKey(f => f.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Imagenes)
            .WithOne(i => i.Declaracion)
            .HasForeignKey(i => i.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registro>()
            .HasIndex(r => r.Fecha);

        modelBuilder.Entity<Registro>()
            .HasIndex(r => r.Numero);

        modelBuilder.Entity<Declaracion>()
            .HasIndex(d => d.Orden);
    }
}
