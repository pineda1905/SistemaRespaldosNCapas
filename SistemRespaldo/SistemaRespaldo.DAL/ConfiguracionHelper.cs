using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SistemaRespaldo.DAL
{
    public static class ConfiguracionHelper
    {
        public static string CadenaConexion { get; private set; }
        public static string RutaMysqlDump { get; private set; }
        public static string RutaGuardado { get; private set; }

        static ConfiguracionHelper()
        {
            // Detectamos si existe appsettings.json (Web) o config.json (Escritorio)
            string nombreArchivo = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
                                   ? "appsettings.json"
                                   : "config.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(nombreArchivo, optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            // Extraemos los datos (la estructura es idéntica en ambos JSON)
            string servidor = config["ConfiguracionServidor:Servidor"];
            string puerto = config["ConfiguracionServidor:Puerto"];
            string usuario = config["ConfiguracionServidor:Usuario"];
            string password = config["ConfiguracionServidor:Password"];
            string bd = config["ConfiguracionServidor:BaseDatosConfig"];

            CadenaConexion = $"Server={servidor};Port={puerto};Database={bd};Uid={usuario};Pwd={password};";
            RutaMysqlDump = config["Rutas:RutaMysqlDump"];
            RutaGuardado = config["Rutas:RutaGuardadoRespaldos"];
        }
    }
}