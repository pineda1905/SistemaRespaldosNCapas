using System;
using System.Collections.Generic;
using MySqlConnector;
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // Método para listar las bases de datos guardadas
        public List<BaseDatos> ObtenerBasesDatos()
        {
            List<BaseDatos> lista = new List<BaseDatos>();
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "SELECT Id, Nombre FROM BasesDatos";
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new BaseDatos { 
                                Id = reader.GetInt32("Id"), 
                                Nombre = reader.GetString("Nombre") 
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al leer: " + ex.Message); }
            return lista;
        }

        // Método para insertar una nueva base de datos
        public bool GuardarBaseDatos(BaseDatos db)
        {
            try
            {
                using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
                {
                    conn.Open();
                    string sql = "INSERT INTO BasesDatos (Nombre) VALUES (@nom)";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nom", db.Nombre);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error al guardar: " + ex.Message); }
        }


        public bool GuardarHorario(Horario horario)
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
    }
        

}