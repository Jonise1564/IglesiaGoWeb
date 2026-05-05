using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGo.Models
{
    [Table("consultas")]
    public class Consulta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("NombreRemitente")] // Mapeo explícito
        public string NombreRemitente { get; set; }

        [Column("Telefono")]
        public string? Telefono { get; set; }

        [Required]
        [EmailAddress]
        [Column("mail")] // En la DB se llama 'mail'
        public string mail { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public bool Atendido { get; set; }

        // Relación con los mensajes del hilo
        public virtual ICollection<MensajeConsulta> Mensajes { get; set; } = new List<MensajeConsulta>();
    }
}