using MaintenenceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;



namespace MaintenenceSystem.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly AppDbContext _context;
        public EmpleadosController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var empleados = _context.Empleados.ToList();
            return View(empleados);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var Empleado = _context.Empleados.Find(id);
            if (Empleado == null)
            {
                return NotFound();
            }

            _context.Empleados.Remove(Empleado);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Empleados model)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorEmpleado = "Completa todos los campos correctamente.";
                return View("Index", _context.Empleados.ToList());
            }

            // Validar duplicado
            var existe = _context.Empleados
                .Any(e => e.NumeroEmpleado == model.NumeroEmpleado);

            if (existe)
            {
                ViewBag.ErrorEmpleado = "El número de empleado ya existe.";
                return View("Index", _context.Empleados.ToList());
            }

            _context.Empleados.Add(model);
            _context.SaveChanges();

            ViewBag.SuccessEmpleado = "Empleado registrado correctamente.";
            return View("Index", _context.Empleados.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ImportarExcel(IFormFile archivoExcel)
        {
            // Validar archivo
            if (archivoExcel == null || archivoExcel.Length == 0)
                return BadRequest("No se seleccionó ningún archivo.");

            // Validar extensión
            var extension = Path.GetExtension(archivoExcel.FileName).ToLower();
            if (extension != ".xlsx")
                return BadRequest("Solo se permiten archivos .xlsx");

            // Activar licencia EPPlus 7.x
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var lista = new List<Empleados>();
            var errores = new List<string>();

            using (var stream = new MemoryStream())
            {
                await archivoExcel.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var ws = package.Workbook.Worksheets[0];
                    int rowCount = ws.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string numeroTxt = ws.Cells[row, 1].Text.Trim();
                        string nombre = ws.Cells[row, 2].Text.Trim();

                        // Saltar filas completamente vacías
                        if (string.IsNullOrWhiteSpace(numeroTxt) && string.IsNullOrWhiteSpace(nombre))
                            continue;

                        // Validar número de empleado
                        if (!int.TryParse(numeroTxt, out int numeroEmpleado))
                        {
                            errores.Add($"Fila {row}: Número de empleado inválido.");
                            continue;
                        }

                        // Validar nombre
                        if (string.IsNullOrWhiteSpace(nombre))
                        {
                            errores.Add($"Fila {row}: El nombre está vacío.");
                            continue;
                        }

                        // Evitar duplicados
                        bool existe = _context.Empleados.Any(e => e.NumeroEmpleado == numeroEmpleado);
                        if (existe)
                        {
                            errores.Add($"Fila {row}: El número de empleado {numeroEmpleado} ya existe.");
                            continue;
                        }

                        lista.Add(new Empleados
                        {
                            NumeroEmpleado = numeroEmpleado,
                            NombreEmpleado = nombre
                        });
                    }
                }
            }

            // Guardar en BD
            if (lista.Count > 0)
            {
                _context.Empleados.AddRange(lista);
                await _context.SaveChangesAsync();
            }

            // Mostrar errores si los hay
            if (errores.Count > 0)
            {
                TempData["ErroresExcel"] = errores;
            }

            TempData["SuccessExcel"] = $"Se importaron {lista.Count} empleados correctamente.";

            return RedirectToAction("Index");
        }

    }
}
