using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic; // Necesario para List<>
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
                // Nota: Asegúrate de que este método exista en tu WebBL
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
    }
}