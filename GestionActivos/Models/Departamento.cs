using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class Departamento
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}