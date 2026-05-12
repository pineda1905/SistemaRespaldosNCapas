using System;
using System.Windows.Forms;
using SistemaRespaldo.DAL; //
using SistemaRespaldo.EN;  //

namespace SistemaRespaldo.UI.Escritorio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // --- MODIFICACIÓN: Se eliminó el MessageBox de prueba para que el motor sea silencioso ---
        }

        // --- ESTE ES EL MÉTODO QUE EL DISEÑADOR BUSCA ---
        // Si el error persiste, tenerlo aquí vacío lo solucionará
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba forzada...");

                // --- CORRECCIÓN: Creamos el objeto que el motor espera ahora ---
                var configuracionPrueba = new SistemaRespaldo.EN.ConfiguracionRespaldo
                {
                    NombreBaseDatos = "SistemaRespaldos",
                    TipoRespaldoCompletoOParcial = true, // Prueba completa
                    TablasAIgnorar = "" // Sin ignorar nada por ahora
                };

                // Ahora enviamos el objeto completo, no solo el string
                var resultado = RespaldoMotor.GenerarRespaldo(configuracionPrueba);

                ConsultasDAL dal = new ConsultasDAL();
                dal.InsertarLog(new HistorialLog
                {
                    BaseDeDatos = configuracionPrueba.NombreBaseDatos,
                    Estado = resultado.exito ? "Exito" : "Error",
                    Mensaje = resultado.mensaje
                });

                MessageBox.Show($"Motor dice: {resultado.mensaje}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
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
                timer1.Enabled = false; // Pausa para evitar bucles en el mismo minuto

                // Leemos TODAS las configuraciones de bases de datos
                var listaConfig = dal.ObtenerBasesDeDatos();

                foreach (var config in listaConfig)
                {
                    // Ejecutamos el motor pasando el objeto de configuración (Día 7)
                    var resultado = RespaldoMotor.GenerarRespaldo(config);

                    // --- DÍA 8: GUARDADO DE LOG SEGÚN RESULTADO ---
                    HistorialLog log = new HistorialLog
                    {
                        BaseDeDatos = config.NombreBaseDatos,
                        Estado = resultado.exito ? "Exito" : "Error",
                        Mensaje = resultado.mensaje
                    };

                    dal.InsertarLog(log);
                }

                timer1.Enabled = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba forzada...");

                // 1. Creamos la configuración para la prueba
                var configPrueba = new SistemaRespaldo.EN.ConfiguracionRespaldo
                {
                    NombreBaseDatos = "SistemaRespaldos",
                    TipoRespaldoCompletoOParcial = false, // ¡FALSO para que sea parcial!
                    TablasAIgnorar = "horarios" // ¡Aquí le decimos la tabla!
                };

                // 2. Ahora enviamos el objeto 'configPrueba' en lugar del string con comillas
                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);

                // 3. Registramos el Log usando la DAL
                SistemaRespaldo.DAL.ConsultasDAL dal = new SistemaRespaldo.DAL.ConsultasDAL();
                dal.InsertarLog(new SistemaRespaldo.EN.HistorialLog
                {
                    BaseDeDatos = configPrueba.NombreBaseDatos,
                    Estado = resultado.exito ? "Exito" : "Error",
                    Mensaje = resultado.mensaje
                });

                MessageBox.Show($"Resultado: {resultado.mensaje}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el botón: {ex.Message}");
            }
        }
    }
}