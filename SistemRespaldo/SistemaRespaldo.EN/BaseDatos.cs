namespace SistemaRespaldo.EN
{
    public class BaseDatos
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        // Nuevo: TRUE para completo, FALSE para parcial
        public bool EsCompleto { get; set; } = true; 
        
        // Nuevo: Lista de tablas separadas por coma (ej: "logs,temp")
        public string TablasAIgnorar { get; set; } = string.Empty;

        // Soporte Multi-Motor (Día 10)
        public string TipoMotor { get; set; } = "MySQL";
        public string CadenaConexion { get; set; } = string.Empty;
    }
}