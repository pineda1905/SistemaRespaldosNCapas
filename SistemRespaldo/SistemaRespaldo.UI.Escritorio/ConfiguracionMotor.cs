using System;
using Microsoft.Extensions.Configuration;

namespace SistemaRespaldo.UI.Escritorio
{
    public static class ConfiguracionMotor
    {
        // Credenciales que necesita el mysqldump
        public static string Servidor { get; private set; }
        public static string Puerto { get; private set; }
        public static string Usuario { get; private set; }
        public static string Password { get; private set; }

        // Rutas
        public static string RutaMysqlDump { get; private set; }
        public static string RutaGuardadoRespaldos { get; private set; }

        static ConfiguracionMotor()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("config.json", optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();

                // 1. Extraemos las credenciales para el comando
                Servidor = config["ConfiguracionServidor:Servidor"];
                Puerto = config["ConfiguracionServidor:Puerto"];
                Usuario = config["ConfiguracionServidor:Usuario"];
                Password = config["ConfiguracionServidor:Password"];

                // 2. Extraemos las rutas
                RutaMysqlDump = config["Rutas:RutaMysqlDump"];
                RutaGuardadoRespaldos = config["Rutas:RutaGuardadoRespaldos"];
            }
            catch (Exception ex)
            {
                throw new Exception("Error al leer el archivo config.json: " + ex.Message);
            }
        }
    }
}