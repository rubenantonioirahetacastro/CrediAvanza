namespace CrediAvanzaAPI.Helpers
{
    public static class EmailTemplates
    {
        public static string SolicitudCredito(
            string nombre,
            int token,
            string passwordTemporal)
        {
            return $@"

                <p>Estimado(a) <b>{nombre}</b>,</p>

                <p>
                    Reciba un cordial saludo de <b>CrediAvanza</b>.
                    Le informamos que su solicitud de crédito ha sido registrada exitosamente en nuestro sistema.
                </p>

                <p>
                    Actualmente, su solicitud se encuentra en proceso de análisis por parte de nuestro equipo de evaluación,
                    quien revisará la información proporcionada para determinar la aprobación correspondiente.
                </p>

                <p>
                    Como parte del proceso, hemos generado sus credenciales de acceso al sistema:
                </p>

                <p><b>Contraseña temporal:</b></p>
                <h2 style='color:#000000'>{passwordTemporal}</h2>

                <p>
                    El usuario para iniciar sesión en la plataforma corresponde a su número de documento registrado.
                </p>

                <p>
                    Le recomendamos ingresar a la plataforma con esta contraseña y proceder a su cambio inmediato
                    por motivos de seguridad.
                </p>

                <p>
                    En caso de requerir información adicional, uno de nuestros asesores se pondrá en contacto con usted
                    en los próximos días hábiles para dar seguimiento a su solicitud.
                </p>

                <p>
                    Agradecemos su confianza.
                </p>

                <p><b>Atentamente,</b><br>Equipo CrediAvanza</p>

                <br>
                <hr>

                <p style='font-size:11px; color:#666; line-height:1.4;'>
                    <b>NOTA CONFIDENCIAL:</b> La información contenida en este correo electrónico es confidencial y de uso exclusivo del destinatario.
                    Si usted ha recibido este mensaje por error, queda estrictamente prohibida su divulgación, copia o distribución.
                    Por favor elimínelo de inmediato y notifíquelo al remitente.
                    Este correo no constituye una firma electrónica válida ni un compromiso legal vinculante.
                </p>

                <p style='font-size:11px; color:#666; line-height:1.4;'>
                    <b>CONFIDENTIAL NOTICE:</b> This email and any attachments may contain confidential and privileged information intended solely
                    for the recipient. If you are not the intended recipient, any disclosure, copying, or distribution is strictly prohibited.
                    Please notify the sender immediately and delete this message.
                </p>
            ";
        }
    }
}