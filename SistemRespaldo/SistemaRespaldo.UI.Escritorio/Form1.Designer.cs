namespace SistemaRespaldo.UI.Escritorio
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            timer1 = new System.Windows.Forms.Timer(components);
            button1 = new Button();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            restaurarMotorToolStripMenuItem = new ToolStripMenuItem();
            salirPorCompletoToolStripMenuItem = new ToolStripMenuItem();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 60000;
            timer1.Tick += timer1_Tick;
            // 
            // button1
            // 
            button1.Location = new Point(426, 114);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "Motor de Respaldos";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { restaurarMotorToolStripMenuItem, salirPorCompletoToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(172, 48);
            // 
            // restaurarMotorToolStripMenuItem
            // 
            restaurarMotorToolStripMenuItem.Name = "restaurarMotorToolStripMenuItem";
            restaurarMotorToolStripMenuItem.Size = new Size(171, 22);
            restaurarMotorToolStripMenuItem.Text = "Restaurar Motor";
            restaurarMotorToolStripMenuItem.Click += restaurarMotorToolStripMenuItem_Click;
            // 
            // salirPorCompletoToolStripMenuItem
            // 
            salirPorCompletoToolStripMenuItem.Name = "salirPorCompletoToolStripMenuItem";
            salirPorCompletoToolStripMenuItem.Size = new Size(171, 22);
            salirPorCompletoToolStripMenuItem.Text = "Salir por completo";
            salirPorCompletoToolStripMenuItem.Click += salirPorCompletoToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load_1;
            Resize += Form1_Resize;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }



        #endregion

        private System.Windows.Forms.Timer timer1;
        private Button button1;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem restaurarMotorToolStripMenuItem;
        private ToolStripMenuItem salirPorCompletoToolStripMenuItem;
    }
}