using Microsoft.EntityFrameworkCore;
using ControlCalidadProduccion.Models;

namespace ControlCalidadProduccion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Medicion
            modelBuilder.Entity<Medicion>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Grasa).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Acidez).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Proteina).HasColumnType("decimal(5,2)");
                entity.Property(e => e.PH).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Humedad).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Lote)
                      .WithMany()
                      .HasForeignKey(e => e.LoteId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => e.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Lote
            modelBuilder.Entity<Lote>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Fecha).HasColumnType("datetime");
                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
                entity.Property(e => e.Observaciones).HasMaxLength(500);
            });

            // Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.NormaGrasa).HasColumnType("decimal(5,2)");
                entity.Property(e => e.NormaAcidez).HasColumnType("decimal(5,2)");
                entity.Property(e => e.NormaProteina).HasColumnType("decimal(5,2)");
                entity.Property(e => e.NormaPH).HasColumnType("decimal(5,2)");
                entity.Property(e => e.NormaHumedad).HasColumnType("decimal(5,2)");
            });
        }
    }
}
