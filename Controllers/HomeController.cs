using Microsoft.AspNetCore.Mvc;

namespace Tareas.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();  // is : /Views/Home/Index.cshtml
        }
        public IActionResult Privacy()
        {
            return View();  // is /Views/Home/Privacy.cshtml
        }
    }
}
