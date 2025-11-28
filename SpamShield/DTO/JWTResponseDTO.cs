namespace InstaHyreSDETest.DTO
{
    public class JWTResponseDTO
    {
        public string Token { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime expiration { get; set; }
    }
}
