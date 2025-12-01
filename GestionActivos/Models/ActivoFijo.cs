using System.ComponentModel.DataAnnotations;

namespace GestionActivos.Models
{
    public class ActivoFijo
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public int TipoActivoId { get; set; }
        public TipoActivo? TipoActivo { get; set; }

        public DateTime FechaRegistro { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal DepreciacionAcumulada { get; set; }
    }
}