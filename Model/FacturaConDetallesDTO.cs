namespace API_BigFOOD.Model
{
    public class FacturaConDetallesDTO
    {
        public Factura Factura { get; set; }
        public List<DetFactura> Detalles { get; set; }
    }
}
