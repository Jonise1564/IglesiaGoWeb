using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGo.Models
{
    [Table("mensajes_consulta")]
    public class MensajeConsulta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConsultaId { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoRemitente { get; set; } // "Admin" o "Usuario"

        [Required]
        public string CuerpoMensaje { get; set; }

        public DateTime? FechaEnvio { get; set; }

        [ForeignKey("ConsultaId")]
        public virtual Consulta? Consulta { get; set; }
    }
}