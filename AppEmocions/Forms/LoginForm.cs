using System;
using System.Drawing;
using System.Net.Http;
using System.Windows.Forms;
using AppEmocions.Helpers;
using AppEmocions.Services;

namespace AppEmocions.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            Resize += (s, e) => pnlLeft.Width = (int)(ClientSize.Width * 0.55);
            pnlRight.Resize += (s, e) => CentrarTarjeta();
            CentrarTarjeta();

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

            if (string.IsNullOrEmpty(usuari) || string.IsNullOrEmpty(contrasenya))
            {
                MessageBox.Show(
                    "Introdueix el nom d'usuari i la contrasenya.",
                    "Dades incompletes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnEntrar.Enabled = false;
                var docente = await ApiClient.LoginAsync(usuari, contrasenya);
                SesionActual.Iniciar(docente);

                var dashboard = new FrmDashboard();
                dashboard.FormClosed += (s, args) => Close();

                Hide();
                dashboard.Show();
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(
                    "No s'ha pogut connectar amb el servidor (API). Comprova que AppEmocions.Api estigui en marxa.",
                    "Error de connexiÃ³",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "AccÃ©s denegat",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                btnEntrar.Enabled = true;
            }
        }

        private void pnlLeft_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}