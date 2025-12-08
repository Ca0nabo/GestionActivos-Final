using Microsoft.EntityFrameworkCore;
using GestionActivos.Models;

namespace GestionActivos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<TipoActivo> TiposActivos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<ActivoFijo> ActivosFijos { get; set; }
        public DbSet<CalculoDepreciacion> CalculoDepreciacion { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}