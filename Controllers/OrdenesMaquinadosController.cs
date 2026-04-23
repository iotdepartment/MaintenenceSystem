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

            return RedirectToAction(nameof(Index));
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