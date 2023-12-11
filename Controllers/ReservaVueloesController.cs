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
    public class ReservaVueloesController : Controller
    {
        private readonly MiContexto _context;

        public ReservaVueloesController(MiContexto context)
        {
            _context = context;
        }

        // GET: ReservaVueloes
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.reservaVuelos.Include(r => r.miUsuario).Include(r => r.miVuelo);
            return View(await miContexto.ToListAsync());
        }

        // GET: ReservaVueloes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos
                .Include(r => r.miUsuario)
                .Include(r => r.miVuelo)
                .FirstOrDefaultAsync(m => m.idReservaVuelo == id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }

            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Create
        public IActionResult Create()
        {
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni");
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea");
            return View();
        }

        // POST: ReservaVueloes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idReservaVuelo,idVuelo,idUsuario,pagado")] ReservaVuelo reservaVuelo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservaVuelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos.FindAsync(id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // POST: ReservaVueloes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idReservaVuelo,idVuelo,idUsuario,pagado")] ReservaVuelo reservaVuelo)
        {
            if (id != reservaVuelo.idReservaVuelo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservaVuelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaVueloExists(reservaVuelo.idReservaVuelo))
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
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni", reservaVuelo.idUsuario);
            ViewData["idVuelo"] = new SelectList(_context.vuelos, "id", "aerolinea", reservaVuelo.idVuelo);
            return View(reservaVuelo);
        }

        // GET: ReservaVueloes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.reservaVuelos == null)
            {
                return NotFound();
            }

            var reservaVuelo = await _context.reservaVuelos
                .Include(r => r.miUsuario)
                .Include(r => r.miVuelo)
                .FirstOrDefaultAsync(m => m.idReservaVuelo == id);
            if (reservaVuelo == null)
            {
                return NotFound();
            }

            return View(reservaVuelo);
        }

        // POST: ReservaVueloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.reservaVuelos == null)
            {
                return Problem("Entity set 'MiContexto.reservaVuelos'  is null.");
            }
            var reservaVuelo = await _context.reservaVuelos.FindAsync(id);
            if (reservaVuelo != null)
            {
                _context.reservaVuelos.Remove(reservaVuelo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaVueloExists(int id)
        {
          return (_context.reservaVuelos?.Any(e => e.idReservaVuelo == id)).GetValueOrDefault();
        }
    }
}
