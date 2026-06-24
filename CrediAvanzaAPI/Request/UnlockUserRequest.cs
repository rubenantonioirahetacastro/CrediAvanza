namespace CrediAvanzaAPI.Request
{
    public class UnlockUserRequest
    {
        public string Usuario { get; set; } = null!;
        public string? Observacion { get; set; }
    }
}
