using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [ValidadorCedula]
        public string Cedula { get; set; }

        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public string TipoPersona { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estado { get; set; }
    }
}