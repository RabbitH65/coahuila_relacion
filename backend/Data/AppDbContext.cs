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
    public DbSet<Actor> Actores => Set<Actor>();
    public DbSet<Lugar> Lugares => Set<Lugar>();
    public DbSet<Circunstancia> Circunstancias => Set<Circunstancia>();
    public DbSet<DeclaracionActor> DeclaracionActores => Set<DeclaracionActor>();
    public DbSet<DeclaracionLugar> DeclaracionLugares => Set<DeclaracionLugar>();
    public DbSet<DeclaracionCircunstancia> DeclaracionCircunstancias => Set<DeclaracionCircunstancia>();

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

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Actores)
            .WithOne(a => a.Declaracion)
            .HasForeignKey(a => a.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Lugares)
            .WithOne(l => l.Declaracion)
            .HasForeignKey(l => l.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Declaracion>()
            .HasMany(d => d.Circunstancias)
            .WithOne(c => c.Declaracion)
            .HasForeignKey(c => c.DeclaracionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeclaracionActor>()
            .HasOne(a => a.Actor)
            .WithMany()
            .HasForeignKey(a => a.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeclaracionLugar>()
            .HasOne(l => l.Lugar)
            .WithMany()
            .HasForeignKey(l => l.LugarId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeclaracionCircunstancia>()
            .HasOne(c => c.Circunstancia)
            .WithMany()
            .HasForeignKey(c => c.CircunstanciaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Registro>()
            .HasIndex(r => r.Fecha);

        modelBuilder.Entity<Registro>()
            .HasIndex(r => r.Numero);

        modelBuilder.Entity<Declaracion>()
            .HasIndex(d => d.Orden);

        modelBuilder.Entity<Actor>()
            .HasIndex(a => a.NombreNormalizado);

        modelBuilder.Entity<Lugar>()
            .HasIndex(l => l.NombreNormalizado);

        modelBuilder.Entity<Circunstancia>()
            .HasIndex(c => c.NombreNormalizado);

        modelBuilder.Entity<DeclaracionActor>()
            .HasIndex(a => new { a.DeclaracionId, a.ActorId })
            .IsUnique();

        modelBuilder.Entity<DeclaracionLugar>()
            .HasIndex(l => new { l.DeclaracionId, l.LugarId })
            .IsUnique();

        modelBuilder.Entity<DeclaracionCircunstancia>()
            .HasIndex(c => new { c.DeclaracionId, c.CircunstanciaId })
            .IsUnique();
    }
}
