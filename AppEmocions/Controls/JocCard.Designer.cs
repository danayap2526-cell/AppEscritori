namespace AppEmocions.Controls
{
    partial class JocCard
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
            pnlCabecera = new Panel();
            lblIcono = new Label();
            lblNom = new Label();
            chkActiu = new CheckBox();
            lblId = new Label();
            lblDescripcio = new Label();
            pnlLinia = new Panel();
            btnEditar = new Button();
            btnEliminar = new Button();
            pnlCabecera.SuspendLayout();
            SuspendLayout();
            // 
            // pnlCabecera
            // 
            pnlCabecera.BackColor = Color.FromArgb(31, 43, 77);
            pnlCabecera.Controls.Add(lblIcono);
            pnlCabecera.Dock = DockStyle.Top;
            pnlCabecera.Location = new Point(0, 0);
            pnlCabecera.Name = "pnlCabecera";
            pnlCabecera.Size = new Size(400, 100);
            pnlCabecera.TabIndex = 0;
            // 
            // lblIcono
            // 
            lblIcono.BackColor = Color.Transparent;
            lblIcono.Dock = DockStyle.Fill;
            lblIcono.Font = new Font("Segoe UI Symbol", 28F);
            lblIcono.ForeColor = Color.FromArgb(245, 190, 79);
            lblIcono.Location = new Point(0, 0);
            lblIcono.Name = "lblIcono";
            lblIcono.Size = new Size(400, 100);
            lblIcono.TabIndex = 0;
            lblIcono.Text = "♥";
            lblIcono.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNom
            // 
            lblNom.Font = new Font("Segoe UI", 11F);
            lblNom.ForeColor = Color.FromArgb(31, 43, 77);
            lblNom.Location = new Point(16, 112);
            lblNom.Name = "lblNom";
            lblNom.Size = new Size(280, 26);
            lblNom.TabIndex = 1;
            lblNom.Text = "Nom del joc";
            lblNom.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkActiu
            // 
            chkActiu.AutoSize = true;
            chkActiu.Cursor = Cursors.Hand;
            chkActiu.Font = new Font("Segoe UI", 9F);
            chkActiu.ForeColor = Color.FromArgb(31, 43, 77);
            chkActiu.Location = new Point(318, 116);
            chkActiu.Name = "chkActiu";
            chkActiu.Size = new Size(54, 19);
            chkActiu.TabIndex = 2;
            chkActiu.Text = "Actiu";
            chkActiu.UseVisualStyleBackColor = true;
            // 
            // lblId
            // 
            lblId.Font = new Font("Consolas", 8F);
            lblId.ForeColor = Color.FromArgb(46, 139, 139);
            lblId.Location = new Point(16, 142);
            lblId.Name = "lblId";
            lblId.Size = new Size(366, 18);
            lblId.TabIndex = 3;
            lblId.Text = "IDENTIFICADOR";
            // 
            // lblDescripcio
            // 
            lblDescripcio.Font = new Font("Segoe UI", 9F);
            lblDescripcio.ForeColor = Color.FromArgb(120, 120, 127);
            lblDescripcio.Location = new Point(16, 166);
            lblDescripcio.Name = "lblDescripcio";
            lblDescripcio.Size = new Size(366, 40);
            lblDescripcio.TabIndex = 4;
            lblDescripcio.Text = "Descripció";
            // 
            // pnlLinia
            // 
            pnlLinia.BackColor = Color.FromArgb(232, 229, 223);
            pnlLinia.Location = new Point(0, 214);
            pnlLinia.Name = "pnlLinia";
            pnlLinia.Size = new Size(398, 1);
            pnlLinia.TabIndex = 5;
            // 
            // btnEditar
            // 
            btnEditar.BackColor = Color.White;
            btnEditar.Cursor = Cursors.Hand;
            btnEditar.FlatAppearance.BorderColor = Color.FromArgb(227, 224, 218);
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.Font = new Font("Segoe UI", 9F);
            btnEditar.ForeColor = Color.FromArgb(31, 43, 77);
            btnEditar.Location = new Point(296, 224);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(86, 30);
            btnEditar.TabIndex = 6;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = false;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.White;
            btnEliminar.Cursor = Cursors.Hand;
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(220, 100, 100);
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Font = new Font("Segoe UI", 9F);
            btnEliminar.ForeColor = Color.FromArgb(180, 55, 55);
            btnEliminar.Location = new Point(198, 224);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(92, 30);
            btnEliminar.TabIndex = 7;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            // 
            // JocCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(btnEliminar);
            Controls.Add(btnEditar);
            Controls.Add(pnlLinia);
            Controls.Add(lblDescripcio);
            Controls.Add(lblId);
            Controls.Add(chkActiu);
            Controls.Add(lblNom);
            Controls.Add(pnlCabecera);
            Name = "JocCard";
            Size = new Size(400, 266);
            pnlCabecera.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlCabecera;
        private System.Windows.Forms.Label lblIcono;
        private System.Windows.Forms.Label lblNom;
        private System.Windows.Forms.CheckBox chkActiu;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblDescripcio;
        private System.Windows.Forms.Panel pnlLinia;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
    }
}