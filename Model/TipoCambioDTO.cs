namespace API_BigFOOD.Model
{
    public class TipoCambioDTO
    {
        public string moneda { get; set; } = string.Empty;
        public decimal compra { get; set; }
        public decimal venta { get; set; }
        public string fecha { get; set; } = string.Empty;
    }
}
