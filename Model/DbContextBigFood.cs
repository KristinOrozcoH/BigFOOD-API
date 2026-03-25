using Microsoft.EntityFrameworkCore;

namespace API_BigFOOD.Model
{
    public class DbContextBigFOOD : DbContext
    {
        public DbContextBigFOOD(DbContextOptions<DbContextBigFOOD> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetFactura> Det_Facturas { get; set; }
        public DbSet<CuentasPorCobrar> CuentasPorCobrar { get; set; }
        public DbSet<Bitacora> Bitacora { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DetFactura>()
                .HasKey(df => new { df.numFactura, df.codInterno });

            modelBuilder.Entity<Bitacora>()
                .HasNoKey(); 
        }
    }
}


