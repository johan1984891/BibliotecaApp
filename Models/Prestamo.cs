using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class Prestamo
    {
        public int Id { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }

        [Required(ErrorMessage = "El nombre del solicitante es obligatorio")]
        public string? Solicitante { get; set; }

        [Required(ErrorMessage = "La fecha de préstamo es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaPrestamo { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de devolución estimada es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaDevolucionEstimada { get; set; } = DateTime.Today.AddDays(14);

        [DataType(DataType.Date)]
        public DateTime? FechaDevolucionReal { get; set; }

        public string? Estado { get; set; } = "Activo"; // Activo, Devuelto, Atrasado
    }
}