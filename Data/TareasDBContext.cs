using Microsoft.EntityFrameworkCore;
using Tareas.Model;

namespace Tareas.Data
{
    public class TareasDbContext : DbContext
    {
        public TareasDbContext(DbContextOptions<TareasDbContext> options) : base(options)
        {
        }

        public DbSet<Tarea> Tarea { get; set; }
    }
}

