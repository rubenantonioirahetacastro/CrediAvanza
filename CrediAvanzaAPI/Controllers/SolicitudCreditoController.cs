using CrediAvanzaAPI.Models;
using CrediAvanzaAPI.Request;
using CrediAvanzaAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System;


namespace CrediAvanzaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudCreditoController : ControllerBase
    {
        private readonly ISolicitudCreditoService _service;
        private readonly IBlobStorageService _blobService;

        public SolicitudCreditoController(ISolicitudCreditoService service, IBlobStorageService blobService)
        {
            _service = service;
            _blobService = blobService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Crear([FromForm] SolicitudCreditoRequest request)
        {
            // validation settings
            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            const long maxSizeBytes = 5 * 1024 * 1024; // 5MB

            // Helper to validate and upload a single file
            async Task<string> ValidateAndUploadAsync(IFormFile? file, string folder)
            {
                if (file == null)
                    return null;

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExt.Contains(ext))
                    throw new InvalidOperationException($"Tipo de archivo no permitido: {ext}");

                if (file.Length > maxSizeBytes)
                    throw new InvalidOperationException($"Tamaño de archivo excede el máximo permitido ({maxSizeBytes} bytes)");

                var fileName = $"{Guid.NewGuid()}{ext}";
                using var stream = file.OpenReadStream();
                var relativePath = await _blobService.UploadImageAsync(stream, folder, fileName);
                return relativePath;
            }

            try
            {
                // inside the business service. El sistema de negocio puede mover/renombrar o mantener estas rutas.
                var requestFolder = $"solicitudes/{Guid.NewGuid()}";
                var personaFolder = $"{requestFolder}/personas";
                var garantiaFolder = $"{requestFolder}/garantias";
                var negocioFolder = $"{requestFolder}/negocios";
                var documentacionFolder = $"{requestFolder}/documentacion";

                // Map FotoIds
                List<FotoId>? fotoIds = null;
                if (request.FotoIds != null)
                {
                    fotoIds = new List<FotoId>();
                    foreach (var f in request.FotoIds)
                    {
                        string? path = null;
                        if (f.Archivo != null)
                            path = await ValidateAndUploadAsync(f.Archivo, personaFolder);

                        fotoIds.Add(new FotoId
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto ?? string.Empty,
                            NTipoFoto = f.NTipoFoto,
                            IdPersona = f.IdPersona
                        });
                    }
                }

                // Map FotoDocumentacions
                List<FotoDocumentacion>? fotoDocs = null;
                if (request.FotoDocumentacions != null)
                {
                    fotoDocs = new List<FotoDocumentacion>();
                    foreach (var f in request.FotoDocumentacions)
                    {
                        string? path = null;
                        if (f.Archivo != null)
                            path = await ValidateAndUploadAsync(f.Archivo, documentacionFolder);

                        fotoDocs.Add(new FotoDocumentacion
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto,
                            IdTipoDocumentacion = f.IdTipoDocumentacion,
                            IdDocumentacion = f.IdDocumentacion
                        });
                    }
                }

                // Map GarantiaFotos
                List<GarantiaFoto>? garantiaFotos = null;
                if (request.GarantiaFotos != null)
                {
                    garantiaFotos = new List<GarantiaFoto>();
                    foreach (var f in request.GarantiaFotos)
                    {
                        string? path = null;
                        if (f.Archivo != null)
                            path = await ValidateAndUploadAsync(f.Archivo, garantiaFolder);

                        garantiaFotos.Add(new GarantiaFoto
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto ?? string.Empty,
                            NValor = f.NValor,
                            IdArticuloGarantia = f.IdArticuloGarantia,
                            IdGarantia = f.IdGarantia
                        });
                    }
                }

                // Map FotoNegocios
                List<FotoNegocio>? fotoNegocios = null;
                if (request.FotoNegocios != null)
                {
                    fotoNegocios = new List<FotoNegocio>();
                    foreach (var f in request.FotoNegocios)
                    {
                        string? path = null;
                        if (f.Archivo != null)
                            path = await ValidateAndUploadAsync(f.Archivo, negocioFolder);

                        fotoNegocios.Add(new FotoNegocio
                        {
                            IdFoto = f.IdFoto,
                            VFoto = path ?? f.VFoto,
                            NTipoFoto = f.NTipoFoto,
                            IdNegocio = f.IdNegocio
                        });
                    }
                }

                var filas = await _service.CrearSolicitudAsync(
                    fotoIds,
                    fotoDocs,
                    garantiaFotos,
                    fotoNegocios,
                    request.Persona,
                    request.Conyuge,
                    request.Fiador,
                    request.Negocio,
                    request.CapacidadPago,
                    request.Compra,
                    request.Venta,
                    request.Credito
                );

                return Ok(new { filasAfectadas = filas });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
