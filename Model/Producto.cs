using System.ComponentModel.DataAnnotations;

namespace API_BigFOOD.Model
{
    public class Producto
    {
        [Key]
        public int CodigoInterno { get; set; }
        public string? CodigoBarra { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public string UnidadMedida { get; set; }
        public decimal PrecioCompra { get; set; }
        public int Usuario { get; set; }
        public int Existencia { get; set; }
    }
}

