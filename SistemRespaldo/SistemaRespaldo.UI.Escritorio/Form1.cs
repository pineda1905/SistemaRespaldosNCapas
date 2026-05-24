using System;
using System.Windows.Forms;
using SistemaRespaldo.DAL; 
using SistemaRespaldo.EN;  
using SistemaRespaldo.BL; 

namespace SistemaRespaldo.UI.Escritorio
{
    public partial class Form1 : Form
    {
        private bool cerrarRealmente = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // --- TAREA DEL CRONOGRAMA: AUTO-ARRANQUE EN SEGUNDO PLANO ---
            try
            {
                Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                rk.SetValue("MotorRespaldosApp", $"\"{Application.ExecutablePath}\"");
            }
            catch { /* Ignoramos si el usuario no tiene permisos */ }
        }

        // --- BOTÓN DE PRUEBA MANUAL ---
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba de respaldo manual...");

                // Entidad unificada
                var configPrueba = new ConfiguracionRespaldo
                {
                    NombreBaseDatos = "SistemaRespaldos",
                    TipoRespaldoCompletoOParcial = false, 
                    TablasAIgnorar = "HistorialLogs",
                    TipoMotor = "MySQL" // Por defecto en pruebas manuales
                };

                // Enviamos al motor
                var resultado = RespaldoMotor.GenerarRespaldo(configPrueba);

                // Guardamos el Log
                ConsultasDAL dal = new ConsultasDAL();
                dal.InsertarLog(new HistorialLog
                {
                    BaseDeDatos = configPrueba.NombreBaseDatos,
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

                // Obtenemos las bases usando WebDAL para que traiga la columna TipoMotor de Mongo
                WebDAL webDal = new WebDAL();
                var listaConfig = webDal.ObtenerBasesDeDatosActualizado();

                foreach (var config in listaConfig)
                {
                    var resultado = (exito: false, mensaje: "");

                    // --- EL SEMÁFORO DE PINEDA (MySQL vs MongoDB) ---
                    if (config.TipoMotor == "MongoDB")
                    {
                        resultado = RespaldoMongoMotor.GenerarRespaldo(config);
                    }
                    else
                    {
                        resultado = RespaldoMotor.GenerarRespaldo(config);
                    }

                    // --- GUARDADO DE LOG (Sirve para ambos) ---
                    dal.InsertarLog(new HistorialLog
                    {
                        BaseDeDatos = config.NombreBaseDatos,
                        Estado = resultado.exito ? "Exito" : "Error",
                        Mensaje = resultado.mensaje
                    });
                }

                System.Threading.Thread.Sleep(60000); 
                timer1.Enabled = true;
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