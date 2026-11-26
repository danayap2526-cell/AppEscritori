using System;
using System.Drawing;
using System.Windows.Forms;
using AppEmocions.Models;

namespace AppEmocions.Controls
{
    public partial class JocCard : UserControl
    {
        private JocItem? _joc;

        // Avisa al formulario de las acciones de la tarjeta
        public event EventHandler? EditarClick;
        public event EventHandler? EliminarClick;

        public JocItem? Joc => _joc;

        public JocCard()
        {
            InitializeComponent();

            btnEditar.Click += (s, e) => EditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => EliminarClick?.Invoke(this, EventArgs.Empty);
            chkActiu.CheckedChanged += ChkActiu_CheckedChanged;
        }

        // Rellena la tarjeta con los datos de un juego
        public void Mostrar(JocItem joc)
        {
            _joc = joc;

            lblNom.Text = joc.Nom;
            lblId.Text = joc.Identificador;
            lblDescripcio.Text = joc.Descripcio;

            chkActiu.Checked = joc.Actiu;

            PonerEstilo(joc.Icono);
        }

        // Si el usuario marca o desmarca "Actiu", se guarda en el juego
        private void ChkActiu_CheckedChanged(object? sender, EventArgs e)
        {
            if (_joc != null)
                _joc.Actiu = chkActiu.Checked;
        }

        // Cada tipo de juego tiene su color de cabecera y su símbolo
        private void PonerEstilo(TipoIcono icono)
        {
            switch (icono)
            {
                case TipoIcono.Corazon:
                    pnlCabecera.BackColor = ColorTranslator.FromHtml("#1F2B4D");
                    lblIcono.ForeColor = ColorTranslator.FromHtml("#F5BE4F");
                    lblIcono.Text = "♥";
                    break;

                case TipoIcono.Sol:
                    pnlCabecera.BackColor = ColorTranslator.FromHtml("#E8F3F0");
                    lblIcono.ForeColor = ColorTranslator.FromHtml("#2E8B8B");
                    lblIcono.Text = "☀";
                    break;

                case TipoIcono.Estrella:
                    pnlCabecera.BackColor = ColorTranslator.FromHtml("#EDE8F7");
                    lblIcono.ForeColor = ColorTranslator.FromHtml("#8E6BBF");

                    lblIcono.Text = "✦";
                    break;
            }
        }
    }
}