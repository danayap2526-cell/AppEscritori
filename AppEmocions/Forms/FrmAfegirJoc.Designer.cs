namespace AppEmocions.Forms
{
    partial class FrmAfegirJoc
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblNom = new Label();
            txtNom = new TextBox();
            lblId = new Label();
            txtId = new TextBox();
            lblDescripcio = new Label();
            txtDescripcio = new TextBox();
            btnCancelar = new Button();
            btnGuardar = new Button();
            SuspendLayout();
            // 
            // lblNom
            // 
            lblNom.AutoSize = true;
            lblNom.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNom.ForeColor = Color.FromArgb(31, 43, 77);
            lblNom.Location = new Point(24, 20);
            lblNom.Name = "lblNom";
            lblNom.Size = new Size(73, 15);
            lblNom.TabIndex = 0;
            lblNom.Text = "Nom del joc";
            // 
            // txtNom
            // 
            txtNom.Font = new Font("Segoe UI", 10F);
            txtNom.Location = new Point(24, 42);
            txtNom.Name = "txtNom";
            txtNom.PlaceholderText = "Nom del joc";
            txtNom.Size = new Size(372, 25);
            txtNom.TabIndex = 1;
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblId.ForeColor = Color.FromArgb(31, 43, 77);
            lblId.Location = new Point(24, 86);
            lblId.Name = "lblId";
            lblId.Size = new Size(79, 15);
            lblId.TabIndex = 2;
            lblId.Text = "Identificador";
            // 
            // txtId
            // 
            txtId.CharacterCasing = CharacterCasing.Upper;
            txtId.Font = new Font("Segoe UI", 10F);
            txtId.Location = new Point(24, 108);
            txtId.Name = "txtId";
            txtId.PlaceholderText = "IDENTIFICADOR_JOC";
            txtId.Size = new Size(372, 25);
            txtId.TabIndex = 3;
            // 
            // lblDescripcio
            // 
            lblDescripcio.AutoSize = true;
            lblDescripcio.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDescripcio.ForeColor = Color.FromArgb(31, 43, 77);
            lblDescripcio.Location = new Point(24, 152);
            lblDescripcio.Name = "lblDescripcio";
            lblDescripcio.Size = new Size(65, 15);
            lblDescripcio.TabIndex = 4;
            lblDescripcio.Text = "Descripció";
            // 
            // txtDescripcio
            // 
            txtDescripcio.Font = new Font("Segoe UI", 10F);
            txtDescripcio.Location = new Point(24, 174);
            txtDescripcio.Multiline = true;
            txtDescripcio.Name = "txtDescripcio";
            txtDescripcio.PlaceholderText = "Breu descripció educativa...";
            txtDescripcio.Size = new Size(372, 90);
            txtDescripcio.TabIndex = 5;
            // 
            // btnCancelar
            // 
            btnCancelar.DialogResult = DialogResult.Cancel;
            btnCancelar.Font = new Font("Segoe UI", 9F);
            btnCancelar.Location = new Point(170, 290);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(110, 36);
            btnCancelar.TabIndex = 6;
            btnCancelar.Text = "Cancel·lar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(245, 190, 79);
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.FromArgb(31, 43, 77);
            btnGuardar.Location = new Point(286, 290);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(110, 36);
            btnGuardar.TabIndex = 7;
            btnGuardar.Text = "Desar canvis";
            btnGuardar.UseVisualStyleBackColor = false;
            // 
            // FrmAfegirJoc
            // 
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnCancelar;
            ClientSize = new Size(420, 346);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            Controls.Add(txtDescripcio);
            Controls.Add(lblDescripcio);
            Controls.Add(txtId);
            Controls.Add(lblId);
            Controls.Add(txtNom);
            Controls.Add(lblNom);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAfegirJoc";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Afegir joc";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblNom;
        private System.Windows.Forms.TextBox txtNom;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblDescripcio;
        private System.Windows.Forms.TextBox txtDescripcio;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
    }
}