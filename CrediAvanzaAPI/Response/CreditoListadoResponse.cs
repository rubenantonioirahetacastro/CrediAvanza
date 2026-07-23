using System.Text.Json.Serialization;

namespace CrediAvanzaAPI.Response
{
    public class CreditoListadoResponse
    {
        [JsonPropertyName("nombreCliente")]
        public string? NombreCliente { get; set; }

        [JsonPropertyName("nCodCred")]
        public int NCodCred { get; set; }

        [JsonPropertyName("nCodAge")]
        public int NCodAge { get; set; }

        [JsonPropertyName("agencia")]
        public string? Agencia { get; set; }

        [JsonPropertyName("montoSolicitado")]
        public decimal MontoSolicitado { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("nSubProd")]
        public int NSubProd { get; set; }

        [JsonPropertyName("subproducto")]
        public string? SubProducto { get; set; }
    }
}
