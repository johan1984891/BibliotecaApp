using BibliotecaApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BibliotecaApp.Controllers
{
    public class ReservasController : Controller
    {
        private readonly BibliotecaContext _context;

        public ReservasController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas!
                .Include(r => r.Libro)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();
            return View(reservas);
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas!
                .Include(r => r.Libro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create(int? libroId)
        {
            var libros = _context.Libros!.ToList();

            ViewBag.Libros = new SelectList(libros, "Id", "Titulo", libroId);

            // Si se proporciona un libroId, pre-seleccionarlo
            if (libroId.HasValue)
            {
                var reserva = new Reserva
                {
                    LibroId = libroId.Value,
                    FechaReserva = DateTime.Today,
                    FechaVencimiento = DateTime.Today.AddDays(3)
                };
                return View(reserva);
            }

            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibroId,Solicitante,FechaReserva,FechaVencimiento")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el libro existe
                var libro = await _context.Libros!.FindAsync(reserva.LibroId);
                if (libro == null)
                {
                    ModelState.AddModelError("LibroId", "El libro seleccionado no existe.");
                    ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", reserva.LibroId);
                    return View(reserva);
                }

                reserva.Estado = "Activa";
                _context.Add(reserva);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reserva creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", reserva.LibroId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas!.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", reserva.LibroId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibroId,Solicitante,FechaReserva,FechaVencimiento,Estado")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Reserva actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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

            ViewBag.Libros = new SelectList(_context.Libros!.ToList(), "Id", "Titulo", reserva.LibroId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas!
                .Include(r => r.Libro)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas!.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reserva eliminada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Reservas/Cancelar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await _context.Reservas!.FindAsync(id);
            if (reserva != null)
            {
                reserva.Estado = "Cancelada";
                _context.Update(reserva);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reserva cancelada exitosamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Reservas/Completar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Completar(int id)
        {
            var reserva = await _context.Reservas!
                .Include(r => r.Libro)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva != null && reserva.Libro != null)
            {
                reserva.Estado = "Completada";

                // Si hay ejemplares disponibles, crear un préstamo automáticamente
                if (reserva.Libro.EjemplaresDisponibles > 0)
                {
                    var prestamo = new Prestamo
                    {
                        LibroId = reserva.LibroId,
                        Solicitante = reserva.Solicitante,
                        FechaPrestamo = DateTime.Today,
                        FechaDevolucionEstimada = DateTime.Today.AddDays(14),
                        Estado = "Activo"
                    };

                    reserva.Libro.EjemplaresDisponibles--;
                    _context.Prestamos!.Add(prestamo);

                    TempData["SuccessMessage"] = "Reserva completada y préstamo creado exitosamente.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Reserva completada, pero no hay ejemplares disponibles para préstamo.";
                }

                _context.Update(reserva);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas!.Any(e => e.Id == id);
        }
    }
}