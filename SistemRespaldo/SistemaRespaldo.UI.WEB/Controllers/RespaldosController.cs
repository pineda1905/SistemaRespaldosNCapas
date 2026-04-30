using Microsoft.AspNetCore.Mvc;
using System;
using SistemaRespaldo.EN;
using SistemaRespaldo.BL;

namespace SistemaRespaldo.UI.WEB.Controllers
{
    public class RespaldosController : Controller
    {
        // 1. Carga la pantalla visual
        public IActionResult Index()
        {
            return View();
        }

        // 2. Se ejecuta al darle clic a "Guardar Horario"
        [HttpPost]
        public IActionResult GuardarHorario(TimeSpan horaRespaldo)
        {
            try
            {
                // 1. Llenamos tu Entidad exacta
                Horario nuevoHorario = new Horario
                {
                    HoraEjecucion = horaRespaldo
                };

                // 2. Usamos tu BL exclusiva para la Web
                WebBL negocio = new WebBL();
                bool exito = negocio.GuardarNuevoHorario(nuevoHorario);

                // 3. Verificamos el resultado
                if (exito)
                {
                    TempData["Mensaje"] = "¡Hora de respaldo guardada correctamente en MySQL!";
                }
                else
                {
                    TempData["Error"] = "Hubo un problema al guardar el horario.";
                }

                // Recarga la página para mostrar el mensaje
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