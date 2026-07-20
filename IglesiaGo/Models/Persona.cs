using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGo.Models
{
    [Table("personas")]
    public class Persona
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("DocumentoIdentidad")]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        [Required]
        [Column("Nombres")]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [Column("Apellidos")]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Column("Email")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Column("Telefono")]
        [StringLength(20)]
        public string? Telefono { get; set; }

        [Column("TelefonoAlternativo")]
        [StringLength(20)]
        public string? TelefonoAlternativo { get; set; }

        [Required]
        [Column("FechaNacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("Genero")]
        [StringLength(20)]
        public string? Genero { get; set; }

        [Column("EstadoCivil")]
        [StringLength(30)]
        public string? EstadoCivil { get; set; }

        [Column("Direccion")]
        [StringLength(255)]
        public string? Direccion { get; set; }

        [Column("Ciudad")]
        [StringLength(100)]
        public string? Ciudad { get; set; }

        [Column("EstadoProvincia")]
        [StringLength(100)]
        public string? EstadoProvincia { get; set; }

        [Column("CodigoPostal")]
        [StringLength(15)]
        public string? CodigoPostal { get; set; }

        [Column("Pais")]
        [StringLength(100)]
        public string Pais { get; set; } = "Argentina";

        [Required]
        [Column("TipoPersona")]
        [StringLength(50)]
        public string TipoPersona { get; set; } = "Miembro";

        [Column("Activo")]
        public sbyte? Activo { get; set; } = 1; // tinyint(1) en MySQL mapea mejor como sbyte o bool

        [Column("UsuarioId")]
        public int? UsuarioId { get; set; }

        [Column("CreadoEl")]
        public DateTime? CreadoEl { get; set; }

        [Column("ActualizadoEl")]
        public DateTime? ActualizadoEl { get; set; }
    }
}