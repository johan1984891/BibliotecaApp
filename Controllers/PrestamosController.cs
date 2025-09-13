using BibliotecaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BibliotecaApp.Controllers
{
    public class PrestamosController : Controller
    {
        private readonly BibliotecaContext _context;

        public PrestamosController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Prestamos
        public async Task<IActionResult> Index(string estadoFiltro)
        {
            var prestamos = _context.Prestamos!
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaPrestamo)
                .AsQueryable();

            // Filtrar por estado si se especifica
            if (!string.IsNullOrEmpty(estadoFiltro) && estadoFiltro != "Todos")
            {
                prestamos = prestamos.Where(p => p.Estado == estadoFiltro);
            }

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "Todos", Text = "Todos los estados" },
                new SelectListItem { Value = "Activo", Text = "Activos" },
                new SelectListItem { Value = "Devuelto", Text = "Devueltos" },
                new SelectListItem { Value = "Atrasado", Text = "Atrasados" }
            };

            ViewBag.EstadoSeleccionado = estadoFiltro ?? "Todos";

            return View(await prestamos.ToListAsync());
        }

        // GET: Prestamos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos!
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return View(prestamo);
        }

        // GET: Prestamos/Create
        public IActionResult Create(int? libroId)
        {
            var librosDisponibles = _context.Libros!.Where(l => l.EjemplaresDisponibles > 0).ToList();

            ViewBag.Libros = new SelectList(librosDisponibles, "Id", "Titulo", libroId);

            // Si se proporciona un libroId, pre-seleccionarlo
            if (libroId.HasValue)
            {
                var prestamo = new Prestamo
                {
                    LibroId = libroId.Value,
                    FechaPrestamo = DateTime.Today,
                    FechaDevolucionEstimada = DateTime.Today.AddDays(14)
                };
                return View(prestamo);
            }

            return View();
        }

        // POST: Prestamos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroId,Solicitante,FechaPrestamo,FechaDevolucionEstimada")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el libro existe y tiene disponibilidad
                var libro = await _context.Libros!.FindAsync(prestamo.LibroId);
                if (libro == null)
                {
                    ModelState.AddModelError("LibroId", "El libro seleccionado no existe.");
                    ViewBag.Libros = new SelectList(_context.Libros!.Where(l => l.EjemplaresDisponibles > 0).ToList(), "Id", "Titulo", prestamo.LibroId);
                    return View(prestamo);
                }

                if (libro.EjemplaresDisponibles <= 0)
                {
                    ModelState.AddModelError("", "No hay ejemplares disponibles de este libro.");
                    ViewBag.Libros = new SelectList(_context.Libros!.Where(l => l.EjemplaresDisponibles > 0).ToList(), "Id", "Titulo", prestamo.LibroId);
                    return View(prestamo);
                }

                // Actualizar disponibilidad del libro
                libro.EjemplaresDisponibles--;
                _context.Update(libro);

                prestamo.Estado = "Activo";
                _context.Add(prestamo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Préstamo registrado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Libros = new SelectList(_context.Libros!.Where(l => l.EjemplaresDisponibles > 0).ToList(), "Id", "Titulo", prestamo.LibroId);
            return View(prestamo);
        }

        // GET: Prestamos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos!.FindAsync(id);
            if (prestamo == null)
            {
                return NotFound();
            }

            ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", prestamo.LibroId);
            ViewBag.Estados = new SelectList(new[] { "Activo", "Devuelto", "Atrasado" }, prestamo.Estado);

            return View(prestamo);
        }

        // POST: Prestamos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibroId,Solicitante,FechaPrestamo,FechaDevolucionEstimada,FechaDevolucionReal,Estado")] Prestamo prestamo)
        {
            if (id != prestamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica especial para cuando se marca como devuelto
                    if (prestamo.Estado == "Devuelto" && prestamo.FechaDevolucionReal == null)
                    {
                        prestamo.FechaDevolucionReal = DateTime.Today;

                        // Actualizar disponibilidad del libro si es una devolución
                        var libro = await _context.Libros!.FindAsync(prestamo.LibroId);
                        if (libro != null)
                        {
                            libro.EjemplaresDisponibles++;
                            _context.Update(libro);
                        }
                    }

                    _context.Update(prestamo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Préstamo actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrestamoExists(prestamo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", prestamo.LibroId);
            ViewBag.Estados = new SelectList(new[] { "Activo", "Devuelto", "Atrasado" }, prestamo.Estado);

            return View(prestamo);
        }

        // GET: Prestamos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prestamo = await _context.Prestamos!
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return View(prestamo);
        }

        // POST: Prestamos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prestamo = await _context.Prestamos!
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo != null)
            {
                // Si el préstamo está activo, devolver el libro al inventario
                if (prestamo.Estado == "Activo" && prestamo.Libro != null)
                {
                    prestamo.Libro.EjemplaresDisponibles++;
                    _context.Update(prestamo.Libro);
                }

                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Préstamo eliminado exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Prestamos/Devolver/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            var prestamo = await _context.Prestamos!
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo != null && prestamo.Libro != null)
            {
                prestamo.FechaDevolucionReal = DateTime.Today;
                prestamo.Estado = "Devuelto";

                // Actualizar disponibilidad del libro
                prestamo.Libro.EjemplaresDisponibles++;
                _context.Update(prestamo.Libro);
                _context.Update(prestamo);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Libro devuelto exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Prestamos/MarcarAtrasado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarAtrasado(int id)
        {
            var prestamo = await _context.Prestamos!.FindAsync(id);
            if (prestamo != null)
            {
                prestamo.Estado = "Atrasado";
                _context.Update(prestamo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Préstamo marcado como atrasado.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Prestamos/ReporteAtrasados
        public async Task<IActionResult> ReporteAtrasados()
        {
            var prestamosAtrasados = await _context.Prestamos!
                .Include(p => p.Libro)
                .Where(p => p.Estado == "Atrasado" ||
                           (p.Estado == "Activo" && p.FechaDevolucionEstimada < DateTime.Today))
                .OrderBy(p => p.FechaDevolucionEstimada)
                .ToListAsync();

            // Actualizar estado de préstamos atrasados
            foreach (var prestamo in prestamosAtrasados.Where(p => p.Estado == "Activo"))
            {
                prestamo.Estado = "Atrasado";
                _context.Update(prestamo);
            }
            await _context.SaveChangesAsync();

            return View(prestamosAtrasados);
        }

        private bool PrestamoExists(int id)
        {
            return _context.Prestamos!.Any(e => e.Id == id);
        }
    }
}