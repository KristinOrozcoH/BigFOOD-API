using System.ComponentModel.DataAnnotations;

namespace API_BigFOOD.Model
{
    public class Cliente
    {
        [Key]
        public string cedulaLegal { get; set; }
        public string tipoCedula { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string estado { get; set; }
        public int Usuario { get; set; }
    }
}

