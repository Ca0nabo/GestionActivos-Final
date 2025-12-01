using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [ValidadorCedula] // <--- Esta es la magia
        public string Cedula { get; set; }

        public int DepartamentoId { get; set; } // La llave foranea
        public Departamento? Departamento { get; set; } // La relación para navegar

        public string TipoPersona { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estado { get; set; }
    }
}