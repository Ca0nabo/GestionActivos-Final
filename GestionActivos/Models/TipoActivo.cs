using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class TipoActivo
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CuentaCompra { get; set; }
        public string CuentaDepreciacion { get; set; }
        public bool Estado { get; set; }
    }
}