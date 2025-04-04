using Tareas.Data;
using Tareas.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TaskController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Home()
    {
        var tasks = await _context.Tareas.ToListAsync();
        return View(tasks);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Tarea task)
    {
        if (ModelState.IsValid)
        {
            _context.Add(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Home));
        }
        return View(task);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _context.Tareas.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        _context.Tareas.Remove(task);
        await _context.SaveChangesAsync();
       
        return RedirectToAction(nameof(Home));
    }
}

