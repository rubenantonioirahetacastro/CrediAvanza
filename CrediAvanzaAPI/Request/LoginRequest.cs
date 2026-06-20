namespace CrediAvanzaAPI.Request
{
    public class LoginRequest
    {
        public string Documento { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
