using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class TipoActivo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Cuenta Compra")]
        public string CuentaCompra { get; set; }

        [Display(Name = "Cuenta Depreciación")]
        public string CuentaDepreciacion { get; set; }

        // --- ESTE ES EL CAMPO QUE TE FALTA 👇 ---
        [Display(Name = "Vida Útil (Meses)")]
        public int VidaUtil { get; set; } 
        // ----------------------------------------

        public bool Estado { get; set; }
    }
}