using System.ComponentModel.DataAnnotations;

namespace Tareas.Model
{
    public class Tarea
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public bool EstaCompleta { get; set; }


        /*
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "M�ximo 100 caracteres")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "M�ximo 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        public DateTime DueDate { get; set; }

        public Guid Id { get; set; }
        */
    }
}
