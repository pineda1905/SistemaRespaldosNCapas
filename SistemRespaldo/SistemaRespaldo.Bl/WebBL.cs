using System;
using System.Collections.Generic;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;

namespace SistemaRespaldo.BL
{
    public class WebBL
    {
        private WebDAL dal = new WebDAL();

        // --- MÉTODOS DEL DÍA 6 (Gestión de Bases de Datos) ---

        // 1. Obtener la lista de bases de datos registradas
        public List<BaseDatos> ObtenerBasesDatos()
        {
            return dal.ObtenerBasesDatos();
        }

        // 2. Registrar una nueva base de datos con reglas de negocio
        public bool GuardarBaseDatos(BaseDatos db)
        {
            // Regla de negocio: Validar que no venga vacío o solo con espacios
            if (db == null || string.IsNullOrWhiteSpace(db.Nombre))
            {
                throw new ArgumentException("El nombre de la base de datos no puede estar vacío.");
            }
            
            return dal.GuardarBaseDatos(db);
        }

        // --- TU MÉTODO DE LA SEMANA 1 (Gestión de Horarios) ---
        public bool GuardarNuevoHorario(Horario nuevoHorario)
        {
            // Regla de negocio: Validar que no venga vacío por error
            if (nuevoHorario == null)
            {
                throw new ArgumentException("El horario no puede estar vacío.");
            }

            // Llamamos a tu DAL exclusiva de la Web
            return dal.GuardarHorario(nuevoHorario);
        }

// Agrega esto dentro de la clase WebBL
public bool EliminarBaseDatos(int id)
{
    if (id <= 0)
    {
        throw new ArgumentException("El ID no es válido.");
    }
    return dal.EliminarBaseDatos(id);
}


public List<HistorialLog> ObtenerHistorialLogs()
{
    // Llamamos a la DAL para traer los datos
    SistemaRespaldo.DAL.ConsultasDAL dal = new SistemaRespaldo.DAL.ConsultasDAL();
    return dal.ObtenerLogs(); // Asegúrate de que en ConsultasDAL se llame ObtenerLogs
}

// Día 12: Buscar un log específico por ID (para la descarga del archivo)
public HistorialLog ObtenerLogPorId(int id)
{
    if (id <= 0)
    {
        throw new ArgumentException("El ID del log no es válido.");
    }
    SistemaRespaldo.DAL.ConsultasDAL dal = new SistemaRespaldo.DAL.ConsultasDAL();
    return dal.ObtenerLogPorId(id);
}

    }

}