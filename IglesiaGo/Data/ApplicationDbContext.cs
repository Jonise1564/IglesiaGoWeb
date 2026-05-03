using Microsoft.EntityFrameworkCore;
using IglesiaGo.Models; // Asegúrate de que este namespace coincida con tus modelos

namespace IglesiaGo.Data
{
    public class ApplicationDbContext : DbContext
    {
        // El constructor es necesario para recibir la configuración (cadena de conexión)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Cada DbSet representa una tabla en tu base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Enseñanza> Enseñanzas { get; set; }
        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Contacto> Contacto { get; set; }
       // public DbSet<IglesiaGo.Models.Contacto> Contacto { get; set; }

        // Opcional: Configuración adicional (Fluente API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ejemplo: Asegurar que el email de usuario sea único
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Si los nombres de tus tablas en MySQL son distintos a los nombres de las clases,
            // puedes mapearlos aquí. Por ejemplo:
            // modelBuilder.Entity<Enseñanza>().ToTable("ensenanzas");
        }
    }
}