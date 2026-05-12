using System;
using System.Collections.Generic;
using MySqlConnector;
using SistemaRespaldo.EN;

namespace SistemaRespaldo.DAL
{
    public class WebDAL
    {
        // Método para listar las bases de datos guardadas
      // 1. Modifica el SELECT para traer las nuevas columnas
public List<BaseDatos> ObtenerBasesDatos()
{
    List<BaseDatos> lista = new List<BaseDatos>();
    using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
    {
        conn.Open();
        string sql = "SELECT Id, Nombre, EsCompleto, TablasAIgnorar FROM BasesDatos";
        using (var cmd = new MySqlCommand(sql, conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                lista.Add(new BaseDatos { 
                    Id = reader.GetInt32("Id"), 
                    Nombre = reader.GetString("Nombre"),
                    EsCompleto = reader.GetBoolean("EsCompleto"),
                    TablasAIgnorar = reader.IsDBNull(reader.GetOrdinal("TablasAIgnorar")) ? "" : reader.GetString("TablasAIgnorar")
                });
            }
        }
    }
    return lista;
}

// 2. Modifica el INSERT para guardar los nuevos datos
public bool GuardarBaseDatos(BaseDatos db)
{
    using (var conn = new MySqlConnection(ConfiguracionHelper.CadenaConexion))
    {
        conn.Open();
        string sql = "INSERT INTO BasesDatos (Nombre, EsCompleto, TablasAIgnorar) VALUES (@nom, @comp, @ign)";
        using (var cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@nom", db.Nombre);
            cmd.Parameters.AddWithValue("@comp", db.EsCompleto);
            cmd.Parameters.AddWithValue("@ign", db.TablasAIgnorar ?? (object)DBNull.Value);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
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
    
        public bool EliminarBaseDatos(int id)
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

    }
}