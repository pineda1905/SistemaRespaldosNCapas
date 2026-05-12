using System;
using System.Windows.Forms;
using SistemaRespaldo.DAL; 
using SistemaRespaldo.EN;  

namespace SistemaRespaldo.UI.Escritorio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // El motor arranca el Timer automáticamente si lo tienes configurado en el diseñador
        }

        // --- BOTÓN DE PRUEBA MANUAL ---
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba de respaldo manual...");

                // Usamos la nueva entidad 'BaseDatos'
                var configPrueba = new BaseDatos
                {
                    Nombre = "SistemaRespaldos",
                    EsCompleto = false, // Prueba parcial
                    TablasAIgnorar = "HistorialLogs" // Ejemplo de tabla a ignorar
                };

                // Enviamos al motor
                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);

                // Guardamos el Log
                ConsultasDAL dal = new ConsultasDAL();
                dal.InsertarLog(new HistorialLog
                {
                    BaseDeDatos = configPrueba.Nombre,
                    Estado = resultado.exito ? "Exito" : "Error",
                    Mensaje = resultado.mensaje
                });

                MessageBox.Show($"Resultado: {resultado.mensaje}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en la prueba: {ex.Message}");
            }
        }

        // --- EL RELOJ QUE REVISA LOS HORARIOS ---
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Solo comparamos Horas y Minutos (segundos a 0)
            TimeSpan horaActual = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            ConsultasDAL dal = new ConsultasDAL();
            
            var horariosGuardados = dal.ObtenerHorarios();

            bool esHoraDeRespaldar = false;
            foreach (var h in horariosGuardados)
            {
                if (h.HoraEjecucion.Hours == horaActual.Hours && h.HoraEjecucion.Minutes == horaActual.Minutes)
                {
                    esHoraDeRespaldar = true;
                    break;
                }
            }

            if (esHoraDeRespaldar)
            {
                timer1.Enabled = false; // Pausar para que no se repita en el mismo minuto

                // Obtenemos todas las bases de datos configuradas desde tu Web
                var listaConfig = dal.ObtenerBasesDeDatos();

                foreach (var config in listaConfig)
                {
                    // El motor ahora recibe 'BaseDatos' y aplica tus reglas de ignorar tablas
                    var resultado = RespaldoMotor.GenerarRespaldo(config);

                    // Registro del historial
                    dal.InsertarLog(new HistorialLog
                    {
                        BaseDeDatos = config.Nombre,
                        Estado = resultado.exito ? "Exito" : "Error",
                        Mensaje = resultado.mensaje
                    });
                }

                // Esperamos un minuto antes de reactivar para evitar doble ejecución
                System.Threading.Thread.Sleep(60000); 
                timer1.Enabled = true;
            }
        }
    }
}