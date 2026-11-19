using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppEmocions.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            // Mantiene el panel izquierdo al 55 % del ancho al cambiar el tamaño
            Resize += (s, e) => pnlLeft.Width = (int)(ClientSize.Width * 0.55);

            // Cada vez que cambia el panel derecho, recentramos la tarjeta
            pnlRight.Resize += (s, e) => CentrarTarjeta();
            CentrarTarjeta();
        }

        private void CentrarTarjeta()
        {
            pnlCard.Left = (pnlRight.ClientSize.Width - pnlCard.Width) / 2;
            pnlCard.Top = (pnlRight.ClientSize.Height - pnlCard.Height) / 2;
        }
    }
}