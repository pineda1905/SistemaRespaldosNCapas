using System;
using System.Collections.Generic; // IMPORTANTE: Para poder usar List<>
using MySqlConnector;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // --- MÉTODO EXISTENTE ---
        public bool GuardarHorario(Horario horario)
        {
            try
            {
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

        // --- NUEVO MÉTODO: LECTURA DE CONFIGURACIONES INCLUYENDO MONGODB ---
        public List<ConfiguracionRespaldo> ObtenerBasesDeDatosActualizado()
        {
            List<ConfiguracionRespaldo> lista = new List<ConfiguracionRespaldo>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    // Seleccionamos todas las columnas, incluyendo las dos nuevas del Sprint de Mongo
                    string query = "SELECT NombreBaseDatos, TipoRespaldoCompletoOParcial, TablasAIgnorar, TipoMotor, CadenaConexion FROM configuracionrespaldos";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader lector = cmd.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                var config = new ConfiguracionRespaldo
                                {
                                    NombreBaseDatos = lector["NombreBaseDatos"].ToString(),
                                    TipoRespaldoCompletoOParcial = Convert.ToBoolean(lector["TipoRespaldoCompletoOParcial"]),
                                    TablasAIgnorar = lector["TablasAIgnorar"]?.ToString(),

                                    // Mapeo de las nuevas columnas. Si TipoMotor llega nulo por algún motivo, se asigna "MySQL" por defecto
                                    TipoMotor = lector["TipoMotor"]?.ToString() ?? "MySQL",
                                    CadenaConexion = lector["CadenaConexion"]?.ToString()
                                };
                                lista.Add(config);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las configuraciones de respaldo: " + ex.Message);
            }
            return lista;
        }
    }
}