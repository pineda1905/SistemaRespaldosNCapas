using Microsoft.AspNetCore.Mvc;
using SistemaRespaldo.BL;
using SistemaRespaldo.EN;
using System;

namespace SistemaRespaldo.UI.WEB.Controllers
{
    public class BasesDatosController : Controller
    {
        private WebBL bl = new WebBL();

        // 1. Mostrar la página con la lista actual
        public IActionResult Index()
        {
            try
            {
                var lista = bl.ObtenerBasesDatos();
                return View(lista);
            }
            catch (Exception ex)
            {
                // Si falla la conexión a MySQL, enviamos el error a la pantalla
                ViewBag.Error = "No se pudieron cargar los datos: " + ex.Message;
                return View(new System.Collections.Generic.List<BaseDatos>());
            }
        }

        // 2. Procesar el formulario cuando le des al botón "Registrar"
        [HttpPost]
        public IActionResult Guardar(string nombreBaseDatos)
        {
            try
            {
                BaseDatos nuevaDb = new BaseDatos { Nombre = nombreBaseDatos };
                bl.GuardarBaseDatos(nuevaDb);
                TempData["Mensaje"] = "¡Base de datos registrada con éxito!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al registrar: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}