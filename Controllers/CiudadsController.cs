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
    public class CiudadsController : Controller
    {
        private readonly MiContexto _context;

        public CiudadsController(MiContexto context)
        {
            _context = context;
        }

        // GET: Ciudads
        public async Task<IActionResult> Index()
        {
              return _context.ciudades != null ? 
                          View(await _context.ciudades.ToListAsync()) :
                          Problem("Entity set 'MiContexto.ciudades'  is null.");
        }

        // GET: Ciudads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ciudades == null)
            {
                return NotFound();
            }

            var ciudad = await _context.ciudades
                .FirstOrDefaultAsync(m => m.id == id);
            if (ciudad == null)
            {
                return NotFound();
            }

            return View(ciudad);
        }

        // GET: Ciudads/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ciudads/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,nombre")] Ciudad ciudad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ciudad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ciudad);
        }

        // GET: Ciudads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ciudades == null)
            {
                return NotFound();
            }

            var ciudad = await _context.ciudades.FindAsync(id);
            if (ciudad == null)
            {
                return NotFound();
            }
            return View(ciudad);
        }

        // POST: Ciudads/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,nombre")] Ciudad ciudad)
        {
            if (id != ciudad.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ciudad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CiudadExists(ciudad.id))
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
            return View(ciudad);
        }

        // GET: Ciudads/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ciudades == null)
            {
                return NotFound();
            }

            var ciudad = await _context.ciudades
                .FirstOrDefaultAsync(m => m.id == id);
            if (ciudad == null)
            {
                return NotFound();
            }

            return View(ciudad);
        }

        // POST: Ciudads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ciudades == null)
            {
                return Problem("Entity set 'MiContexto.ciudades'  is null.");
            }
            var ciudad = await _context.ciudades.FindAsync(id);
            if (ciudad != null)
            {
                _context.ciudades.Remove(ciudad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CiudadExists(int id)
        {
          return (_context.ciudades?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
