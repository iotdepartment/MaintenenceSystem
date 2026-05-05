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

        public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Si no hay fechas, usar el mes actual
            if (!fechaInicio.HasValue || !fechaFin.HasValue)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1); // primer día del mes
                fechaFin = fechaInicio.Value.AddMonths(1).AddDays(-1); // último día del mes
            }

            var query = _context.OrdenesMaquinados.AsQueryable();

            // FILTRO POR RANGO DE FECHAS
            query = query.Where(o =>
                o.FechaInicio.HasValue &&
                o.FechaInicio.Value.Date >= fechaInicio.Value.Date &&
                o.FechaInicio.Value.Date <= fechaFin.Value.Date
            );

            // GRÁFICA POR ÁREA
            var areas = query
                .Where(o => o.Area != null)
                .GroupBy(o => o.Area)
                .Select(g => new AreaChartDto
                {
                    Area = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // GRÁFICA POR STATUS
            var status = query
                .Where(o => o.Status != null)
                .GroupBy(o => o.Status)
                .Select(g => new StatusChartDto
                {
                    Status = g.Key,
                    Total = g.Count()
                })
                .ToList();

            // GENERADAS Y CERRADAS
            var totalGeneradas = query.Count();
            var totalCerradas = query.Count(o => o.Status == "Cerrada");

            var vm = new GraficasMaquinadosViewModel
            {
                Areas = areas,
                Status = status,
                TotalGeneradas = totalGeneradas,
                TotalCerradas = totalCerradas,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            return View(vm);
        }
    }
}
