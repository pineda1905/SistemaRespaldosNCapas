using System;

namespace SistemaRespaldo.EN
{
    public class HistorialLog
    {
        public int Id { get; set; }
        public string BaseDeDatos { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
        
        // ¡ESTA ES LA LÍNEA QUE FALTA!
        public DateTime Fecha { get; set; } 
    }
}