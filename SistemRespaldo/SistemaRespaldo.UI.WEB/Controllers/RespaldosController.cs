using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SistemaRespaldo.EN;
using SistemaRespaldo.BL;
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.UI.WEB.Controllers
{
    public class RespaldosController : Controller
    {
        // 1. Carga la pantalla visual con los datos necesarios
        public IActionResult Index()
        {
            try
            {
                WebBL negocio = new WebBL();
                
                // Buscamos los logs en la base de datos a través de la BL
                var listaLogs = negocio.ObtenerHistorialLogs(); 

                // Si por alguna razón llega nulo, mandamos una lista vacía para que no explote
                if (listaLogs == null) listaLogs = new List<HistorialLog>();

                // IMPORTANTE: Pasamos la lista a la vista
                return View(listaLogs);
            }
            catch (Exception ex)
            {
                // Loguear el detalle real en el servidor (invisible al usuario)
                Console.WriteLine($"[ERROR] Index: {ex}");
                ViewBag.Error = "Ocurrió un error interno al cargar los registros. Contacte al administrador.";
                return View(new List<HistorialLog>());
            }
        }

        // 2. Se ejecuta al darle clic a "Guardar Horario"
        [HttpPost]
        public IActionResult GuardarHorario(TimeSpan horaRespaldo)
        {
            try
            {
                Horario nuevoHorario = new Horario
                {
                    HoraEjecucion = horaRespaldo
                };

                WebBL negocio = new WebBL();
                bool exito = negocio.GuardarNuevoHorario(nuevoHorario);

                if (exito)
                {
                    TempData["Mensaje"] = "¡Hora de respaldo guardada correctamente en MySQL!";
                }
                else
                {
                    TempData["Error"] = "Hubo un problema al guardar el horario.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GuardarHorario: {ex}");
                TempData["Error"] = "Ocurrió un error interno al guardar el horario. Contacte al administrador.";
                return RedirectToAction("Index");
            }
        }

        // --- Día 12: Descarga el archivo de respaldo asociado a un log ---
        public IActionResult Descargar(int id)
        {
            try
            {
                WebBL negocio = new WebBL();
                HistorialLog log = negocio.ObtenerLogPorId(id);

                if (log == null)
                {
                    TempData["Error"] = "No se encontró el registro solicitado.";
                    return RedirectToAction("Index");
                }

                if (log.Estado != "Exito")
                {
                    TempData["Error"] = "Solo se pueden descargar archivos de respaldos exitosos.";
                    return RedirectToAction("Index");
                }

                // Extraemos la ruta del archivo desde el campo Mensaje
                string rutaArchivo = ExtraerRutaDeArchivo(log.Mensaje);

                if (string.IsNullOrEmpty(rutaArchivo))
                {
                    TempData["Error"] = "No se pudo determinar la ruta del archivo de respaldo.";
                    return RedirectToAction("Index");
                }

                // =====================================================
                // SEGURIDAD: Validación contra Path Traversal (Hallazgo #1)
                // Verificamos que la ruta resuelta esté DENTRO del directorio
                // autorizado de respaldos. Esto previene lectura de archivos
                // arbitrarios como /etc/passwd o C:\Windows\System32.
                // =====================================================
                string rutaBase = ConfiguracionHelper.RutaGuardado;
                string rutaCompleta = Path.GetFullPath(rutaArchivo);
                string rutaBaseCompleta = Path.GetFullPath(rutaBase);

                // Asegurar que la ruta base termine con separador para evitar
                // falsos positivos (ej. /home/alex/Desktop/RespaldosMaliciosos)
                if (!rutaBaseCompleta.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    rutaBaseCompleta += Path.DirectorySeparatorChar;
                }

                if (!rutaCompleta.StartsWith(rutaBaseCompleta, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"[SEGURIDAD] Intento de Path Traversal bloqueado. Ruta solicitada: {rutaCompleta} | Ruta base: {rutaBaseCompleta}");
                    TempData["Error"] = "Acceso denegado: la ruta del archivo no pertenece al directorio de respaldos.";
                    return RedirectToAction("Index");
                }

                // Verificamos que el archivo exista en disco
                if (!System.IO.File.Exists(rutaCompleta))
                {
                    TempData["Error"] = "El archivo de respaldo ya no existe en el disco.";
                    return RedirectToAction("Index");
                }

                // Determinamos el tipo MIME según la extensión
                string nombreArchivo = Path.GetFileName(rutaCompleta);
                string contentType = "application/octet-stream"; // Tipo genérico para descarga

                // Retornamos el archivo físico para que el navegador lo descargue
                return PhysicalFile(rutaCompleta, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Descargar ID {id}: {ex}");
                TempData["Error"] = "Ocurrió un error interno al intentar descargar el archivo. Contacte al administrador.";
                return RedirectToAction("Index");
            }
        }

        // Método auxiliar: Extrae la ruta del archivo desde el texto del mensaje del log
        private string ExtraerRutaDeArchivo(string mensaje)
        {
            if (string.IsNullOrEmpty(mensaje)) return null;

            // Patrón 1: Buscar "en: " seguido de una ruta absoluta
            // Funciona para: "Respaldo exitoso en: /home/alex/..."
            //            y:  "Respaldo MongoDB completado con éxito en: /home/alex/..."
            var match = Regex.Match(mensaje, @"en:\s*(.+)$");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            // Patrón 2: Buscar cualquier ruta absoluta Linux o Windows en el mensaje
            var matchRuta = Regex.Match(mensaje, @"(/[^\s]+\.(sql|archive))|(([A-Z]:\\[^\s]+\.(sql|archive)))");
            if (matchRuta.Success)
            {
                return matchRuta.Value.Trim();
            }

            return null;
        }
    }
}