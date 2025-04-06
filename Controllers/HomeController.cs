using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using Tareas.Model;

namespace Tareas.Controllers
{
    public class HomeController : Controller
    {
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

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

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

    }
}