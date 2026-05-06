using System;
using MySqlConnector;
using SistemaRespaldo.EN;
// IMPORTANTE: Agregamos la referencia a la DAL para usar el Helper
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // ELIMINAMOS la línea de la contraseña "anderson" o "base1234"
        // Ahora usamos ConfiguracionHelper.CadenaConexion

        public bool GuardarHorario(Horario horario)
        {
            try
            {
                // Ahora el código es universal. 
                // Si Alex tiene su JSON con su clave, le funcionará a él.
                // Si tú tienes el tuyo, te funcionará a ti.
                using (MySqlConnection conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string query = "INSERT INTO Horarios (HoraEjecucion) VALUES (@hora)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hora", horario.HoraEjecucion);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en Base de Datos: " + ex.Message);
            }
        }
    }
}