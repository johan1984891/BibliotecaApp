using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Models
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options) { }

        public DbSet<Libro>? Libros { get; set; }
        public DbSet<Prestamo>? Prestamos { get; set; }
        public DbSet<Reserva>? Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales del modelo si son necesarias
        }
    }
}