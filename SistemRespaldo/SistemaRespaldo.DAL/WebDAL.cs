using System;
<<<<<<< HEAD
using System.Collections.Generic;
using MySqlConnector; 
=======
using MySqlConnector;
>>>>>>> develop
using SistemaRespaldo.EN;
// IMPORTANTE: Agregamos la referencia a la DAL para usar el Helper
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
<<<<<<< HEAD
       private string connectionString = "Server=localhost;Database=SistemaRespaldos;Uid=alex;Pwd=12345;";
       
=======
        // ELIMINAMOS la línea de la contraseña "anderson" o "base1234"
        // Ahora usamos ConfiguracionHelper.CadenaConexion
>>>>>>> develop

        // 1. Obtener todas las bases de datos registradas
        public List<BaseDatos> ObtenerBasesDatos()
        {
            List<BaseDatos> lista = new List<BaseDatos>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Nombre FROM BasesDatos";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new BaseDatos
                                {
                                    Id = reader.GetInt32("Id"),
                                    Nombre = reader.GetString("Nombre")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener bases de datos de la BD: " + ex.Message);
            }
            return lista;
        }

        // 2. Registrar una nueva base de datos para respaldar
        public bool GuardarBaseDatos(BaseDatos db)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO BasesDatos (Nombre) VALUES (@nombre)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", db.Nombre);
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la base de datos: " + ex.Message);
            }
        }


        // --- TU MÉTODO DE LA SEMANA 1 (Gestión de Horarios) ---
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
<<<<<<< HEAD
                        return filasAfectadas > 0; 
=======
                        return filasAfectadas > 0;
>>>>>>> develop
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