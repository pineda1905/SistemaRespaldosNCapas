using System;
using MySqlConnector; // Asegúrate de tener instalado el paquete NuGet de MySqlConnector en la DAL
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // NOTA DE MENTOR: Cambia "tu_contraseña" por la clave de tu MySQL local.
        private string connectionString = "Server=localhost;Database=SistemaRespaldos;Uid=root;Pwd=anderson;";

        public bool GuardarHorario(Horario horario)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    // Usamos @hora para evitar inyecciones SQL (buenas prácticas)
                    string query = "INSERT INTO Horarios (HoraEjecucion) VALUES (@hora)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Pasamos el TimeSpan directamente, MySqlConnector lo entiende perfecto
                        cmd.Parameters.AddWithValue("@hora", horario.HoraEjecucion);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0; // Si afectó más de 0 filas, fue un éxito
                    }
                }
            }
            catch (Exception ex)
            {
                // Si la BD falla, lanzamos el error para que la Web lo muestre en rojo
                throw new Exception("Error en Base de Datos: " + ex.Message);
            }
        }
    }
}