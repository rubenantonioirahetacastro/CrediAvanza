namespace CrediAvanzaAPI.Request
{
    public class ChangePasswordRequest
    {
        public string Usuario { get; set; } = null!;
        public string ContrasenaActual { get; set; } = null!;
        public string ContrasenaNueva { get; set; } = null!;
    }
}
