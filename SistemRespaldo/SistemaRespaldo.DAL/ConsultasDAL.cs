using System;
using System.Collections.Generic;
using MySqlConnector; 
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class ConsultasDAL
    {
        // FUNCIÓN 1 ACTUALIZADA: Ahora lee de tu nueva tabla 'BasesDatos'
        public List<BaseDatos> ObtenerBasesDeDatos()
        {
            List<BaseDatos> listaBD = new List<BaseDatos>();

            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                // Usamos los campos nuevos: EsCompleto y TablasAIgnorar
                string query = "SELECT Id, Nombre, EsCompleto, TablasAIgnorar FROM BasesDatos";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                conexion.Open();

                using (MySqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaBD.Add(new BaseDatos
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            EsCompleto = Convert.ToBoolean(reader["EsCompleto"]),
                            TablasAIgnorar = reader["TablasAIgnorar"] != DBNull.Value ? reader["TablasAIgnorar"].ToString() : ""
                        });
                    }
                }
            }
            return listaBD;
        }

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

        public bool InsertarHorario(TimeSpan horaEjecucion)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO Horarios (HoraEjecucion) VALUES (@hora)";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@hora", horaEjecucion);
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // FUNCIÓN 4 ACTUALIZADA: Para que coincida con tu Web
        public bool InsertarBaseDeDatos(BaseDatos db)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO BasesDatos (Nombre, EsCompleto, TablasAIgnorar) " +
                               "VALUES (@nombre, @completo, @tablas)";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", db.Nombre);
                    comando.Parameters.AddWithValue("@completo", db.EsCompleto);
                    comando.Parameters.AddWithValue("@tablas", string.IsNullOrEmpty(db.TablasAIgnorar) ? (object)DBNull.Value : db.TablasAIgnorar);

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool InsertarLog(HistorialLog log)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                // Asegúrate de que esta tabla 'HistorialLogs' exista en MySQL
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