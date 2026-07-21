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
        public DbSet<Persona> Personas { get; set; } // <-- Mapeo del nuevo CRUD de Miembros
        public DbSet<Enseñanza> Enseñanzas { get; set; }
        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<MensajeConsulta> MensajesConsulta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Mapeo de tablas de Consultas (minúsculas y plural)
            modelBuilder.Entity<Consulta>().ToTable("consultas");
            modelBuilder.Entity<MensajeConsulta>().ToTable("mensajes_consulta");

            // 2. Mapeo de Noticias (plural)
            modelBuilder.Entity<Noticia>().ToTable("noticias"); 
            
            // 3. CORRECCIÓN DEFINITIVA PARA ENSEÑANZAS (plural con 'ñ')
            modelBuilder.Entity<Enseñanza>().ToTable("enseñanzas");

            // 4. MAPEO PARA LA TABLA PERSONAS
            modelBuilder.Entity<Persona>().ToTable("personas");

            // 5. Configuración de Usuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 6. Relación 1 a 1 de Personas con Usuarios (Garantiza un perfil único por cuenta)
            modelBuilder.Entity<Persona>()
                .HasIndex(p => p.UsuarioId)
                .IsUnique();

            // 7. Relación de hilos de mensajes
            modelBuilder.Entity<MensajeConsulta>()
                .HasOne(m => m.Consulta)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.ConsultaId);
        }
    }
}