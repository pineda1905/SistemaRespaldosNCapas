using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms; // Necesario para MessageBox si decides dejarlo, aunque lo ideal es el log

namespace SistemaRespaldo.UI.Escritorio
{
    public static class RespaldoMotor
    {
        // --- CORRECCIÓN DÍA 6: Cambiamos de 'bool' a una Tupla '(bool exito, string mensaje)' ---
        // Esto permite que el motor devuelva no solo si funcionó, sino también el mensaje de error
        public static (bool exito, string mensaje) GenerarRespaldo(string nombreBaseDatos)
        {
            try
            {
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                {
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);
                }

                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string archivoDestino = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{nombreBaseDatos}_{fecha}.sql");

                // Preparamos el comando
                string argumentos = $"/c \"\"{ConfiguracionMotor.RutaMysqlDump}\" -h {ConfiguracionMotor.Servidor} -P {ConfiguracionMotor.Puerto} -u {ConfiguracionMotor.Usuario} -p{ConfiguracionMotor.Password} {nombreBaseDatos} > \"{archivoDestino}\"\"";

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = argumentos;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;

                // --- NUEVO: Redirigimos el error para capturar qué dice MySQL si algo falla ---
                startInfo.RedirectStandardError = true;

                using (Process proceso = Process.Start(startInfo))
                {
                    // Leemos el error de la consola (si es que hubo uno)
                    string errorCapturado = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    // Si el proceso termina con un código distinto a 0, hubo un error en mysqldump
                    if (proceso.ExitCode != 0)
                    {
                        return (false, "Error de MySQL: " + errorCapturado);
                    }
                }

                // Si todo sale bien, devolvemos true y un mensaje de éxito
                return (true, "Respaldo completado con éxito");
            }
            catch (Exception ex)
            {
                // En lugar de un MessageBox que detiene el motor, devolvemos el error para el Log
                return (false, "Error crítico: " + ex.Message);
            }
        }
    }
}