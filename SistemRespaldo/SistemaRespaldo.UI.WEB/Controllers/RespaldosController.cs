using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic; // Necesario para List<>
using System.Text.RegularExpressions;
using SistemaRespaldo.EN;
using SistemaRespaldo.BL;

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
                // En caso de error, mandamos una lista vacía y el mensaje de error
                ViewBag.Error = "No se pudieron cargar los logs: " + ex.Message;
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
                TempData["Error"] = "Error del sistema: " + ex.Message;
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
                    TempData["Error"] = "No se encontró el registro de log con ID: " + id;
                    return RedirectToAction("Index");
                }

                if (log.Estado != "Exito")
                {
                    TempData["Error"] = "Solo se pueden descargar archivos de respaldos exitosos.";
                    return RedirectToAction("Index");
                }

                // Extraemos la ruta del archivo desde el campo Mensaje
                // El formato del mensaje es: "Respaldo exitoso en: /ruta/al/archivo.sql"
                // o "Respaldo MongoDB completado con éxito en: /ruta/al/archivo.archive"
                string rutaArchivo = ExtraerRutaDeArchivo(log.Mensaje);

                if (string.IsNullOrEmpty(rutaArchivo))
                {
                    TempData["Error"] = "No se pudo determinar la ruta del archivo desde el mensaje del log.";
                    return RedirectToAction("Index");
                }

                // Verificamos que el archivo exista en disco
                if (!System.IO.File.Exists(rutaArchivo))
                {
                    TempData["Error"] = $"El archivo ya no existe en el disco: {rutaArchivo}";
                    return RedirectToAction("Index");
                }

                // Determinamos el tipo MIME según la extensión
                string nombreArchivo = Path.GetFileName(rutaArchivo);
                string contentType = "application/octet-stream"; // Tipo genérico para descarga

                // Retornamos el archivo físico para que el navegador lo descargue
                return PhysicalFile(rutaArchivo, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al intentar descargar: " + ex.Message;
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