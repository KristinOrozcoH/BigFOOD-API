namespace API_BigFOOD.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string estado { get; set; }
    }
}

