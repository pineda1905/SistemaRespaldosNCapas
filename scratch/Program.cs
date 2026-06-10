using System;
using MySqlConnector;

class Program
{
    static void Main()
    {
        string connStr = "Server=127.0.0.1;Port=3306;Database=SistemaRespaldos;Uid=root;Pwd=anderson;";
        try
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO BasesDatos (Nombre, EsCompleto, TablasAIgnorar, TipoMotor, CadenaConexion) VALUES (@nom, @comp, @ign, @tipo, @cadena)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", "MongoDb_Ventas");
                    cmd.Parameters.AddWithValue("@comp", true);
                    cmd.Parameters.AddWithValue("@ign", "");
                    cmd.Parameters.AddWithValue("@tipo", "MongoDB");
                    cmd.Parameters.AddWithValue("@cadena", "mongodb://localhost:27017/Ventas");
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine($"MongoDB record inserted! Rows affected: {rows}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
