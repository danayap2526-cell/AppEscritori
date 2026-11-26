namespace AppEmocions.Controls
{
    partial class UcJocs
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            btnAfegir = new Button();
            lblSubtitol = new Label();
            flowJocs = new FlowLayoutPanel();
            pnlTop.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(btnAfegir);
            pnlTop.Controls.Add(lblSubtitol);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1000, 76);
            pnlTop.TabIndex = 0;
            // 
            // btnAfegir
            // 
            btnAfegir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAfegir.BackColor = Color.FromArgb(245, 190, 79);
            btnAfegir.Cursor = Cursors.Hand;
            btnAfegir.FlatAppearance.BorderSize = 0;
            btnAfegir.FlatStyle = FlatStyle.Flat;
            btnAfegir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAfegir.ForeColor = Color.FromArgb(31, 43, 77);
            btnAfegir.Location = new Point(826, 18);
            btnAfegir.Name = "btnAfegir";
            btnAfegir.Size = new Size(130, 40);
            btnAfegir.TabIndex = 1;
            btnAfegir.Text = "+  Afegir joc";
            btnAfegir.UseVisualStyleBackColor = false;
            // 
            // lblSubtitol
            // 
            lblSubtitol.AutoSize = true;
            lblSubtitol.Font = new Font("Segoe UI", 8.5F);
            lblSubtitol.ForeColor = Color.FromArgb(74, 120, 176);
            lblSubtitol.Location = new Point(44, 30);
            lblSubtitol.Name = "lblSubtitol";
            lblSubtitol.Size = new Size(237, 15);
            lblSubtitol.TabIndex = 0;
            lblSubtitol.Text = "Configura els jocs disponibles per als grups.";
            // 
            // flowJocs
            // 
            flowJocs.AutoScroll = true;
            flowJocs.Dock = DockStyle.Fill;
            flowJocs.Location = new Point(0, 76);
            flowJocs.Name = "flowJocs";
            flowJocs.Padding = new Padding(40, 0, 24, 20);
            flowJocs.Size = new Size(1000, 524);
            flowJocs.TabIndex = 1;
            // 
            // UcJocs
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(249, 247, 243);
            Controls.Add(flowJocs);
            Controls.Add(pnlTop);
            Name = "UcJocs";
            Size = new Size(1000, 600);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblSubtitol;
        private System.Windows.Forms.Button btnAfegir;
        private System.Windows.Forms.FlowLayoutPanel flowJocs;
    }
}