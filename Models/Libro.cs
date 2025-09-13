using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [DisplayName("Título")]
        [Required(ErrorMessage = "El título es obligatorio")]
        public string? Titulo { get; set; }

        [DisplayName("Autor")]
        [Required(ErrorMessage = "El autor es obligatorio")]
        public string? Autor { get; set; }

        [DisplayName("ISBN")]
        public string? ISBN { get; set; }

        [DisplayName("Año de Publicación")]
        [Required(ErrorMessage = "El año de publicación es obligatorio")]
        [Range(1000, 2024, ErrorMessage = "El año debe ser entre 1000 y 2024")]
        public int AnioPublicacion { get; set; } = DateTime.Now.Year;

        [DisplayName("Editorial")]
        public string? Editorial { get; set; }

        [DisplayName("Género")]
        public string? Genero { get; set; }

        [DisplayName("Ejemplares Disponibles")]
        public int EjemplaresDisponibles { get; set; }

        [DisplayName("URL de Imagen")]
        public string? ImagenUrl { get; set; } = "/img/libro-default.jpg";

        [DisplayName("Descripción")]
        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<Prestamo>? Prestamos { get; set; }
        public ICollection<Reserva>? Reservas { get; set; }
    }
}