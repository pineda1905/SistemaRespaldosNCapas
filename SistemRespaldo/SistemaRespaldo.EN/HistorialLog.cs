using System;

namespace SistemaRespaldo.EN
{
    public class HistorialLog
    {
        public int Id { get; set; }
        public string BaseDeDatos { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }

        // Día 12: Indica qué motor generó el respaldo (MySQL o MongoDB)
        public string TipoMotor { get; set; } = "MySQL";
    }
}