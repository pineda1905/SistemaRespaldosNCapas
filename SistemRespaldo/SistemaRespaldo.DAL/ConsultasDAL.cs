using System;
using System.Collections.Generic;
using MySqlConnector; 
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class ConsultasDAL
    {
        // FUNCIÓN 1 ACTUALIZADA: Lee de tu nueva tabla 'BasesDatos'
        public List<BaseDatos> ObtenerBasesDeDatos()
        {
            List<BaseDatos> listaBD = new List<BaseDatos>();

            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "SELECT Id, Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion FROM BasesDatos";
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
                            TablasAIgnorar = reader["TablasAIgnorar"] != DBNull.Value ? reader["TablasAIgnorar"].ToString() : "",
                            TipoMotor = reader["TipoMotor"] != DBNull.Value ? reader["TipoMotor"].ToString() : "MySQL",
                            CadenaConexion = reader["CadenaConexion"] != DBNull.Value ? reader["CadenaConexion"].ToString() : ""
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

        public bool InsertarBaseDeDatos(BaseDatos db)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                conexion.Open();
                string query = "INSERT INTO BasesDatos (Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion) " +
                               "VALUES (@nombre, @completo, @tablas, @tipo, @cadena)";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", db.Nombre);
                    comando.Parameters.AddWithValue("@completo", db.EsCompleto);
                    comando.Parameters.AddWithValue("@tablas", string.IsNullOrEmpty(db.TablasAIgnorar) ? (object)DBNull.Value : db.TablasAIgnorar);
                    comando.Parameters.AddWithValue("@tipo", db.TipoMotor ?? "MySQL");
                    comando.Parameters.AddWithValue("@cadena", db.CadenaConexion ?? (object)DBNull.Value);

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool InsertarLog(HistorialLog log)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "INSERT INTO HistorialLogs (BaseDeDatos, Estado, Mensaje, TipoMotor) VALUES (@db, @estado, @msj, @tipo)";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                comando.Parameters.AddWithValue("@db", log.BaseDeDatos);
                comando.Parameters.AddWithValue("@estado", log.Estado);
                comando.Parameters.AddWithValue("@msj", log.Mensaje);
                comando.Parameters.AddWithValue("@tipo", log.TipoMotor ?? "MySQL");

                conexion.Open();
                return comando.ExecuteNonQuery() > 0;
            }
        }

        // --- FUNCIÓN PARA LA PANTALLA WEB (Actualizada Día 12 con TipoMotor) ---
        public List<HistorialLog> ObtenerLogs()
        {
            List<HistorialLog> lista = new List<HistorialLog>();

            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                // Ordenamos por Fecha descendente para ver lo más reciente arriba. Límite de 50 para no saturar.
                string query = "SELECT Id, BaseDeDatos, Estado, Mensaje, Fecha, TipoMotor FROM HistorialLogs ORDER BY Fecha DESC LIMIT 50";
                MySqlCommand comando = new MySqlCommand(query, conexion);

                try
                {
                    conexion.Open();
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new HistorialLog
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                BaseDeDatos = reader["BaseDeDatos"].ToString(),
                                Estado = reader["Estado"].ToString(),
                                Mensaje = reader["Mensaje"].ToString(),
                                Fecha = reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : DateTime.MinValue,
                                TipoMotor = reader["TipoMotor"] != DBNull.Value ? reader["TipoMotor"].ToString() : "MySQL"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener logs: " + ex.Message);
                }
            }
            return lista;
        }

        // --- Día 12: Buscar un log específico por su ID (para descarga) ---
        public HistorialLog ObtenerLogPorId(int id)
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
            {
                string query = "SELECT Id, BaseDeDatos, Estado, Mensaje, Fecha, TipoMotor FROM HistorialLogs WHERE Id = @id";
                MySqlCommand comando = new MySqlCommand(query, conexion);
                comando.Parameters.AddWithValue("@id", id);

                try
                {
                    conexion.Open();
                    using (MySqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new HistorialLog
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                BaseDeDatos = reader["BaseDeDatos"].ToString(),
                                Estado = reader["Estado"].ToString(),
                                Mensaje = reader["Mensaje"].ToString(),
                                Fecha = reader["Fecha"] != DBNull.Value ? Convert.ToDateTime(reader["Fecha"]) : DateTime.MinValue,
                                TipoMotor = reader["TipoMotor"] != DBNull.Value ? reader["TipoMotor"].ToString() : "MySQL"
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener log por ID: " + ex.Message);
                }
            }
            return null;
        }
    }
}