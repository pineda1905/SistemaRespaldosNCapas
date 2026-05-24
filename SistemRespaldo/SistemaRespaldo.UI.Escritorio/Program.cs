using System;
using System.Windows.Forms;

namespace SistemaRespaldo.UI.Escritorio
{
    internal static class Program
    {
        /// <summary>
        ///  Punto de entrada principal de la aplicación Windows Forms.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}