using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistemaWEB.Models;
using sistemaWEB.Models.Business.ReservaHotel;

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
        public IActionResult Create(BusinessReservaHotel ReservaHotel)
        {
            ViewData["idHotel"] = new SelectList(_context.hoteles, "id", "nombre");
            ViewData["idUsuario"] = new SelectList(_context.usuarios, "id", "dni");
            ViewData["Ciudad"] = new SelectList(_context.ciudades, "id", "nombre");

            ReservaHotel.DispReservaHotel = this.traerDisponibilidad(ReservaHotel);

            ViewBag.DispReservaHotel = this.traerDisponibilidad(ReservaHotel);

            return View(new BusinessReservaHotel() { fechaDesde = DateTime.Now, fechaHasta = DateTime.Now, DispReservaHotel = ReservaHotel.DispReservaHotel });

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateConsulta([Bind("Ciudad,cantidadPersonas,fechaDesde,fechaHasta,monto")] BusinessReservaHotel reservaHotel)
        {
            return RedirectToAction(nameof(Create), "ReservaHotels", new RouteValueDictionary(reservaHotel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar([Bind("id,ubicacion,disponibilidad,costo,nombre,FechaDesde,FechaHasta")] BusinessDispReservaHotel BusinessDispReservaHotel, [Bind("Ciudad,cantidadPersonas,fechaDesde,fechaHasta,monto")] BusinessReservaHotel businessReservaHotel)
        {

            if (await this.GenerarReserva(BusinessDispReservaHotel.nombre, businessReservaHotel.fechaDesde, businessReservaHotel.fechaHasta, Convert.ToString(businessReservaHotel.monto), Convert.ToString(businessReservaHotel.cantidadPersonas)) != null)
            {
                //dataGridViewHotel.Rows.Add(new string[] { Agencia.getHotelesByHotel(boxHoteles.Text).nombre, textBoxMonto.Text, Convert.ToString(Agencia.getHotelesByHotel(boxHoteles.Text).capacidad), fechaIngreso.ToShortDateString(), fechaEgreso.ToShortDateString() });
                //disponibilidad = true;
            }


            return RedirectToAction(nameof(Create), "ReservaHotels", new RouteValueDictionary());
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


        #region funcions reserva Hotel

        private List<BusinessCiudad> mappingCiudades()
        {
            List<BusinessCiudad> businessCiudads = new List<BusinessCiudad>();
            foreach (var item in _context.ciudades)
            {
                BusinessCiudad ciudad = new BusinessCiudad();
                ciudad.id = item.id;
                ciudad.nombre = item.nombre;
                businessCiudads.Add(ciudad);
            }
            return businessCiudads;
        }

        private List<BusinessDispReservaHotel> traerDisponibilidad(BusinessReservaHotel businessReservaHotel)
        {
            DateTime fechaIngreso = businessReservaHotel.fechaDesde;
            DateTime fechaEgreso = businessReservaHotel.fechaHasta;
            List<BusinessDispReservaHotel> BusinessDispReservaHotel = new List<BusinessDispReservaHotel>();

            //se genera los datos de la grilla a traves de la disponibilidad generada, tambien se ve reflejado en la grilla la disponibilidad del hotel, si no hay disponibilidad directamente no aparece el registro para seleccionar
            foreach (var itemHotel in this.TraerDisponibilidadHotel(businessReservaHotel.Ciudad, fechaIngreso, fechaEgreso, Convert.ToInt32(businessReservaHotel.cantidadPersonas)))
            {
                itemHotel.ubicacion = _context.ciudades.FirstOrDefault(x => x.id == itemHotel.idCiudad);
                //dataGridViewHotel.Rows.Add(new string[] { Convert.ToString(itemHotel.id), itemHotel.ubicacion.nombre, Convert.ToString(this.calcularDisponibilidad(itemHotel, fechaIngreso, fechaEgreso)), Convert.ToString(this.CalcularCosto(fechaEgreso, fechaIngreso, itemHotel.costo)), itemHotel.nombre, fechaIngreso.ToShortDateString(), fechaEgreso.ToShortDateString() });
                BusinessDispReservaHotel.Add(new BusinessDispReservaHotel { id = itemHotel.id, ubicacion = itemHotel.ubicacion.nombre, disponibilidad = this.calcularDisponibilidad(itemHotel, fechaIngreso, fechaEgreso), costo = this.CalcularCosto(fechaEgreso, fechaIngreso, itemHotel.costo), nombre = itemHotel.nombre, FechaDesde = businessReservaHotel.fechaDesde, FechaHasta = businessReservaHotel.fechaHasta });
            }
            return BusinessDispReservaHotel;
        }

        public List<Hotel> TraerDisponibilidadHotel(int ciudadSeleccionada, DateTime fechaIngreso, DateTime fechaEgreso, Int32 cantPersonas)
        {
            bool estaRango;
            int difCantPer = 0;
            List<Hotel> hotelesDisponibles = new List<Hotel>();
            //recorre por cada hotel las reservas
            foreach (var itemHotel in _context.hoteles)
            {
                if (itemHotel.idCiudad == ciudadSeleccionada)
                {
                    difCantPer = itemHotel.capacidad;
                    foreach (var itemReserva in itemHotel?.listMisReservas)
                    {
                        //Verifica si esta en rango de fecha seleccionada con respecto a la fechas de las reservas que hay en el hotel
                        estaRango = this.verificacionRango(itemReserva, itemHotel, fechaIngreso, fechaEgreso);
                        if (estaRango)
                        {
                            //si esta en rango, le va restando la cantidad de personas por reserva a la capacidad total sobre ese rango de fecha que debe coincidir con el rango de fecha seleccionado
                            difCantPer = difCantPer - itemReserva.cantidadPersonas;
                        }
                    }
                    //a esa resta que se le hizo de la capacidad total, se le resta tamb la cantidad de personas que se ingreso en esa alta de reserva
                    int total = difCantPer - cantPersonas;
                    //se agrega a la lista si el hotel esta disponible, si hay disponibilidad en esa fecha.
                    if (itemHotel.capacidad >= total && total >= 0)
                        hotelesDisponibles.Add(itemHotel);
                }
            }

            return hotelesDisponibles;
        }


        private bool verificacionRango(ReservaHotel itemReserva, Hotel itemHotel, DateTime fechaIngreso, DateTime fechaEgreso)
        {
            bool estaRango = false;

            if (itemReserva.miHotel.id == itemHotel.id)
            {

                if ((itemReserva.fechaDesde.Date >= fechaIngreso.Date) && (itemReserva.fechaHasta.Date <= fechaEgreso.Date))
                {
                    estaRango = true;
                }

                if (fechaIngreso.Date >= itemReserva.fechaDesde && fechaIngreso.Date <= itemReserva.fechaHasta.Date)
                {
                    estaRango = true;
                }

                if (fechaEgreso.Date <= itemReserva.fechaHasta.Date && fechaEgreso.Date >= itemReserva.fechaDesde.Date)
                {
                    estaRango = true;
                }

            }
            else
            {
                estaRango = false;
            }
            return estaRango;
        }

        private int calcularDisponibilidad(Hotel itemHotel, DateTime fechaIngreso, DateTime fechaEgreso)
        {
            int difCantPer = itemHotel.capacidad;
            bool estaRango;
            foreach (var itemMiReserva in itemHotel.listMisReservas)
            {
                estaRango = this.verificacionRango(itemMiReserva, itemHotel, fechaIngreso, fechaEgreso);
                if (estaRango)
                {
                    difCantPer = difCantPer - itemMiReserva.cantidadPersonas;
                }
            }
            return difCantPer;
        }

        private double CalcularCostoParaEdicion(DateTime fechaDesde, DateTime fechaHasta, Int32 idHotel)
        {
            Hotel miHotel = _context.hoteles.FirstOrDefault(x => x.id == idHotel);
            TimeSpan ts = fechaHasta.Date.Subtract(fechaDesde.Date);
            return (ts.Days + 1) * miHotel.costo;
        }

        private double CalcularCosto(DateTime fechaHasta, DateTime fechaDesde, double costo)
        {
            TimeSpan ts = fechaHasta.Date.Subtract(fechaDesde.Date);
            return ((ts.Days + 1) * costo);
        }

        public Hotel? getHotelesByHotel(string boxHoteles)
        {
            return _context.hoteles?.FirstOrDefault(x => x.nombre == boxHoteles);
        }
        public async Task<ReservaHotel>? GenerarReserva(string nombreHotel, DateTime fechaIngreso, DateTime fechaEgreso, string textBoxMonto, string cantidadPersonas)
        {
            var usuarioActual = Helper.SessionExtensions.Get<Usuario>(HttpContext.Session, "usuarioActual");
            //Busco el hotel por nombre
            Hotel hotelSeleccionado = this.getHotelesByHotel(nombreHotel);
            //Calcula el costo por rango de fechas, sobre el costo que sale el hotel lo multiplica por cantidad de dias
            TimeSpan ts = fechaEgreso.Date.Subtract(fechaIngreso.Date);
            double costo = ((ts.Days + 1) * hotelSeleccionado.costo);
            ReservaHotel? reservaHotel = null;
            //Verifica que el costo calculado sea igual al costo ingresado
            if (costo == Convert.ToDouble(textBoxMonto))
            {
                try
                {
                    //crea un objeto reserva hotel
                    reservaHotel = new ReservaHotel(hotelSeleccionado, usuarioActual, fechaIngreso, fechaEgreso, Convert.ToDouble(textBoxMonto), Convert.ToInt32(cantidadPersonas));
                    //Genera la reserva en la base a traves del context
                    await this.generarReservaContextAsync(reservaHotel);
                    //odbtiene el objeto de la tabla intermedia correspondiente al id usuario y al hotel
                    HotelUsuario hotelUsuario = _context.HotelUsuario.Where(x => x.idUsuario == reservaHotel.miUsuario.id && x.idHotel == reservaHotel.miHotel.id).FirstOrDefault();
                    //Modifica la tabla intermedia o genera un registro nuevo dependiendo si existe esa relacion de hotel, usuario
                    this.generarHotelUsuario(hotelUsuario, reservaHotel);
                    //se obtiene el usuario actual, se le resta el credito para que quede registrado en memoria
                    usuarioActual.credito = usuarioActual.credito - Convert.ToDouble(textBoxMonto);
                    //se modifica el dato en la base a traves del context
                    modificarCreditoContext(usuarioActual);
                    // this.modificarUsuarioActual(usuarioActual); revisar

                    Helper.SessionExtensions.Set(HttpContext.Session, "usuarioActual", usuarioActual);

                }
                catch (Exception ex)
                {
                    return null;
                }
                //setea el usuario actual
                return reservaHotel;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> generarReservaContextAsync(ReservaHotel reservaHotel)
        {
            try
            {
                _context.reservaHoteles.Add(reservaHotel);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool generarHotelUsuario(HotelUsuario? hotelUsuario, ReservaHotel reservaHotel)
        {
            try
            {
                //si no existe el hotel crea una nueva realacion y genera como registro que se visito una ves a ese hotel, si no modifica la cantidad sumandole uno a esa visita
                if (hotelUsuario == null)
                {
                    _context.HotelUsuario.Add(new HotelUsuario() { idHotel = reservaHotel.miHotel.id, idUsuario = reservaHotel.miUsuario.id, cantidad = 1 });
                }
                else
                {
                    hotelUsuario.cantidad++;
                    _context.HotelUsuario.Update(hotelUsuario);
                }
                _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool modificarCreditoContext(Usuario usuarioActual)
        {
            try
            {
                Usuario? usuario = _context.usuarios.Where(u => u.id == usuarioActual.id).FirstOrDefault();

                if (usuario != null)
                {
                    usuario.credito = usuarioActual.credito;
                    _context.usuarios.Update(usuario);
                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion


    }
}
