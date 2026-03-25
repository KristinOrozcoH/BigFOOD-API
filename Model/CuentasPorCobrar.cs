using System.ComponentModel.DataAnnotations;

namespace API_BigFOOD.Model
{
    public class CuentasPorCobrar
    {
        [Key]
        public int numFactura { get; set; }
        public string codCliente { get; set; }
        public DateTime FechaFactura { get; set; }
        public DateTime FechaRegistro { get; set; }
        public decimal montoFactura { get; set; }
        public int Usuario { get; set; }
        public string estado { get; set; }
    }
} 

