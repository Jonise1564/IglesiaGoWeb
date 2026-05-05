using Microsoft.EntityFrameworkCore;
using IglesiaGo.Models; 

namespace IglesiaGo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Enseñanza> Enseñanzas { get; set; }
        public DbSet<Noticia> Noticias { get; set; }
        
        // Nuevos DbSets para el sistema de consultas y mensajes
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<MensajeConsulta> MensajesConsulta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Mapeo de nombres exactos para MySQL (minúsculas y plurales según tu DB)
            modelBuilder.Entity<Consulta>().ToTable("consultas");
            modelBuilder.Entity<MensajeConsulta>().ToTable("mensajes_consulta");

            // 2. Configuración de Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 3. Mapeo para otras tablas existentes
            modelBuilder.Entity<Enseñanza>().ToTable("Enseñanza");
            modelBuilder.Entity<Noticia>().ToTable("Noticia");

            // 4. Relación Uno a Muchos (Consulta -> MensajesConsulta)
            modelBuilder.Entity<MensajeConsulta>()
                .HasOne(m => m.Consulta)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.ConsultaId);
        }
    }
}