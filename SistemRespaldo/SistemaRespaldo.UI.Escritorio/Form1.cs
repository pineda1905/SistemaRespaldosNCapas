using System;
using System.Windows.Forms;
using SistemaRespaldo.DAL; 
using SistemaRespaldo.EN;  
using SistemaRespaldo.DAL;
using SistemaRespaldo.EN;
using SistemaRespaldo.BL; // Agregado por si tus motores están en esta capa

namespace SistemaRespaldo.UI.Escritorio
{
    public partial class Form1 : Form
    {
        private bool cerrarRealmente = false;

        public Form1()
        {
            InitializeComponent();
        }

        // --- ESTE ES EL MÉTODO QUE EL DISEÑADOR BUSCA ---
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
                var configuracionPrueba = new SistemaRespaldo.EN.ConfiguracionRespaldo
                {
                    Nombre = "SistemaRespaldos",
                    EsCompleto = false, // Prueba parcial
                    TablasAIgnorar = "HistorialLogs" // Ejemplo de tabla a ignorar
                };

                // Enviamos al motor
                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);
                var resultado = RespaldoMotor.GenerarRespaldo(configuracionPrueba);

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

            // Seguimos usando ConsultasDAL para los horarios y logs
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
                // --- INTEGRACIÓN MONGODB: Usamos WebDAL para leer las nuevas columnas ---
                WebDAL webDal = new WebDAL();
                var listaConfig = webDal.ObtenerBasesDeDatosActualizado();

                foreach (var config in listaConfig)
                {
                    // El motor ahora recibe 'BaseDatos' y aplica tus reglas de ignorar tablas
                    var resultado = RespaldoMotor.GenerarRespaldo(config);
                    var resultado = (exito: false, mensaje: "");

                    // Registro del historial
                    dal.InsertarLog(new HistorialLog
                    // --- EL SEMÁFORO: Decidimos qué motor ejecutar ---
                    if (config.TipoMotor == "MongoDB")
                    {
                        resultado = RespaldoMongoMotor.GenerarRespaldo(config);
                    }
                    else
                    {
                        // Si es "MySQL" o viene vacío, usamos el motor original
                        resultado = RespaldoMotor.GenerarRespaldo(config);
                    }

                    // --- GUARDADO DE LOG: Se mantiene intacto para ambos motores ---
                    HistorialLog log = new HistorialLog
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba forzada...");

                var configPrueba = new SistemaRespaldo.EN.ConfiguracionRespaldo
                {
                    NombreBaseDatos = "SistemaRespaldos",
                    TipoRespaldoCompletoOParcial = false,
                    TablasAIgnorar = "horarios"
                };

                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);

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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void restaurarMotorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void salirPorCompletoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cerrarRealmente = true;
            Application.Exit();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            try
            {
                // Se registra en el inicio de Windows del usuario actual
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rk.SetValue("MotorRespaldosApp", $"\"{Application.ExecutablePath}\"");
            }
            catch { /* Ignoramos si no hay permisos */ }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!cerrarRealmente)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }
    }
}