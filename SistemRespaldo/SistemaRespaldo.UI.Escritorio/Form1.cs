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
    }
}