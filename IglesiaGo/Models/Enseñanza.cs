using System.ComponentModel.DataAnnotations;

namespace IglesiaGo.Models
{
    public class Enseñanza
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string VideoUrl { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}