using System;
using System.Windows.Forms;

namespace SistemaRespaldo.UI.Escritorio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Prueba para tu Día 3:
            MessageBox.Show(
                $"Ruta del Motor: {ConfiguracionMotor.RutaMysqlDump}\n" +
                $"Carpeta de Destino: {ConfiguracionMotor.RutaGuardadoRespaldos}",
                "Prueba de Lectura JSON",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

    
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 1. Obtenemos la hora actual de tu computadora (horas y minutos, ignorando los segundos)
            TimeSpan horaActual = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);

            // 2. Nos conectamos a la BD para leer los horarios usando la Capa de Datos (DAL)
            SistemaRespaldo.DAL.ConsultasDAL dal = new SistemaRespaldo.DAL.ConsultasDAL();
            var horariosGuardados = dal.ObtenerHorarios();

            // 3. Revisamos si la hora actual de tu PC coincide con algún horario guardado
            bool esHoraDeRespaldar = false;
            foreach (var horario in horariosGuardados)
            {
                if (horario.HoraEjecucion.Hours == horaActual.Hours &&
                    horario.HoraEjecucion.Minutes == horaActual.Minutes)
                {
                    esHoraDeRespaldar = true;
                    break; // Si encontramos coincidencia, dejamos de buscar
                }
            }

            // 4. Si es la hora exacta, iniciamos la magia
            if (esHoraDeRespaldar)
            {
                // IMPORTANTE: Pausamos el reloj para que no intente hacer 2 respaldos en el mismo minuto
                timer1.Enabled = false;

                // Leemos de la BD cuáles son las bases de datos que debemos respaldar
                var basesDeDatos = dal.ObtenerBasesDeDatos();

                // Mandamos a respaldar cada una de ellas
                foreach (var bd in basesDeDatos)
                {
                    // Aquí llamamos a tu motor enviándole el nombre de la BD
                    RespaldoMotor.GenerarRespaldo(bd.NombreBaseDatos);
                }

                // Encendemos el reloj nuevamente para que siga vigilando las próximas horas
                timer1.Enabled = true;
            }
        }
    }
}