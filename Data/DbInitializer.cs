using BibliotecaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BibliotecaContext context)
        {
            context.Database.EnsureCreated();

            // Verificar si ya existen libros
            if (context.Libros!.Any())
            {
                return; // La BD ya ha sido inicializada
            }

            var libros = new Libro[]  
            {

            };

            foreach (var libro in libros)  
            {
                context.Libros!.Add(libro);
            }
            context.SaveChanges();  
        }
    }
}