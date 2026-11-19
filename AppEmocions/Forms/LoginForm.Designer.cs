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
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblSeguretat = new System.Windows.Forms.Label();
            this.pnlLinea = new System.Windows.Forms.Panel();
            this.lnkCanviar = new System.Windows.Forms.LinkLabel();
            this.btnEntrar = new System.Windows.Forms.Button();
            this.txtContrasenya = new System.Windows.Forms.TextBox();
            this.lblContrasenya = new System.Windows.Forms.Label();
            this.txtUsuari = new System.Windows.Forms.TextBox();
            this.lblUsuari = new System.Windows.Forms.Label();
            this.lblInstruccio = new System.Windows.Forms.Label();
            this.lblBenvinguda = new System.Windows.Forms.Label();
            this.lblAreaPrivada = new System.Windows.Forms.Label();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblEyebrow = new System.Windows.Forms.Label();
            this.lblSubMarca = new System.Windows.Forms.Label();
            this.lblMarca = new System.Windows.Forms.Label();
            this.pnlLogo = new System.Windows.Forms.Panel();
            this.pnlRight.SuspendLayout();
            this.pnlCard.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlRight
            //
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(249, 247, 243);
            this.pnlRight.Controls.Add(this.pnlCard);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(704, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(576, 720);
            this.pnlRight.TabIndex = 1;
            //
            // pnlCard
            //
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.Controls.Add(this.lblSeguretat);
            this.pnlCard.Controls.Add(this.pnlLinea);
            this.pnlCard.Controls.Add(this.lnkCanviar);
            this.pnlCard.Controls.Add(this.btnEntrar);
            this.pnlCard.Controls.Add(this.txtContrasenya);
            this.pnlCard.Controls.Add(this.lblContrasenya);
            this.pnlCard.Controls.Add(this.txtUsuari);
            this.pnlCard.Controls.Add(this.lblUsuari);
            this.pnlCard.Controls.Add(this.lblInstruccio);
            this.pnlCard.Controls.Add(this.lblBenvinguda);
            this.pnlCard.Controls.Add(this.lblAreaPrivada);
            this.pnlCard.Location = new System.Drawing.Point(88, 140);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(400, 440);
            this.pnlCard.TabIndex = 0;
            //
            // lblAreaPrivada
            //
            this.lblAreaPrivada.AutoSize = true;
            this.lblAreaPrivada.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblAreaPrivada.ForeColor = System.Drawing.Color.FromArgb(46, 139, 139);
            this.lblAreaPrivada.Location = new System.Drawing.Point(32, 36);
            this.lblAreaPrivada.Name = "lblAreaPrivada";
            this.lblAreaPrivada.TabIndex = 0;
            this.lblAreaPrivada.Text = "ÀREA PRIVADA";
            //
            // lblBenvinguda
            //
            this.lblBenvinguda.AutoSize = true;
            this.lblBenvinguda.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblBenvinguda.ForeColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.lblBenvinguda.Location = new System.Drawing.Point(28, 56);
            this.lblBenvinguda.Name = "lblBenvinguda";
            this.lblBenvinguda.TabIndex = 1;
            this.lblBenvinguda.Text = "Benvinguda, benvingut";
            //
            // lblInstruccio
            //
            this.lblInstruccio.AutoSize = true;
            this.lblInstruccio.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblInstruccio.ForeColor = System.Drawing.Color.FromArgb(120, 120, 130);
            this.lblInstruccio.Location = new System.Drawing.Point(32, 102);
            this.lblInstruccio.Name = "lblInstruccio";
            this.lblInstruccio.TabIndex = 2;
            this.lblInstruccio.Text = "Introdueix les teves dades per accedir al panell.";
            //
            // lblUsuari
            //
            this.lblUsuari.AutoSize = true;
            this.lblUsuari.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblUsuari.ForeColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.lblUsuari.Location = new System.Drawing.Point(32, 142);
            this.lblUsuari.Name = "lblUsuari";
            this.lblUsuari.TabIndex = 3;
            this.lblUsuari.Text = "Nom d'usuari";
            //
            // txtUsuari
            //
            this.txtUsuari.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuari.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsuari.Location = new System.Drawing.Point(32, 164);
            this.txtUsuari.Name = "txtUsuari";
            this.txtUsuari.Size = new System.Drawing.Size(336, 27);
            this.txtUsuari.TabIndex = 4;
            //
            // lblContrasenya
            //
            this.lblContrasenya.AutoSize = true;
            this.lblContrasenya.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblContrasenya.ForeColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.lblContrasenya.Location = new System.Drawing.Point(32, 212);
            this.lblContrasenya.Name = "lblContrasenya";
            this.lblContrasenya.TabIndex = 5;
            this.lblContrasenya.Text = "Contrasenya";
            //
            // txtContrasenya
            //
            this.txtContrasenya.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtContrasenya.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtContrasenya.Location = new System.Drawing.Point(32, 234);
            this.txtContrasenya.Name = "txtContrasenya";
            this.txtContrasenya.Size = new System.Drawing.Size(336, 27);
            this.txtContrasenya.TabIndex = 6;
            this.txtContrasenya.UseSystemPasswordChar = true;
            //
            // btnEntrar
            //
            this.btnEntrar.BackColor = System.Drawing.Color.FromArgb(245, 190, 79);
            this.btnEntrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEntrar.FlatAppearance.BorderSize = 0;
            this.btnEntrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEntrar.ForeColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.btnEntrar.Location = new System.Drawing.Point(32, 290);
            this.btnEntrar.Name = "btnEntrar";
            this.btnEntrar.Size = new System.Drawing.Size(336, 44);
            this.btnEntrar.TabIndex = 7;
            this.btnEntrar.Text = "Entrar  →";
            this.btnEntrar.UseVisualStyleBackColor = false;
            //
            // lnkCanviar
            //
            this.lnkCanviar.ActiveLinkColor = System.Drawing.Color.FromArgb(46, 139, 139);
            this.lnkCanviar.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lnkCanviar.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkCanviar.LinkColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.lnkCanviar.Location = new System.Drawing.Point(32, 348);
            this.lnkCanviar.Name = "lnkCanviar";
            this.lnkCanviar.Size = new System.Drawing.Size(336, 20);
            this.lnkCanviar.TabIndex = 8;
            this.lnkCanviar.TabStop = true;
            this.lnkCanviar.Text = "Canviar contrasenya";
            this.lnkCanviar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlLinea
            //
            this.pnlLinea.BackColor = System.Drawing.Color.FromArgb(232, 230, 226);
            this.pnlLinea.Location = new System.Drawing.Point(32, 384);
            this.pnlLinea.Name = "pnlLinea";
            this.pnlLinea.Size = new System.Drawing.Size(336, 1);
            this.pnlLinea.TabIndex = 9;
            //
            // lblSeguretat
            //
            this.lblSeguretat.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblSeguretat.ForeColor = System.Drawing.Color.FromArgb(120, 120, 130);
            this.lblSeguretat.Location = new System.Drawing.Point(32, 400);
            this.lblSeguretat.Name = "lblSeguretat";
            this.lblSeguretat.Size = new System.Drawing.Size(336, 18);
            this.lblSeguretat.TabIndex = 10;
            this.lblSeguretat.Text = "Connexió segura i dades pseudonimitzades";
            this.lblSeguretat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlLeft
            //
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(31, 43, 77);
            this.pnlLeft.Controls.Add(this.lblSubtitulo);
            this.pnlLeft.Controls.Add(this.lblTitulo);
            this.pnlLeft.Controls.Add(this.lblEyebrow);
            this.pnlLeft.Controls.Add(this.lblSubMarca);
            this.pnlLeft.Controls.Add(this.lblMarca);
            this.pnlLeft.Controls.Add(this.pnlLogo);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(704, 720);
            this.pnlLeft.TabIndex = 0;
            //
            // pnlLogo
            //
            this.pnlLogo.BackColor = System.Drawing.Color.FromArgb(245, 190, 79);
            this.pnlLogo.Location = new System.Drawing.Point(36, 24);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(36, 36);
            this.pnlLogo.TabIndex = 0;
            //
            // lblMarca
            //
            this.lblMarca.AutoSize = true;
            this.lblMarca.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMarca.ForeColor = System.Drawing.Color.White;
            this.lblMarca.Location = new System.Drawing.Point(82, 24);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.TabIndex = 1;
            this.lblMarca.Text = "Poble de les Emocions";
            //
            // lblSubMarca
            //
            this.lblSubMarca.AutoSize = true;
            this.lblSubMarca.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblSubMarca.ForeColor = System.Drawing.Color.FromArgb(143, 184, 232);
            this.lblSubMarca.Location = new System.Drawing.Point(82, 44);
            this.lblSubMarca.Name = "lblSubMarca";
            this.lblSubMarca.TabIndex = 2;
            this.lblSubMarca.Text = "Panell d'educadors";
            //
            // lblEyebrow
            //
            this.lblEyebrow.AutoSize = true;
            this.lblEyebrow.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblEyebrow.ForeColor = System.Drawing.Color.FromArgb(245, 190, 79);
            this.lblEyebrow.Location = new System.Drawing.Point(36, 106);
            this.lblEyebrow.Name = "lblEyebrow";
            this.lblEyebrow.TabIndex = 3;
            this.lblEyebrow.Text = "EDUCACIÓ EMOCIONAL, AMB CURA";
            //
            // lblTitulo
            //
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(36, 126);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(360, 90);
            this.lblTitulo.TabIndex = 4;
            this.lblTitulo.Text = "Una mirada amable a cada pas del seu aprenentatge.";
            //
            // lblSubtitulo
            //
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(213, 219, 234);
            this.lblSubtitulo.Location = new System.Drawing.Point(36, 225);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(300, 50);
            this.lblSubtitulo.TabIndex = 5;
            this.lblSubtitulo.Text = "Observa el progrés, acompanya les emocions i celebra cada petit assoliment.";
            //
            // LoginForm
            //
            this.AcceptButton = this.btnEntrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Poble de les Emocions";
            this.pnlRight.ResumeLayout(false);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.ResumeLayout(false);
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