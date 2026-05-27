namespace CrediAvanzaAPI.Helpers
{
    public static class EmailTemplates
    {
        public static string SolicitudCredito(
            string nombre,
            int token)
        {
            return $@"
            <h2>Solicitud de crédito recibida</h2>

            <p>Hola {nombre}, hemos recibido correctamente tu solicitud de crédito.</p>

            <p>Tu código de verificación es:</p>

            <h1 style='color:#0d6efd'>{token}</h1>

            <p>Uno de nuestros asesores se pondrá en contacto contigo próximamente.</p>

            <p>Tu solicitud quedará en espera de autorización.</p>

            <br>

            <p><b>CrediAvanza</b></p>
            ";
        }
    }
}