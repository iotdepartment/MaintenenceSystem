using MaintenenceSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaintenenceSystem.Controllers
{
    public class GraficasMaquinadosController : Controller
    {
        private readonly AppDbContext _context;

        public GraficasMaquinadosController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var areas = _context.OrdenesMaquinados
                .Where(o => o.Area != null)
                .GroupBy(o => o.Area)
                .Select(g => new AreaChartDto
                {
                    Area = g.Key,
                    Total = g.Count()
                })
                .ToList();

            var status = _context.OrdenesMaquinados
                .Where(o => o.Status != null)
                .GroupBy(o => o.Status)
                .Select(g => new StatusChartDto
                {
                    Status = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // NUEVO: Totales generales
            var totalGeneradas = _context.OrdenesMaquinados.Count();
            var totalCerradas = _context.OrdenesMaquinados
                .Count(o => o.Status == "Cerrada");

            var vm = new GraficasMaquinadosViewModel
            {
                Areas = areas,
                Status = status,
                TotalGeneradas = totalGeneradas,
                TotalCerradas = totalCerradas
            };

            return View(vm);
        }
    }
}
