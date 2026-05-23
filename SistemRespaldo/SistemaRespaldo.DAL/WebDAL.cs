using System;
using System.Collections.Generic;
using MySqlConnector;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // ==========================================
        // METODOS EN ARMONÍA (ALEX & PINEDA)
        // ==========================================

        // 1. Listar bases de datos desde la Web (Usado por Alex)
        public List<BaseDatos> ObtenerBasesDatos()
        {
            List<BaseDatos> lista = new List<BaseDatos>();
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "SELECT Id, Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion FROM BasesDatos";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new BaseDatos
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                EsCompleto = reader.GetBoolean("EsCompleto"),
                                TablasAIgnorar = reader.IsDBNull(reader.GetOrdinal("TablasAIgnorar")) ? "" : reader.GetString("TablasAIgnorar"),
                                TipoMotor = reader.IsDBNull(reader.GetOrdinal("TipoMotor")) ? "MySQL" : reader.GetString("TipoMotor"),
                                CadenaConexion = reader.IsDBNull(reader.GetOrdinal("CadenaConexion")) ? "" : reader.GetString("CadenaConexion")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener BasesDatos (Web): " + ex.Message);
            }
            return lista;
        }

        // 2. Guardar nueva configuración desde la Web (Usado por Alex)
        public bool GuardarBaseDatos(BaseDatos db)
        {
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "INSERT INTO BasesDatos (Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion) VALUES (@nom, @comp, @ign, @tipo, @cadena)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nom", db.Nombre);
                        cmd.Parameters.AddWithValue("@comp", db.EsCompleto);
                        cmd.Parameters.AddWithValue("@ign", db.TablasAIgnorar ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@tipo", db.TipoMotor ?? "MySQL");
                        cmd.Parameters.AddWithValue("@cadena", db.CadenaConexion ?? (object)DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar BaseDatos: " + ex.Message);
            }
        }

        // 3. Guardar Horario de ejecución (Limpio y sin duplicados)
        public bool GuardarHorario(Horario horario)
        {
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "INSERT INTO Horarios (HoraEjecucion) VALUES (@hora)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@hora", horario.HoraEjecucion);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en Base de Datos al guardar horario: " + ex.Message);
            }
        }

        // 4. Eliminar configuración desde la Web (Usado por Alex)
        public bool EliminarBaseDatos(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "DELETE FROM BasesDatos WHERE Id = @id";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar BaseDatos: " + ex.Message);
            }
        }

        // 5. LECTURA PARA EL MOTOR DE ESCRITORIO (Tus respaldos automáticos de MongoDB/MySQL)
        public List<ConfiguracionRespaldo> ObtenerBasesDeDatosActualizado()
        {
            List<ConfiguracionRespaldo> lista = new List<ConfiguracionRespaldo>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    // OJO: Si Alex está usando la tabla 'BasesDatos' para todo, cambiamos el query para apuntar ahí y evitar desajustes.
                    string query = "SELECT Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion FROM BasesDatos";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader lector = cmd.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                var config = new ConfiguracionRespaldo
                                {
                                    NombreBaseDatos = lector["Nombre"].ToString(),
                                    TipoRespaldoCompletoOParcial = Convert.ToBoolean(lector["EsCompleto"]),
                                    TablasAIgnorar = lector["TablasAIgnorar"]?.ToString(),
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
                throw new Exception("Error al obtener las configuraciones de respaldo para el motor: " + ex.Message);
            }
            return lista;
        }
    }
}