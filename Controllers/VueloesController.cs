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
    public class VueloesController : Controller
    {
        private readonly MiContexto _context;

        public VueloesController(MiContexto context)
        {
            _context = context;
        }

        // GET: Vueloes
        public async Task<IActionResult> Index()
        {
            var miContexto = _context.vuelos.Include(v => v.destino).Include(v => v.origen);
            return View(await miContexto.ToListAsync());
        }

        // GET: Vueloes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.vuelos == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos
                .Include(v => v.destino)
                .Include(v => v.origen)
                .FirstOrDefaultAsync(m => m.id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        // GET: Vueloes/Create
        public IActionResult Create()
        {
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre");
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre");
            return View();
        }

        // POST: Vueloes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,CiudadOrigenId,CiudadDestinoId,capacidad,vendido,costo,fecha,aerolinea,avion")] Vuelo vuelo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vuelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // GET: Vueloes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.vuelos == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos.FindAsync(id);
            if (vuelo == null)
            {
                return NotFound();
            }
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // POST: Vueloes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,CiudadOrigenId,CiudadDestinoId,capacidad,vendido,costo,fecha,aerolinea,avion")] Vuelo vuelo)
        {
            if (id != vuelo.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vuelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VueloExists(vuelo.id))
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
            ViewData["CiudadDestinoId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadDestinoId);
            ViewData["CiudadOrigenId"] = new SelectList(_context.ciudades, "id", "nombre", vuelo.CiudadOrigenId);
            return View(vuelo);
        }

        // GET: Vueloes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.vuelos == null)
            {
                return NotFound();
            }

            var vuelo = await _context.vuelos
                .Include(v => v.destino)
                .Include(v => v.origen)
                .FirstOrDefaultAsync(m => m.id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        // POST: Vueloes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.vuelos == null)
            {
                return Problem("Entity set 'MiContexto.vuelos'  is null.");
            }
            var vuelo = await _context.vuelos.FindAsync(id);
            if (vuelo != null)
            {
                _context.vuelos.Remove(vuelo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VueloExists(int id)
        {
          return (_context.vuelos?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
