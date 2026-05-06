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
                var resultado = RespaldoMotor.GenerarRespaldo("SistemaRespaldos");

                ConsultasDAL dal = new ConsultasDAL();
                dal.InsertarLog(new HistorialLog
                {
                    BaseDeDatos = "SistemaRespaldos",
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
            // 1. Obtenemos la hora actual de tu computadora
            TimeSpan horaActual = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

            // 2. Nos conectamos a la BD para leer los horarios
            ConsultasDAL dal = new ConsultasDAL(); //
            var horariosGuardados = dal.ObtenerHorarios(); //

            // 3. Revisamos si la hora actual coincide con algún horario
            bool esHoraDeRespaldar = false;
            foreach (var horario in horariosGuardados)
            {
                if (horario.HoraEjecucion.Hours == horaActual.Hours &&
                    horario.HoraEjecucion.Minutes == horaActual.Minutes)
                {
                    esHoraDeRespaldar = true;
                    break;
                }
            }

            // 4. Si es la hora exacta, iniciamos el proceso
            if (esHoraDeRespaldar)
            {
                timer1.Enabled = false;

                var basesDeDatos = dal.ObtenerBasesDeDatos(); //

                foreach (var bd in basesDeDatos)
                {
                    // --- EDICIÓN DÍA 6: Captura de resultados y registro de Logs ---

                    // Ejecutamos el motor y recibimos (Éxito/Error y el Mensaje)
                    var resultado = RespaldoMotor.GenerarRespaldo(bd.NombreBaseDatos);

                    // Creamos el objeto del historial para la base de datos
                    HistorialLog nuevoLog = new HistorialLog
                    {
                        BaseDeDatos = bd.NombreBaseDatos,
                        Estado = resultado.exito ? "Exito" : "Error", //
                        Mensaje = resultado.mensaje // Guardamos el éxito o el error detallado
                    };

                    // Guardamos el log en la tabla HistorialLogs
                    dal.InsertarLog(nuevoLog);

                    // --- FIN EDICIÓN DÍA 6 ---
                }

                timer1.Enabled = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Iniciando prueba forzada...");
                // Intentamos respaldar la BD que configuraste en el JSON
                var resultado = RespaldoMotor.GenerarRespaldo("SistemaRespaldos");

                // Registramos el Log
                SistemaRespaldo.DAL.ConsultasDAL dal = new SistemaRespaldo.DAL.ConsultasDAL();
                dal.InsertarLog(new SistemaRespaldo.EN.HistorialLog
                {
                    BaseDeDatos = "SistemaRespaldos",
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