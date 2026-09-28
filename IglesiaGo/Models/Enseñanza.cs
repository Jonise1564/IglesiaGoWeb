using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGo.Models
{
    public class Enseñanza
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

     
        [NotMapped]
        public string? PasajeClave { get; set; }

        public string Contenido { get; set; } = string.Empty;

        public string VideoUrl { get; set; } = string.Empty;

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    }
}