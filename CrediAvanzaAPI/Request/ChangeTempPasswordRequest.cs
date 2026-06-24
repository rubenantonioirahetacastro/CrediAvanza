namespace CrediAvanzaAPI.Request
{
    public class ChangeTempPasswordRequest
    {
        public string Usuario { get; set; } = null!;
        public string ContrasenaNueva { get; set; } = null!;
    }
}
