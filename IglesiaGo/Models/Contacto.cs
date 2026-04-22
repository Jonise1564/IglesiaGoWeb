using System.ComponentModel.DataAnnotations;

namespace IglesiaGo.Models
{
    public class Contacto
    {
        public int Id { get; set; }
        public string NombreRemitente { get; set; }
        public string Telefono { get; set; }
        public string mail { get; set; } // Ojo: lo llamamos 'mail' para que coincida con tu SQL
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}