using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaWEB.Models;

namespace sistemaWEB.Controllers
{
    public class ReservaHotelsController : Controller
    {
        private readonly MiContexto _context;

        public ReservaHotelsController(MiContexto context)
        {
            _context = context;
        }

        // GET: ReservaHotels
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.reservaHoteles.Include(r => r.miHotel).Include(r => r.miUsuario);
            return View(await miContexto.ToListAsync());
        }

        // GET: ReservaHotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservaHoteles == null)
            {
                return NotFound();
            }

            var reservaHotel = await _context.reservaHoteles
                .Include(r => r.miHotel)
                .Include(r => r.miUsuario)
                .FirstOrDefaultAsync(m => m.idReservaHotel == id);
            if (reservaHotel == null)
            {
                return NotFound();
            }

            return View(reservaHotel);
        }

        // GET: ReservaHotels/Create
        public IActionResult Create()
        {
            ViewData["idHotel"] = new SelectList(_context.hoteles, "id", "nombre");
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni");
            return View();
        }

        // POST: ReservaHotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idReservaHotel,idHotel,cantidadPersonas,idUsuario,fechaDesde,fechaHasta,pagado")] ReservaHotel reservaHotel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservaHotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idHotel"] = new SelectList(_context.hoteles, "id", "nombre", reservaHotel.idHotel);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaHotel.idUsuario);
            return View(reservaHotel);
        }

        // GET: ReservaHotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reservaHoteles == null)
            {
                return NotFound();
            }

            var reservaHotel = await _context.reservaHoteles.FindAsync(id);
            if (reservaHotel == null)
            {
                return NotFound();
            }
            ViewData["idHotel"] = new SelectList(_context.hoteles, "id", "nombre", reservaHotel.idHotel);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaHotel.idUsuario);
            return View(reservaHotel);
        }

        // POST: ReservaHotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idReservaHotel,idHotel,cantidadPersonas,idUsuario,fechaDesde,fechaHasta,pagado")] ReservaHotel reservaHotel)
        {
            if (id != reservaHotel.idReservaHotel)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservaHotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaHotelExists(reservaHotel.idReservaHotel))
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
            ViewData["idHotel"] = new SelectList(_context.hoteles, "id", "nombre", reservaHotel.idHotel);
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaHotel.idUsuario);
            return View(reservaHotel);
        }

        // GET: ReservaHotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservaHoteles == null)
            {
                return NotFound();
            }

            var reservaHotel = await _context.reservaHoteles
                .Include(r => r.miHotel)
                .Include(r => r.miUsuario)
                .FirstOrDefaultAsync(m => m.idReservaHotel == id);
            if (reservaHotel == null)
            {
                return NotFound();
            }

            return View(reservaHotel);
        }

        // POST: ReservaHotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reservaHoteles == null)
            {
                return Problem("Entity set 'MiContexto.reservaHoteles'  is null.");
            }
            var reservaHotel = await _context.reservaHoteles.FindAsync(id);
            if (reservaHotel != null)
            {
                _context.reservaHoteles.Remove(reservaHotel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaHotelExists(int id)
        {
          return (_context.reservaHoteles?.Any(e => e.idReservaHotel == id)).GetValueOrDefault();
        }
    }
}
