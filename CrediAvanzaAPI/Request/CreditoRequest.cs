namespace CrediAvanzaAPI.Request
{
    public class CreditoRequest
    {
        public decimal nPrestamo { get; set; }     
        public int nProd { get; set; }      
        public int nSubProd { get; set; }   
        public int nTipoGasto { get; set; }     
        public int nPeriodo { get; set; }      
        public int nTipoCargo { get; set; }    
        public int nCobroEnAgencia { get; set; } 
        public int nCodCred { get; set; }

        public int nCodAge { get; set; }
    }
}
