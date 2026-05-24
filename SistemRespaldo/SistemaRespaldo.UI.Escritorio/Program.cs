using System;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;
using SistemaRespaldo.UI.Escritorio;

namespace SistemaRespaldo.UI.Escritorio
{
    internal static class Program
    {
        static void Main()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("🚀 EJECUTANDO MOTOR DE RESPALDO (LINUX)");
            Console.WriteLine("==========================================");

            try 
            {
                // 1. Configuramos la prueba manual para ver si genera el archivo
                BaseDatos configPrueba = new BaseDatos {
                    Nombre = "SistemaRespaldos", // Nombre de tu base en MySQL
                    EsCompleto = true,
                    TablasAIgnorar = ""
                };

                Console.WriteLine($"[*] Procesando base de datos: {configPrueba.Nombre}");

                // 2. Llamamos al motor que ya tiene las rutas de Linux
                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);

                // 3. Resultado en pantalla
                if (resultado.exito) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✅ ¡RESPALDO GENERADO CON ÉXITO!");
                    Console.WriteLine($"[+] {resultado.mensaje}");
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n❌ ERROR EN EL PROCESO");
                    Console.WriteLine($"[-] Detalle: {resultado.mensaje}");
                }
                Console.ResetColor();

                // 4. Guardamos el historial en MySQL para que lo veas en la Web
                ConsultasDAL dal = new ConsultasDAL();
                dal.InsertarLog(new HistorialLog {
                    BaseDeDatos = configPrueba.Nombre,
                    Estado = resultado.exito ? "Exito" : "Error",
                    Mensaje = resultado.mensaje
                });
                Console.WriteLine("[*] Registro guardado en el historial de MySQL.");

            } 
            catch (Exception ex) 
            {
                Console.WriteLine("\n FALLO CRÍTICO:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\n==========================================");
            Console.WriteLine("🏁 Fin del programa. Revisa tu carpeta 'Respaldos'.");
        }
    }
}