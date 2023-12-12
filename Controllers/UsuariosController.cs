﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

                if ((this.existeUsuarioConDniOMail(usuario.dni, usuario.mail)))
                {
                    //   MessageBox.Show("ya existe un usuario con el mismo mail o dni.");
                    //true
                }
                else
                {
                    if (usuario.name.Length >= 3 && usuario.apellido.Length >= 3 && usuario.dni.Length == 8 && usuario.mail.Contains("@"))
                    {
                        //Dni, Nombre, apellido, Mail,pass, EsADM, Bloqueado);
                        await this.agregarUsuarioContextoAsync(usuario.dni, usuario.name, usuario.apellido, usuario.mail, usuario.password, usuario.esAdmin, usuario.bloqueado);

                        //  MessageBox.Show("Agregado con éxito");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // MessageBox.Show("Problemas al agregar");
                    }
                }

                //_context.Add(usuario);
                //await _context.SaveChangesAsync();
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
                //  MessageBox.Show("Debe seleccionar un usuario y no puede haber datos incompletos");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(usuario.name) && !string.IsNullOrEmpty(usuario.apellido) &&
                    !string.IsNullOrEmpty(usuario.dni) && !string.IsNullOrEmpty(usuario.mail))
                    {

                        //Dni, Nombre, apellido, Mail,pass, EsADM, Bloqueado);
                        // int Id, string Nombre, string Apellido, string Dni, string Mail
                        if (this.modificarUsuariocontexto(id, usuario.name, usuario.apellido, usuario.dni, usuario.mail, usuario.password, usuario.esAdmin, usuario.bloqueado))
                        {
                            // MessageBox.Show("Modificado con éxito");
                        }
                        else
                        {
                            // MessageBox.Show("Problemas al modificar");
                        }
                    }
                    else
                    {
                        // MessageBox.Show("no pueden haber datos incompletos");
                    }
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
               //MessageBox.Show("Debe seleccionar un usuario");
                return Problem("Entity set 'MiContexto.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                this.eliminarUsuarioContext(id);
                //if (this.eliminarUsuarioContext(id))
                //  MessageBox.Show("Eliminado con éxito");
                //  else
                //  MessageBox.Show("Problemas al eliminar");
            }
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

        #region usuario

        //verificar si ya existe un usuario con ese mail o dni
        //devuelve true si encuentra
        public bool existeUsuarioConDniOMail(string dni, string mail)
        {
            return _context.usuarios.Any(u => u.dni == dni || u.mail == mail);
        }

        public async Task<bool> agregarUsuarioContextoAsync(string Dni, string Nombre, string apellido, string Mail, string Password, bool EsADM, bool Bloqueado)
        {
            //comprobación para que no me agreguen usuarios con DNI duplicado
            bool esValido = true;

            List<Usuario> listUsuarios = _context.usuarios.ToList();

            foreach (Usuario u in listUsuarios)
            {
                if (u.dni == Dni)
                {
                    esValido = false;
                }
            }
            try
            {
                if (esValido)
                {
                    Usuario nuevo = new Usuario { dni = Dni, name = Nombre, apellido = apellido, mail = Mail, password = Password, esAdmin = EsADM, bloqueado = Bloqueado };
                    _context.Add(nuevo);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool modificarUsuariocontexto(int Id, string Nombre, string Apellido, string Dni, string Mail, string pass, bool admin, bool bloqueado)
        {
            //busco usuario y lo asocio a la variable
            Usuario u = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
            if (u != null)
            {
                try
                {
                    u.name = Nombre;
                    u.apellido = Apellido;
                    u.dni = Dni.ToString();
                    u.mail = Mail;
                    u.password = pass;
                    u.esAdmin = admin;
                    u.bloqueado = bloqueado;
                    _context.usuarios.Update(u);
                    _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 1 registro
                return false;
            }
        }
        public bool eliminarUsuarioContext(int Id)
        {
            Usuario u = _context.usuarios.Where(u => u.id == Id).FirstOrDefault();
            if (u != null)
            {
                try
                {
                    u.listMisReservasVuelo.Where(rv => rv.idUsuario == Id).ToList();
                    foreach (ReservaVuelo rv in u.listMisReservasVuelo)
                    {
                        int cantReservas = (int)(rv.pagado / rv.miVuelo.costo);
                        rv.miVuelo.vendido -= cantReservas;
                        _context.vuelos.Update(rv.miVuelo);
                    }
                    _context.usuarios.Remove(u);
                    _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                    throw;
                }
            }
            else
            {
                //algo salió mal con la query porque no generó 
                return false;
            }
        }
        #endregion
    }
}
