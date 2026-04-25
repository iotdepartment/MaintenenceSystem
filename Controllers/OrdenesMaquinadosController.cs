using MaintenenceSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaintenenceSystem.Controllers
{
    public class OrdenesMaquinadosController : Controller
    {
        private readonly AppDbContext _context;

        public OrdenesMaquinadosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Areas = _context.Areas.ToList();
            return View(_context.OrdenesMaquinados.ToList());
        }

        public IActionResult AprobarOrden()
        {
            ViewBag.Areas = _context.Areas.ToList();
            return View(_context.OrdenesMaquinados.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrdenesMaquinados orden)
        {
            // Validar que el número de empleado exista en la base de datos
            var empleado = _context.Empleados
                .FirstOrDefault(e => e.NumeroEmpleado == orden.NumeroEmpleado);

            if (empleado == null)
            {
                ModelState.AddModelError("NumeroEmpleado", "El número de empleado no existe.");

                // Recargar datos necesarios para la vista
                ViewBag.Areas = _context.Areas.ToList();

                // Regresar a Index con la lista de órdenes
                return View("Index", _context.OrdenesMaquinados.ToList());
            }

            // Validación general del modelo
            if (!ModelState.IsValid)
            {
                ViewBag.Areas = _context.Areas.ToList();
                return View("Index", _context.OrdenesMaquinados.ToList());
            }

            int year = DateTime.Now.Year;
            string prefix = $"M-{year}-";

            var lastOrder = _context.OrdenesMaquinados
                .Where(o => o.Folio != null && o.Folio.StartsWith(prefix))
                .OrderByDescending(o => o.ID)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastOrder != null)
            {
                string lastFolio = lastOrder.Folio!;
                string numberPart = lastFolio.Substring(prefix.Length);
                nextNumber = int.Parse(numberPart) + 1;
            }

            orden.Folio = $"{prefix}{nextNumber.ToString("D3")}";
            orden.FechaInicio = DateTime.Now;
            orden.Status = "Pendiente";

            _context.OrdenesMaquinados.Add(orden);
            _context.SaveChanges();
            TempData["OrdenCreada"] = true;

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult TomarOrden(int id)
        {
            var orden = _context.OrdenesMaquinados.FirstOrDefault(o => o.ID == id);

            if (orden == null)
                return NotFound();

            orden.Status = "En proceso";
            orden.FechaAceptacion = DateTime.Now;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult PausarOrden(int id)
        {
            var orden = _context.OrdenesMaquinados.FirstOrDefault(o => o.ID == id);

            if (orden == null)
                return NotFound();

            orden.Status = "Pausada";

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult CancelarOrden(int id)
        {
            var orden = _context.OrdenesMaquinados.FirstOrDefault(o => o.ID == id);

            if (orden == null)
                return NotFound();

            orden.Status = "Cancelada";

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult CerrarOrden(int id)
        {
            var orden = _context.OrdenesMaquinados.FirstOrDefault(o => o.ID == id);

            if (orden == null)
                return NotFound();

            orden.Status = "Cerrada";
            orden.FechaEntrega = DateTime.Now;

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult ReanudarOrden(int id)
        {
            var orden = _context.OrdenesMaquinados.FirstOrDefault(o => o.ID == id);

            if (orden == null)
                return NotFound();

            orden.Status = "En proceso";

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IActionResult ObtenerEmpleado(int numero)
        {
            var empleado = _context.Empleados
                .FirstOrDefault(e => e.NumeroEmpleado == numero);

            if (empleado == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                nombre = empleado.NombreEmpleado
            });
        }
    }
}