using System.ComponentModel.DataAnnotations;

namespace IglesiaGo.Models
{
    public class Contacto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [RegularExpression(@"^[a-zA-Z\sñÑáéíóúÁÉÍÓÚ]+$", ErrorMessage = "Solo se permiten letras")]
        public string NombreRemitente { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string mail { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(15, ErrorMessage = "El teléfono es demasiado largo")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El mensaje no puede estar vacío")]
        [StringLength(500, ErrorMessage = "El mensaje no puede superar los 500 caracteres")]
        public string Mensaje { get; set; }

        public DateTime FechaEnvio { get; set; }
    }
}