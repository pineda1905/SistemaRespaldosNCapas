using System;
using System.Collections.Generic;
using MySqlConnector; // Usamos exclusivamente la librería moderna
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class ConsultasDAL
    {
        // Función 1: Leer las bases de datos a respaldar
        public List<ConfiguracionRespaldo> ObtenerBasesDeDatos()
        {
            List<ConfiguracionRespaldo> listaBD = new List<ConfiguracionRespaldo>();

            // ¡Aplicamos tu lector de JSON!
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "SELECT ID, NombreBaseDatos, TipoRespaldoCompletoOParcial, TablasAIgnorar FROM ConfiguracionRespaldos";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaBD.Add(new ConfiguracionRespaldo
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            NombreBaseDatos = reader["NombreBaseDatos"].ToString(),
                            TipoRespaldoCompletoOParcial = Convert.ToBoolean(reader["TipoRespaldoCompletoOParcial"]),
                            TablasAIgnorar = reader["TablasAIgnorar"] != DBNull.Value ? reader["TablasAIgnorar"].ToString() : null
                        });
                    }
                }
            }
            return listaBD;
        }

        // Función 2: Leer los horarios
        public List<Horario> ObtenerHorarios()
        {
            List<Horario> listaHorarios = new List<Horario>();

            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "SELECT ID, HoraEjecucion FROM Horarios";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaHorarios.Add(new Horario
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            HoraEjecucion = (TimeSpan)reader["HoraEjecucion"]
                        });
                    }
                }
            }
            return listaHorarios;
        }

        // Función 3: GUARDAR un nuevo horario (Tu aporte)
        public bool InsertarHorario(TimeSpan horaEjecucion)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO Horarios (HoraEjecucion) VALUES (@hora)";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@hora", horaEjecucion);
                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        // Función 4: GUARDAR una nueva Base de Datos a respaldar (Tu aporte)
        public bool InsertarBaseDeDatos(string nombreBd, bool esCompleto, string tablasIgnorar)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                conexion.Open();
                // Ajustado para coincidir con la tabla que lee Anderson
                string query = "INSERT INTO ConfiguracionRespaldos (NombreBaseDatos, TipoRespaldoCompletoOParcial, TablasAIgnorar) " +
                               "VALUES (@nombre, @completo, @tablas)";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", nombreBd);
                    comando.Parameters.AddWithValue("@completo", esCompleto);
                    comando.Parameters.AddWithValue("@tablas", string.IsNullOrEmpty(tablasIgnorar) ? (object)DBNull.Value : tablasIgnorar);

                    int filasAfectadas = comando.ExecuteNonQuery();
                    return filasAfectadas > 0;
                }
            }
        }

        //Metodo que usará el motor para "escribir en el diario" de la base de datos.
        public bool InsertarLog(HistorialLog log)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "INSERT INTO HistorialLogs (BaseDeDatos, Estado, Mensaje) VALUES (@db, @estado, @msj)";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@db", log.BaseDeDatos);
                comando.Parameters.AddWithValue("@estado", log.Estado);
                comando.Parameters.AddWithValue("@msj", log.Mensaje);

                conexion.Open();
                return comando.ExecuteNonQuery() > 0;
            }
        }
    }
}