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
    public class UsuariosController : Controller
    {
        private readonly MiContexto _context;

        public UsuariosController(MiContexto context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Loguin()
        {
            return View();
        }
        // Loguin: Usuarios
        [HttpPost]
        public async Task<IActionResult> Loguin([Bind("mail,password")] Usuario usuario)
        {

            string resp = this.login(usuario.password, usuario.mail);
            switch (resp)
            {
                case "OK":
                    //this.volverIntentosFallidosCeroContext(usuario);
                    //redirecciono Menu
                    return RedirectToAction("Menu", "Usuarios");
                case "BLOQUEADO":
                    //MessageBox.Show("Error, usuario bloqueado");
                    ViewBag.Error = "usuario bloqueado";
                    //  textContrasenia.Enabled = false;
                    //  textMail.Enabled = false;
                    //  Aceptar.Enabled = false;
                    break;
                case "MAILERROR":
                    ViewBag.Error = "usuario o contraseña incorrectos";
                    //  MessageBox.Show("Error, usuario o contraseña incorrectos");
                    break;
                case "INGRESARDATOS":
                    //  MessageBox.Show("Debe ingresar un usuario y contraseña!");
                    ViewBag.Error = "Debe ingresar un usuario y contraseña!";
                    break;
                case "FALTAUSUARIO":
                    //MessageBox.Show("No existe el usuario");
                    ViewBag.Error = "No existe el usuario";
                    break;
                default:
                    break;
            }
            return View();
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return _context.usuarios != null ?
                        View(await _context.usuarios.ToListAsync()) :
                        Problem("Entity set 'MiContexto.usuarios'  is null.");
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,apellido,dni,mail,password,intentosFallidos,bloqueado,credito,esAdmin")] Usuario usuario)
        {
            if (id != usuario.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m.id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MiContexto.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.usuarios?.Any(e => e.id == id)).GetValueOrDefault();
        }


        #region metodos privados Loguin
        public string login(string? _contraseña, string? _mail)
        {
            if (_contraseña != null && _mail != "" && _contraseña != null && _mail != "")
            {
                Usuario usuarioSeleccionados = _context.usuarios.Where(u => u.mail == _mail).FirstOrDefault();
                return validacionEstadoUsuario(usuarioSeleccionados, _mail, _contraseña);
            }
            else
            {
                return "INGRESARDATOS";
            }
        }

        private string validacionEstadoUsuario(Usuario? usuarioSeleccionados, string mailInput, string Inputpass)
        {
            return this.iniciarSesion(usuarioSeleccionados, mailInput, Inputpass);
        }

        public string iniciarSesion(Usuario? usuarioSeleccionados, string inputMail, string inputpass)
        {
            string codigoReturn;
            if (usuarioSeleccionados == null)
            {

                codigoReturn = "FALTAUSUARIO";
            }
            else if (usuarioSeleccionados.mail.Trim().Equals(inputMail) && usuarioSeleccionados.password.Trim() == inputpass && !usuarioSeleccionados.bloqueado)
            {
                codigoReturn = "OK";
                //this.usuarioActual = usuarioSeleccionados;
            }
            else
            {
                usuarioSeleccionados.intentosFallidos++;
                this.IngresarIntentosFallidosContext(usuarioSeleccionados);
                //this.modificarUsuarioActual(usuarioSeleccionados);
                if (usuarioSeleccionados.intentosFallidos >= 3)
                {
                    IngresarUsuarioBloqueadoContext(usuarioSeleccionados);
                    usuarioSeleccionados.bloqueado = true;
                    codigoReturn = "BLOQUEADO";

                }
                else
                {
                    codigoReturn = "MAILERROR";
                }
            }

            return codigoReturn;
        }

        public void IngresarIntentosFallidosContext(Usuario usu)
        {
            try
            {
                Usuario usuario = _context.usuarios.FirstOrDefault(x => x.id == usu.id);
                usuario.intentosFallidos = usuario.intentosFallidos;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void IngresarUsuarioBloqueadoContext(Usuario usuario)
        {
            try
            {
                //Usuario usuario = this.getUsuarioActual();
                usuario.bloqueado = true;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void volverIntentosFallidosCeroContext(Usuario usuario)
        {
            try
            {
                // Usuario usuario = this.getUsuarioActual();
                usuario.intentosFallidos = 0;
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion


    }
}
