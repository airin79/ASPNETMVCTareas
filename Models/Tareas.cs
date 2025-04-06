using System.ComponentModel.DataAnnotations;

namespace Tareas.Model
{
    public class Tarea
    {
            // Error message constant
            public const string RequiredFieldError = "Mandatory field";

            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = RequiredFieldError)]
            [StringLength(100)]
            public string Name { get; set; }

            [StringLength(500)]
            public string Descripcion { get; set; }

            [Required(ErrorMessage = RequiredFieldError)]
            public DateTime Date { get; set; }

            public bool Done { get; set; }

    }
}
