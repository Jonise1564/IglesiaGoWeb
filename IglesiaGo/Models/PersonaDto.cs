using System;
using System.ComponentModel.DataAnnotations;

namespace IglesiaGo.Models // Asegurate de que el namespace coincida con tu carpeta de Dtos/Models
{
    public class PersonaUpsertDto
    {
        [Required(ErrorMessage = "El documento de identidad es obligatorio")]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(20)]
        public string? TelefonoAlternativo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Genero { get; set; }

        [StringLength(30)]
        public string? EstadoCivil { get; set; }

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(100)]
        public string? Ciudad { get; set; }

        [StringLength(100)]
        public string? EstadoProvincia { get; set; }

        [StringLength(15)]
        public string? CodigoPostal { get; set; }

        [StringLength(100)]
        public string Pais { get; set; } = "Argentina";

        [Required]
        [StringLength(50)]
        public string TipoPersona { get; set; } = "Miembro";

        public int? UsuarioId { get; set; }
    }
}