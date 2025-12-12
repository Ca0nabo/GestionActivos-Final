namespace GestionActivos.Models
{
    public class AsientoContable
    {
        public int IdentificadorAsiento { get; set; }
        public string Descripcion { get; set; }
        public int IdentificadorTipoInventario { get; set; }
        public string CuentaContable { get; set; }
        public string TipoMovimiento { get; set; }
        public DateTime FechaAsiento { get; set; }
        public decimal MontoAsiento { get; set; }
        public bool Estado { get; set; }
    }
}