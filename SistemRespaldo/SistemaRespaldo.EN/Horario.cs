using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaRespaldo.EN
{
    public class Horario
    {
        public int Id { get; set; }
        public TimeSpan HoraEjecucion { get; set; } // TimeSpan es ideal para guardar horas sin fecha
    }
}
