namespace API_BigFOOD.Model
{
    public class ClienteGometaDTO
    {
        public int resultcount { get; set; }
        public List<ClienteInfo> results { get; set; } = new();
    }

    public class ClienteInfo
    {
        public string fullname { get; set; } = string.Empty;     
        public string guess_type { get; set; } = string.Empty;   
    }
}