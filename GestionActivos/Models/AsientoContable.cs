namespace GestionActivos.Models
{
    // Esta clase solo sirve para dar formato al JSON, no se guarda en la base de datos
    public class AsientoContable
    {
        public int IdentificadorAsiento { get; set; }
        public string Descripcion { get; set; }
        public int IdentificadorTipoInventario { get; set; }
        public string CuentaContable { get; set; }
        public string TipoMovimiento { get; set; } // "DB" (Débito) o "CR" (Crédito)
        public DateTime FechaAsiento { get; set; }
        public decimal MontoAsiento { get; set; }
        public bool Estado { get; set; }
    }
}