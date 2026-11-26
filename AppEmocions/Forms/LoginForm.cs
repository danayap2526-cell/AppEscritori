using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AppEmocions.Helpers;
using AppEmocions.Models.Usuaris;
using AppEmocions.Services;

namespace AppEmocions.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Mantiene el panel izquierdo al 55 % del ancho al cambiar el tamaño
            Resize += (s, e) => pnlLeft.Width = (int)(ClientSize.Width * 0.55);

            // Mantiene la tarjeta centrada en el panel derecho
            pnlRight.Resize += (s, e) => CentrarTarjeta();
            CentrarTarjeta();

            // Conecta el botón con su método
            btnEntrar.Click += BtnEntrar_Click;
        }

        private void CentrarTarjeta()
        {
            pnlCard.Left = (pnlRight.ClientSize.Width - pnlCard.Width) / 2;
            pnlCard.Top = (pnlRight.ClientSize.Height - pnlCard.Height) / 2;
        }

        private async void BtnEntrar_Click(object? sender, EventArgs e)
        {
            string usuari = txtUsuari.Text.Trim();

            string contrasenya = txtContrasenya.Text;

            // 1. Validación básica: campos vacíos
            if (string.IsNullOrEmpty(usuari) || string.IsNullOrEmpty(contrasenya))
            {
                MessageBox.Show(
                    "Introdueix el nom d'usuari i la contrasenya.",
                    "Dades incompletes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. TODO: aquí irá la validación real contra la API.

            // 3. Abrir el dashboard de Diego
            var dashboard = new FrmDashboard();

            // Cuando se cierre el dashboard, se cierra también el login
            // y con él termina la aplicación.
            dashboard.FormClosed += (s, args) => Close();

            Hide();            // oculta el login (no lo cierra)
            dashboard.Show();
        }

        private void pnlLeft_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}