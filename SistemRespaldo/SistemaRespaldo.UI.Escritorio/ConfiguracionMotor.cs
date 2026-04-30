using System;
using Microsoft.Extensions.Configuration;

namespace SistemaRespaldo.UI.Escritorio
{
    public static class ConfiguracionMotor
    {
        public static string RutaMysqlDump { get; private set; }
        public static string RutaGuardadoRespaldos { get; private set; }

        // Este constructor se ejecuta solo la primera vez que llames a la clase
        static ConfiguracionMotor()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();

                // Leemos específicamente el bloque de "Rutas" del JSON
                RutaMysqlDump = config["Rutas:RutaMysqlDump"];
                RutaGuardadoRespaldos = config["Rutas:RutaGuardadoRespaldos"];
            }
            catch (Exception ex)
            {
                // Si hay un error (ej. falta el archivo), lanzamos una excepción clara
                throw new Exception("Error al leer el archivo config.json: " + ex.Message);
            }
        }
    }
}