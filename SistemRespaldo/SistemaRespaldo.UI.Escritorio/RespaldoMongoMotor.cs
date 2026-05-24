using System;
using System.Diagnostics;
using System.IO;
using SistemaRespaldo.EN;

namespace SistemaRespaldo.BL
{
    public class RespaldoMongoMotor
    {
        public static (bool exito, string mensaje) GenerarRespaldo(ConfiguracionRespaldo config)
        {
            try
            {
                // Usamos la ruta de guardado del config.json (la misma carpeta para MySQL y Mongo)
                string rutaGuardado = SistemaRespaldo.UI.Escritorio.ConfiguracionMotor.RutaGuardadoRespaldos;

                if (!Directory.Exists(rutaGuardado))
                    Directory.CreateDirectory(rutaGuardado);

                // Generamos un archivo .archive con nombre descriptivo
                string rutaSalida = Path.Combine(rutaGuardado, $"Mongo_{config.NombreBaseDatos}_{DateTime.Now:yyyyMMdd_HHmmss}.archive");

                // Armamos el comando con la URI y --archive
                string argumentos = $"--uri=\"{config.CadenaConexion}\" --archive=\"{rutaSalida}\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "mongodump",
                    Arguments = argumentos,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process proceso = Process.Start(psi))
                {
                    string errorCapturado = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    if (proceso.ExitCode == 0)
                        return (true, "Respaldo MongoDB completado con éxito en: " + rutaSalida);
                    else
                        return (false, "Error mongodump. Código: " + proceso.ExitCode + " - " + errorCapturado);
                }
            }
            catch (Exception ex)
            {
                return (false, "Error fatal en Mongo: " + ex.Message);
            }
        }
    }
}