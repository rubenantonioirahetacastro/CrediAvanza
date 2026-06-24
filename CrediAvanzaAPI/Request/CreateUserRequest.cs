namespace CrediAvanzaAPI.Request
{
    public class CreateUserRequest
    {
        public string Documento { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Correo { get; set; } = string.Empty;
        public string[] Roles { get; set; } = new string[0];
    }
}
