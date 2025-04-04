using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Data;

namespace Tareas.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Home()
        {
            var tareas = await _context.Tarea.ToListAsync();
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

        private readonly TareasDbContext _context;

        public HomeController(TareasDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ListaTareas()
        {
            var tareas = await _context.Tarea.ToListAsync();
            return View(tareas); // esto buscará /Views/Home/ListaTareas.cshtml
        }
    }
}