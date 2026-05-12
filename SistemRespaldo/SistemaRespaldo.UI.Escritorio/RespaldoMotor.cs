using SistemaRespaldo.EN;
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
        public static (bool exito, string mensaje) GenerarRespaldo(ConfiguracionRespaldo config)
        {
            try
            {
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);

                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string archivoDestino = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{config.NombreBaseDatos}_{fecha}.sql");

                // --- DÍA 7: CONSTRUCCIÓN DINÁMICA DEL COMANDO ---
                string comandoIgnorar = "";

                // Si el respaldo NO es completo (es parcial) y hay tablas para ignorar
                if (!config.TipoRespaldoCompletoOParcial && !string.IsNullOrEmpty(config.TablasAIgnorar))
                {
                    // Alex guarda las tablas separadas por coma (ej: "usuarios,logs,temp")
                    string[] tablas = config.TablasAIgnorar.Split(',');
                    foreach (string tabla in tablas)
                    {
                        // Formato: --ignore-table=base_datos.tabla
                        comandoIgnorar += $" --ignore-table={config.NombreBaseDatos}.{tabla.Trim()}";
                    }
                }

                // Armamos los argumentos finales incluyendo los ignorados
                string argumentos = $"/c \"\"{ConfiguracionMotor.RutaMysqlDump}\" -h {ConfiguracionMotor.Servidor} -P {ConfiguracionMotor.Puerto} -u {ConfiguracionMotor.Usuario} -p{ConfiguracionMotor.Password} {config.NombreBaseDatos} {comandoIgnorar} > \"{archivoDestino}\"\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = argumentos,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                };

                using (Process proceso = Process.Start(startInfo))
                {
                    string errorCapturado = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    // --- DÍA 8: CONEXIÓN DE RESULTADO ---
                    if (proceso.ExitCode != 0)
                        return (false, "Error MySQL: " + errorCapturado);
                }

                return (true, "Respaldo exitoso");
            }
            catch (Exception ex)
            {
                return (false, "Error Crítico: " + ex.Message);
            }
        }
    }
}