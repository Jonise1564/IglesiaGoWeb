using System.ComponentModel.DataAnnotations;

namespace IglesiaGo.Models
{
    public class Noticia
    {
        public int Id { get; set; }
        public string ImagenPortada { get; set; }
        public string Cuerpo { get; set; }
        public DateTime Fecha { get; set; }
    }
}