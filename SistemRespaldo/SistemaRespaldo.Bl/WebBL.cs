using System;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.BL
{
    public class WebBL
    {
        public bool GuardarNuevoHorario(Horario nuevoHorario)
        {
            // Regla de negocio: Validar que no venga vacío por error
            if (nuevoHorario == null)
            {
                throw new ArgumentException("El horario no puede estar vacío.");
            }

            // Llamamos a tu DAL exclusiva de la Web
            WebDAL dal = new WebDAL();
            return dal.GuardarHorario(nuevoHorario);
        }
    }
}
