using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaRespaldo.EN
{
   
    public class ConfiguracionRespaldo
    {
        public int Id { get; set; }
        public string NombreBaseDatos { get; set; }
        public bool TipoRespaldoCompletoOParcial { get; set; }
        public string TablasAIgnorar { get; set; }
    }
}

