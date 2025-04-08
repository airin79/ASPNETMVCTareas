using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Data;
using Tareas.Model;
using Tareas.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;

namespace Tareas.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;

        public HomeController(ApplicationDbContext context, IConverter converter, IEmailService emailService)
        {
            _context = context;
            _converter = converter; // Inyección del servicio IConverter a través del constructor
            _emailService = emailService;
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
            //Console.WriteLine($"Deleting task with ID: {id}");

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return RedirectToAction("Home"); // Redirect to the Home page
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
        public async Task<IActionResult> GeneratePdf(string done = "all", DateTime? desde = null, DateTime? hasta = null, string recipientEmail = "")
        {
            // If recipientEmail is not provided, use a default one
            if (string.IsNullOrEmpty(recipientEmail))
            {
                recipientEmail = "airin79@gmail.com"; // Replace with a default email or handle this case
            }

            // Get the base query for tasks
            var tareas = _context.Tareas.AsQueryable();

            // Apply filters based on the 'done' parameter
            switch (done?.ToLower())
            {
                case "done":
                    tareas = tareas.Where(t => t.Done == true);
                    break;
                case "undone":
                    tareas = tareas.Where(t => t.Done == false);
                    break;
            }

            // Apply date filters if provided
            if (desde.HasValue)
            {
                tareas = tareas.Where(t => t.Date >= desde.Value);
            }

            if (hasta.HasValue)
            {
                tareas = tareas.Where(t => t.Date <= hasta.Value);
            }

            // Get the filtered list of tasks
            var listaFiltrada = tareas.ToList();

            // Build the HTML content for the PDF
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

            // Create the PDF document
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

            // Convert HTML to PDF
            var pdf = _converter.Convert(doc);

            // Prepare the email details
            var emailSubject = "Tareas PDF";
            var emailBody = "Please find attached the PDF with the list of tasks.";

            // Email sending logic with try-catch
            try
            {
                //var recipientEmail = "airin79@gmail.com";
                await _emailService.SendEmailAsync(recipientEmail, emailSubject, emailBody, pdf, "tareas.pdf");
            }
            catch (Exception ex)
            {
                // Log the error (or handle it based on your needs)
                Console.WriteLine($"Error sending email: {ex.Message}");

                // Modify the email body to notify about the failure
                emailBody += "<br><br><strong>Note:</strong> There was an issue sending the email with the attachment. The email was not sent, but you can still download the PDF.";

                // Send a notification email about the failure (optional)
                // Optionally send a notification to the admin or log it
            }

            // Return the generated PDF to the user
            return File(pdf, "application/pdf", "tareas.pdf");
        }


        public IActionResult MarkAsDone(int id)
        {
            var tarea = _context.Tareas.FirstOrDefault(t => t.Id == id);

            if (tarea != null)
            {
                tarea.Done = true; // Mark the task as completed
                _context.SaveChanges(); // Save the changes to the database
            }

            return RedirectToAction("Home"); // Redirect to the Home page
        }

    }


    
}