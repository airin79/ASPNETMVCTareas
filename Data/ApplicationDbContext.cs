using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Tareas.Model;

namespace Tareas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tarea> Tareas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        
    }
}
