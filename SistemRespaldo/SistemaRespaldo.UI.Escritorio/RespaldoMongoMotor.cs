using System;
using System.Diagnostics;
using System.IO;
using SistemaRespaldo.EN;
using SistemaRespaldo.UI.Escritorio;

namespace SistemaRespaldo.BL
{
    public class RespaldoMongoMotor
    {
        public static (bool exito, string mensaje) GenerarRespaldo(BaseDatos config)
        {
            try
            {
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                {
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);
                }

                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string rutaSalida = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{config.Nombre}_{fecha}.gz");

                // Construimos los argumentos para ejecutar mongodump localmente para la base de datos especificada.
                // Usamos --db para especificar la base de datos, --archive para guardar en un único archivo y --gzip para comprimir.
                string argumentos = $"--db {config.Nombre} --archive=\"{rutaSalida}\" --gzip";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ConfiguracionMotor.RutaMongoDump,
                    Arguments = argumentos,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                };

                using (Process proceso = Process.Start(psi))
                {
                    if (proceso == null)
                    {
                        return (false, "No se pudo iniciar el proceso mongodump.");
                    }

                    string errorCapturado = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    if (proceso.ExitCode == 0)
                    {
                        return (true, "Respaldo MongoDB completado con éxito. Archivo: " + rutaSalida);
                    }
                    else
                    {
                        return (false, $"Error mongodump. Código: {proceso.ExitCode}. Detalle: {errorCapturado}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Error fatal en Mongo: " + ex.Message);
            }
        }
    }
}