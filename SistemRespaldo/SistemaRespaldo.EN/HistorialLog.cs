using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaRespaldo.EN
{
    public class HistorialLog
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string BaseDeDatos { get; set; }
        public string Estado { get; set; } // "Exito" o "Error"
        public string Mensaje { get; set; }
    }
}
