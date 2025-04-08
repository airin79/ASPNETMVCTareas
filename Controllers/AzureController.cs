using Microsoft.AspNetCore.Mvc;

namespace Tareas.Controllers
{
    namespace Tareas.Controllers
    {
        public class AzureController : Controller
        {
            private readonly AzureStorageService _azureStorageService;

            public AzureController(AzureStorageService azureStorageService)
            {
                _azureStorageService = azureStorageService;
            }

            // GET: Azure/TestConnection
            public IActionResult TestConnection()
            {
                // Call the Azure storage service to test the connection and get the result
                var result = _azureStorageService.TestConnectionAsync().Result;
                //ViewBag.TestResult = result;  // temp to pass o a dedicated view
                TempData["TestResult"] = result;
                return RedirectToAction("Home", "Home"); // Action "Home" from controller "Home"
            }
        }
    }

}

