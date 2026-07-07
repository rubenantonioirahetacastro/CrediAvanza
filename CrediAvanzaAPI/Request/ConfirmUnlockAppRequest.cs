namespace CrediAvanzaAPI.Request
{
    public class ConfirmUnlockAppRequest
    {
        public string Usuario { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
