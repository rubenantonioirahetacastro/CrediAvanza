using CrediAvanzaAPI.Models;
using System;
using System.Runtime.CompilerServices;

namespace CrediAvanzaAPI.Helpers
{
    public class ErrorLogger
    {
        private readonly DbNegocioContext _context;
        public ErrorLogger(DbNegocioContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            Exception ex,
            [CallerMemberName] string metodo = "",
            [CallerFilePath] string archivo = "")
        {
            try
            {
                var origen = $"{Path.GetFileNameWithoutExtension(archivo)}.{metodo}";

                _context.LogErrores.Add(new LogErrore
                {
                    Origen = origen,
                    Mensaje = ex.InnerException?.Message ?? ex.Message,
                    StackTrace = ex.StackTrace,
                    TipoExcepcion = ex.GetType().Name,
                    FechaError = DateTime.Now
                });

                await _context.SaveChangesAsync();
            }
            catch
            {
                Console.WriteLine($"Fallo log BD: {ex.Message}");
            }
        }
    }

}