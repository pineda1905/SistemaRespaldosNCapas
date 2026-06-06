using SistemaRespaldo.EN;
using System;
using System.Diagnostics;
using System.IO;

namespace SistemaRespaldo.UI.Escritorio
{
    public static class RespaldoMotor
    {
        // Sobrecarga para compatibilidad con BaseDatos (usado en pruebas directas)
        public static (bool exito, string mensaje) GenerarRespaldo(BaseDatos db)
        {
            var config = new ConfiguracionRespaldo
            {
                NombreBaseDatos = db.Nombre,
                TipoRespaldoCompletoOParcial = db.EsCompleto,
                TablasAIgnorar = db.TablasAIgnorar,
                TipoMotor = db.TipoMotor,
                CadenaConexion = db.CadenaConexion
            };
            return GenerarRespaldo(config);
        }

        // Método principal usado por el semáforo del timer
        public static (bool exito, string mensaje) GenerarRespaldo(ConfiguracionRespaldo config)
        {
            try
            {
                // Verificamos que la ruta de guardado exista (Configurada en tu JSON)
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);

                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string archivoDestino = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{config.NombreBaseDatos}_{fecha}.sql");

                // --- CONSTRUCCIÓN DINÁMICA DEL COMANDO ---
                string comandoIgnorar = "";

                if (!config.TipoRespaldoCompletoOParcial && !string.IsNullOrEmpty(config.TablasAIgnorar))
                {
                    string[] tablas = config.TablasAIgnorar.Split(',');
                    foreach (string tabla in tablas)
                    {
                        // IMPORTANTE: Mantenemos el formato para mysqldump
                        comandoIgnorar += $" --ignore-table={config.NombreBaseDatos}.{tabla.Trim()}";
                    }
                }

                string argumentos = $"-h {ConfiguracionMotor.Servidor} -P {ConfiguracionMotor.Puerto} -u {ConfiguracionMotor.Usuario} -p{ConfiguracionMotor.Password} {config.NombreBaseDatos} {comandoIgnorar}";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ConfiguracionMotor.RutaMysqlDump, // Ruta al binario mysqldump
                    Arguments = $"{argumentos} --result-file=\"{archivoDestino}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                };

                using (Process proceso = Process.Start(startInfo))
                {
                    string errorCapturado = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    if (proceso.ExitCode != 0)
                        return (false, "Error MySQL: " + errorCapturado);
                }

                return (true, "Respaldo exitoso en: " + archivoDestino);
            }
            catch (Exception ex)
            {
                return (false, "Error Crítico: " + ex.Message);
            }
        }
    }
}