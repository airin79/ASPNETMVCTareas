using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using Tareas.Model;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Tareas.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter; // Inyección del servicio IConverter a través del constructor
        }

        public async Task<IActionResult> Home()
        {
            var tareas = await _context.Tareas.ToListAsync();
            return View(tareas);
        }

        public IActionResult Privacy()
        {
            return View();  // is /Views/Home/Privacy.cshtml
        }
        public IActionResult About()
        {
            return View(); // buscará la vista en /Views/Home/About.cshtml
        }

        private readonly ApplicationDbContext _context;

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Tareas.Add(tarea);         
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Home));   // open Home view
            }
            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Home)); // Redirige de vuelta a la lista de tareas
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea); // Carga la vista con el modelo Tarea
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Home)); // o ListaTareas si prefieres
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tareas.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(tarea);
        }

        private readonly IConverter _converter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="done">done=all, done=done y done=undone</param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IActionResult GeneratePdf(string done = "all", DateTime? desde = null, DateTime? hasta = null)
        {
            //var tareas = _context.Tareas.ToList();    // All records

            // Tareas base
            var tareas = _context.Tareas.AsQueryable();

            // Imprimir valor de 'done' para verificar si se recibe correctamente
            Console.WriteLine($"Received done parameter: {done}");

            switch (done?.ToLower())    // Filtro por done
            {
                case "done":
                    tareas = tareas.Where(t => t.Done == true);
                    break;
                case "undone":
                    tareas = tareas.Where(t => t.Done == false);
                    break;
                    // "all" no filtra nada
            }

            // Date filter
            if (desde.HasValue)
            {
                tareas = tareas.Where(t => t.Date >= desde.Value);
            }

            if (hasta.HasValue)
            {
                tareas = tareas.Where(t => t.Date <= hasta.Value);
            }

            var listaFiltrada = tareas.ToList();

            var htmlContent = "<html><head><style>" +
                              "table { width: 100%; border-collapse: collapse; }" +
                              "th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }" +
                              "th { background-color: #f2f2f2; }" +
                              "</style></head><body>" +
                              "<h1>Tareas</h1>" +
                              "<table>" +
                              "<thead><tr><th>Name</th><th>Description</th><th>Date</th><th>Done</th></tr></thead><tbody>";

            foreach (var tarea in listaFiltrada)
            {
                htmlContent += $"<tr><td>{tarea.Name}</td><td>{tarea.Descripcion}</td><td>{tarea.Date.ToString("yyyy-MM-dd")}</td><td>{tarea.Done}</td></tr>";
            }

            htmlContent += "</tbody></table>" +
               $"<div class='footer' style='text-align:justify; text-align-last:center;'>{DateTime.Now.ToString("dddd, dd MMMM yyyy")}</div>" +
               "</body></html>";

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            },
                Objects = { new ObjectSettings() {
                HtmlContent = htmlContent }}

        };

        var pdf = _converter.Convert(doc);
        return File(pdf, "application/pdf", "tareas.pdf");
        }

    }
}