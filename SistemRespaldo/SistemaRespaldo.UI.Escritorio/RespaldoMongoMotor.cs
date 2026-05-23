using System;
using System.Diagnostics;
using SistemaRespaldo.EN;

namespace SistemaRespaldo.BL
{
    public class RespaldoMongoMotor
    {
        public static (bool exito, string mensaje) GenerarRespaldo(ConfiguracionRespaldo config)
        {
            try
            {
                // Usamos la extensión .archive para que sea 1 solo archivo
                string rutaSalida = $@"C:\RespaldosMySQL\Mongo_{config.NombreBaseDatos}_{DateTime.Now:yyyyMMdd_HHmmss}.archive";

                // Armamos el comando con la URI
                string argumentos = $"--uri=\"{config.CadenaConexion}\" --archive=\"{rutaSalida}\"";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "mongodump",
                    Arguments = argumentos,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process proceso = Process.Start(psi))
                {
                    proceso.WaitForExit();
                    if (proceso.ExitCode == 0)
                        return (true, "Respaldo MongoDB completado con éxito");
                    else
                        return (false, "Error mongodump. Código: " + proceso.ExitCode);
                }
            }
            catch (Exception ex)
            {
                return (false, "Error fatal en Mongo: " + ex.Message);
            }
        }
    }
}