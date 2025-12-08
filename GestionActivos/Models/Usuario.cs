using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; } // Aquí vendrá "Admin" o "Usuario"
    }
}