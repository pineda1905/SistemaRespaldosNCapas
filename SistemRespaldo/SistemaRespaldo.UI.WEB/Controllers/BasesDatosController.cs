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
                // Loguear el detalle real en el servidor (invisible al usuario)
                Console.WriteLine($"[ERROR] BasesDatos/Index: {ex}");
                ViewBag.Error = "Ocurrió un error interno al cargar los datos. Contacte al administrador.";
                return View(new System.Collections.Generic.List<BaseDatos>());
            }
        }

        // 2. Procesar el formulario cuando le des al botón "Registrar"
        [HttpPost]
        public IActionResult Guardar(string nombreBaseDatos, string tipoRespaldo, string tablasIgnorar, string tipoMotor, string cadenaConexion)
        {
            try
            {
                BaseDatos nuevaDb = new BaseDatos { 
                    Nombre = nombreBaseDatos,
                    EsCompleto = (tipoRespaldo == "Completo"),
                    TablasAIgnorar = tablasIgnorar ?? "",
                    TipoMotor = tipoMotor ?? "MySQL",
                    CadenaConexion = cadenaConexion ?? ""
                };
                
                bl.GuardarBaseDatos(nuevaDb);
                TempData["Mensaje"] = "¡Configuración guardada correctamente!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] BasesDatos/Guardar: {ex}");
                TempData["Error"] = "Ocurrió un error interno al guardar la configuración. Contacte al administrador.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Eliminar(int id)
        {
            try
            {
                bl.EliminarBaseDatos(id);
                TempData["Mensaje"] = "Base de datos eliminada del monitoreo.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] BasesDatos/Eliminar ID {id}: {ex}");
                TempData["Error"] = "No se pudo eliminar la configuración. Contacte al administrador.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Descargar(int id)
        {
            try
            {
                var db = bl.ObtenerBaseDatosPorId(id);
                if (db == null)
                {
                    TempData["Error"] = "No se encontró la configuración de la base de datos.";
                    return RedirectToAction("Index");
                }

                string extension = db.TipoMotor == "MongoDB" ? "gz" : "sql";
                string mimeType = db.TipoMotor == "MongoDB" ? "application/gzip" : "application/sql";
                string patron = $"{db.Nombre}_*.{extension}";

                // Obtenemos la ruta de guardado
                string rutaGuardado = DAL.ConfiguracionHelper.RutaGuardado;
                if (!System.IO.Directory.Exists(rutaGuardado))
                {
                    TempData["Error"] = $"La ruta de guardado de respaldos no existe: {rutaGuardado}";
                    return RedirectToAction("Index");
                }

                string[] archivos = System.IO.Directory.GetFiles(rutaGuardado, patron);
                if (archivos == null || archivos.Length == 0)
                {
                    TempData["Error"] = $"No se encontró ningún archivo de respaldo para {db.Nombre} (Tipo: {db.TipoMotor}, Extensión: .{extension})";
                    return RedirectToAction("Index");
                }

                // Buscamos el archivo más reciente (último modificado)
                string ultimoArchivo = System.Linq.Enumerable.First(
                    System.Linq.Enumerable.OrderByDescending(
                        System.Linq.Enumerable.Select(archivos, f => new System.IO.FileInfo(f)),
                        fi => fi.LastWriteTime
                    )
                ).FullName;

                byte[] fileBytes = System.IO.File.ReadAllBytes(ultimoArchivo);
                string fileName = System.IO.Path.GetFileName(ultimoArchivo);

                return File(fileBytes, mimeType, fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al descargar respaldo: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
