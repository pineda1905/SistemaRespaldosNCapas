using System;
using System.Diagnostics;
using System.IO;

namespace SistemaRespaldo.UI.Escritorio
{
    public static class RespaldoMotor
    {
        // Esta función recibe el nombre de la BD y crea su archivo .sql
        public static bool GenerarRespaldo(string nombreBaseDatos)
        {
            try
            {
                // --- CÓDIGO NUEVO: Verificamos si la carpeta no existe, ¡y la creamos! ---
                if (!Directory.Exists(ConfiguracionMotor.RutaGuardadoRespaldos))
                {
                    Directory.CreateDirectory(ConfiguracionMotor.RutaGuardadoRespaldos);
                }
                // 1. Armamos el nombre del archivo con la fecha de hoy para que no se sobreescriban
                // Ejemplo: campanaoficial_20260430_153000.sql
                string fecha = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string archivoDestino = Path.Combine(ConfiguracionMotor.RutaGuardadoRespaldos, $"{nombreBaseDatos}_{fecha}.sql");

                // 2. Preparamos el comando exacto que le mandaríamos a la consola
                // NOTA IMPORTANTE: En mysqldump, no debe haber espacio entre la -p y el password.
                string argumentos = $"/c \"\"{ConfiguracionMotor.RutaMysqlDump}\" -h {ConfiguracionMotor.Servidor} -P {ConfiguracionMotor.Puerto} -u {ConfiguracionMotor.Usuario} -p{ConfiguracionMotor.Password} {nombreBaseDatos} > \"{archivoDestino}\"\"";

                // 3. Configuramos la consola para que sea INVISIBLE
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = argumentos;
                startInfo.CreateNoWindow = true; // ¡Esta es la clave para que la ventana negra no salte a la vista!
                startInfo.UseShellExecute = false;

                // 4. Ejecutamos el comando y esperamos a que termine
                using (Process proceso = Process.Start(startInfo))
                {
                    proceso.WaitForExit();
                }

                return true; // Si llegó hasta aquí, el respaldo se hizo con éxito
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico al intentar respaldar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}