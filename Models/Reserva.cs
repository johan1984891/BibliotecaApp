using System.ComponentModel.DataAnnotations;

namespace BibliotecaApp.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int LibroId { get; set; }
        public Libro? Libro { get; set; }

        [Required(ErrorMessage = "El nombre del solicitante es obligatorio")]
        public string? Solicitante { get; set; }

        [Required(ErrorMessage = "La fecha de reserva es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaReserva { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; } = DateTime.Today.AddDays(3);

        public string? Estado { get; set; } = "Activa"; // Activa, Completada, Cancelada, Vencida
    }
}