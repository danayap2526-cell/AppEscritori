namespace AppEmocions.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlRight = new Panel();
            pnlCard = new Panel();
            lblSeguretat = new Label();
            pnlLinea = new Panel();
            lnkCanviar = new LinkLabel();
            btnEntrar = new Button();
            txtContrasenya = new TextBox();
            lblContrasenya = new Label();
            txtUsuari = new TextBox();
            lblUsuari = new Label();
            lblInstruccio = new Label();
            lblBenvinguda = new Label();
            lblAreaPrivada = new Label();
            pnlLeft = new Panel();
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            lblEyebrow = new Label();
            lblSubMarca = new Label();
            lblMarca = new Label();
            pnlLogo = new Panel();
            pnlRight.SuspendLayout();
            pnlCard.SuspendLayout();
            pnlLeft.SuspendLayout();
            SuspendLayout();
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(249, 247, 243);
            pnlRight.Controls.Add(pnlCard);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(704, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(576, 720);
            pnlRight.TabIndex = 1;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.Controls.Add(lblSeguretat);
            pnlCard.Controls.Add(pnlLinea);
            pnlCard.Controls.Add(lnkCanviar);
            pnlCard.Controls.Add(btnEntrar);
            pnlCard.Controls.Add(txtContrasenya);
            pnlCard.Controls.Add(lblContrasenya);
            pnlCard.Controls.Add(txtUsuari);
            pnlCard.Controls.Add(lblUsuari);
            pnlCard.Controls.Add(lblInstruccio);
            pnlCard.Controls.Add(lblBenvinguda);
            pnlCard.Controls.Add(lblAreaPrivada);
            pnlCard.Location = new Point(88, 140);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(400, 440);
            pnlCard.TabIndex = 0;
            // 
            // lblSeguretat
            // 
            lblSeguretat.Font = new Font("Segoe UI", 7.5F);
            lblSeguretat.ForeColor = Color.FromArgb(120, 120, 130);
            lblSeguretat.Location = new Point(32, 400);
            lblSeguretat.Name = "lblSeguretat";
            lblSeguretat.Size = new Size(336, 18);
            lblSeguretat.TabIndex = 10;
            lblSeguretat.Text = "Connexió segura i dades pseudonimitzades";
            lblSeguretat.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlLinea
            // 
            pnlLinea.BackColor = Color.FromArgb(232, 230, 226);
            pnlLinea.Location = new Point(32, 384);
            pnlLinea.Name = "pnlLinea";
            pnlLinea.Size = new Size(336, 1);
            pnlLinea.TabIndex = 9;
            // 
            // lnkCanviar
            // 
            lnkCanviar.ActiveLinkColor = Color.FromArgb(46, 139, 139);
            lnkCanviar.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lnkCanviar.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkCanviar.LinkColor = Color.FromArgb(31, 43, 77);
            lnkCanviar.Location = new Point(32, 348);
            lnkCanviar.Name = "lnkCanviar";
            lnkCanviar.Size = new Size(336, 20);
            lnkCanviar.TabIndex = 8;
            lnkCanviar.TabStop = true;
            lnkCanviar.Text = "Canviar contrasenya";
            lnkCanviar.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(245, 190, 79);
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEntrar.ForeColor = Color.FromArgb(31, 43, 77);
            btnEntrar.Location = new Point(32, 290);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(336, 44);
            btnEntrar.TabIndex = 7;
            btnEntrar.Text = "Entrar  →";
            btnEntrar.UseVisualStyleBackColor = false;
            // 
            // txtContrasenya
            // 
            txtContrasenya.BorderStyle = BorderStyle.FixedSingle;
            txtContrasenya.Font = new Font("Segoe UI", 11F);
            txtContrasenya.Location = new Point(32, 234);
            txtContrasenya.Name = "txtContrasenya";
            txtContrasenya.Size = new Size(336, 27);
            txtContrasenya.TabIndex = 6;
            txtContrasenya.UseSystemPasswordChar = true;
            // 
            // lblContrasenya
            // 
            lblContrasenya.AutoSize = true;
            lblContrasenya.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblContrasenya.ForeColor = Color.FromArgb(31, 43, 77);
            lblContrasenya.Location = new Point(32, 212);
            lblContrasenya.Name = "lblContrasenya";
            lblContrasenya.Size = new Size(75, 15);
            lblContrasenya.TabIndex = 5;
            lblContrasenya.Text = "Contrasenya";
            // 
            // txtUsuari
            // 
            txtUsuari.BorderStyle = BorderStyle.FixedSingle;
            txtUsuari.Font = new Font("Segoe UI", 11F);
            txtUsuari.Location = new Point(32, 164);
            txtUsuari.Name = "txtUsuari";
            txtUsuari.Size = new Size(336, 27);
            txtUsuari.TabIndex = 4;
            // 
            // lblUsuari
            // 
            lblUsuari.AutoSize = true;
            lblUsuari.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblUsuari.ForeColor = Color.FromArgb(31, 43, 77);
            lblUsuari.Location = new Point(32, 142);
            lblUsuari.Name = "lblUsuari";
            lblUsuari.Size = new Size(81, 15);
            lblUsuari.TabIndex = 3;
            lblUsuari.Text = "Nom d'usuari";
            // 
            // lblInstruccio
            // 
            lblInstruccio.AutoSize = true;
            lblInstruccio.Font = new Font("Segoe UI", 8.5F);
            lblInstruccio.ForeColor = Color.FromArgb(120, 120, 130);
            lblInstruccio.Location = new Point(32, 102);
            lblInstruccio.Name = "lblInstruccio";
            lblInstruccio.Size = new Size(252, 15);
            lblInstruccio.TabIndex = 2;
            lblInstruccio.Text = "Introdueix les teves dades per accedir al panell.";
            // 
            // lblBenvinguda
            // 
            lblBenvinguda.AutoSize = true;
            lblBenvinguda.Font = new Font("Segoe UI", 18F);
            lblBenvinguda.ForeColor = Color.FromArgb(31, 43, 77);
            lblBenvinguda.Location = new Point(28, 56);
            lblBenvinguda.Name = "lblBenvinguda";
            lblBenvinguda.Size = new Size(262, 32);
            lblBenvinguda.TabIndex = 1;
            lblBenvinguda.Text = "Benvinguda, benvingut";
            // 
            // lblAreaPrivada
            // 
            lblAreaPrivada.AutoSize = true;
            lblAreaPrivada.Font = new Font("Segoe UI", 7.5F, FontStyle.Bold);
            lblAreaPrivada.ForeColor = Color.FromArgb(46, 139, 139);
            lblAreaPrivada.Location = new Point(32, 36);
            lblAreaPrivada.Name = "lblAreaPrivada";
            lblAreaPrivada.Size = new Size(77, 12);
            lblAreaPrivada.TabIndex = 0;
            lblAreaPrivada.Text = "ÀREA PRIVADA";
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(31, 43, 77);
            pnlLeft.Controls.Add(lblSubtitulo);
            pnlLeft.Controls.Add(lblTitulo);
            pnlLeft.Controls.Add(lblEyebrow);
            pnlLeft.Controls.Add(lblSubMarca);
            pnlLeft.Controls.Add(lblMarca);
            pnlLeft.Controls.Add(pnlLogo);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(704, 720);
            pnlLeft.TabIndex = 0;
            pnlLeft.Paint += pnlLeft_Paint;
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Font = new Font("Segoe UI", 10F);
            lblSubtitulo.ForeColor = Color.FromArgb(213, 219, 234);
            lblSubtitulo.Location = new Point(36, 225);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(300, 50);
            lblSubtitulo.TabIndex = 5;
            lblSubtitulo.Text = "Observa el progrés, acompanya les emocions i celebra cada petit assoliment.";
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 20F);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(36, 126);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(360, 90);
            lblTitulo.TabIndex = 4;
            lblTitulo.Text = "Una mirada amable a cada pas del seu aprenentatge.";
            // 
            // lblEyebrow
            // 
            lblEyebrow.AutoSize = true;
            lblEyebrow.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblEyebrow.ForeColor = Color.FromArgb(245, 190, 79);
            lblEyebrow.Location = new Point(36, 106);
            lblEyebrow.Name = "lblEyebrow";
            lblEyebrow.Size = new Size(196, 13);
            lblEyebrow.TabIndex = 3;
            lblEyebrow.Text = "EDUCACIÓ EMOCIONAL, AMB CURA";
            // 
            // lblSubMarca
            // 
            lblSubMarca.AutoSize = true;
            lblSubMarca.Font = new Font("Segoe UI", 8F);
            lblSubMarca.ForeColor = Color.FromArgb(143, 184, 232);
            lblSubMarca.Location = new Point(82, 44);
            lblSubMarca.Name = "lblSubMarca";
            lblSubMarca.Size = new Size(105, 13);
            lblSubMarca.TabIndex = 2;
            lblSubMarca.Text = "Panell d'educadors";
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblMarca.ForeColor = Color.White;
            lblMarca.Location = new Point(82, 24);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(158, 19);
            lblMarca.TabIndex = 1;
            lblMarca.Text = "Poble de les Emocions";
            // 
            // pnlLogo
            // 
            pnlLogo.BackColor = Color.FromArgb(245, 190, 79);
            pnlLogo.Location = new Point(36, 24);
            pnlLogo.Name = "pnlLogo";
            pnlLogo.Size = new Size(36, 36);
            pnlLogo.TabIndex = 0;
            // 
            // LoginForm
            // 
            AcceptButton = btnEntrar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 720);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            DoubleBuffered = true;
            MinimumSize = new Size(900, 600);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Poble de les Emocions";
            pnlRight.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlLogo;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.Label lblSubMarca;
        private System.Windows.Forms.Label lblEyebrow;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblAreaPrivada;
        private System.Windows.Forms.Label lblBenvinguda;
        private System.Windows.Forms.Label lblInstruccio;
        private System.Windows.Forms.Label lblUsuari;
        private System.Windows.Forms.TextBox txtUsuari;
        private System.Windows.Forms.Label lblContrasenya;
        private System.Windows.Forms.TextBox txtContrasenya;
        private System.Windows.Forms.Button btnEntrar;
        private System.Windows.Forms.LinkLabel lnkCanviar;
        private System.Windows.Forms.Panel pnlLinea;
        private System.Windows.Forms.Label lblSeguretat;
    }
}