using SistemaRespaldo.EN;
using System;
using System.Diagnostics;
using System.IO;

namespace SistemaRespaldo.UI.Escritorio
{
    public static class RespaldoMotor
    {
        // CAMBIO: Ahora recibe 'BaseDatos' en lugar de la clase vieja
        public static (bool exito, string mensaje) GenerarRespaldo(BaseDatos config)
        {
            try
            {
                // Verificamos que la ruta de guardado exista (Configurada en tu JSON)
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);

                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                
                // CAMBIO: Usamos 'config.Nombre'
                string archivoDestino = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{config.Nombre}_{fecha}.sql");

                // --- CONSTRUCCIÓN DINÁMICA DEL COMANDO ---
                string comandoIgnorar = "";

                // CAMBIO: Usamos 'config.EsCompleto'
                if (!config.EsCompleto && !string.IsNullOrEmpty(config.TablasAIgnorar))
                {
                    string[] tablas = config.TablasAIgnorar.Split(',');
                    foreach (string tabla in tablas)
                    {
                        // IMPORTANTE: Mantenemos el formato para mysqldump
                        comandoIgnorar += $" --ignore-table={config.Nombre}.{tabla.Trim()}";
                    }
                }

                // --- AJUSTE PARA LINUX (LUBUNTU) ---
                // Nota: En Linux no usamos 'cmd.exe /c', llamamos directamente a mysqldump o usamos /bin/bash
                // Pero para mantener la compatibilidad con lo que tienes:
                string argumentos = $"-h {ConfiguracionMotor.Servidor} -P {ConfiguracionMotor.Puerto} -u {ConfiguracionMotor.Usuario} -p{ConfiguracionMotor.Password} {config.Nombre} {comandoIgnorar}";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ConfiguracionMotor.RutaMysqlDump, // Ruta directa al binario (/usr/bin/mysqldump)
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