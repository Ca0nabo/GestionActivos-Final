using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class CalculoDepreciacion
    {
        [Key]
        public int Id { get; set; }
        public int AnoProceso { get; set; }
        public int MesProceso { get; set; }

        public int ActivoFijoId { get; set; }
        public ActivoFijo? ActivoFijo { get; set; } // Relación

        public DateTime FechaProceso { get; set; }
        public decimal MontoDepreciado { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
        public string CuentaCompra { get; set; }
        public string CuentaDepreciacion { get; set; }
    }
}