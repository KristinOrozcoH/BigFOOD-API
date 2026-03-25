using System.ComponentModel.DataAnnotations;

namespace API_BigFOOD.Model
{
    public class Factura
    {
        [Key]
        public int numero { get; set; } 
        public DateTime Fecha { get; set; }
        public string codCliente { get; set; }
        public decimal Subtotal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoImpuesto { get; set; }
        public decimal Total { get; set; }
        public string estado { get; set; }
        public int Usuario { get; set; }
        public string TipoPago { get; set; }
        public string Condicion { get; set; }
    }
}

