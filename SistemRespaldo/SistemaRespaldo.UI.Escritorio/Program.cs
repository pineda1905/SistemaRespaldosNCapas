using System;
using System.Windows.Forms;
using SistemaRespaldo.EN;
using SistemaRespaldo.DAL;
using SistemaRespaldo.UI.Escritorio;

namespace SistemaRespaldo.UI.Escritorio
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}