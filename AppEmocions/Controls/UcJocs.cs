using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AppEmocions.Forms;
using AppEmocions.Models;

namespace AppEmocions.Controls
{
    public partial class UcJocs : UserControl
    {
        // Lista de juegos en memoria (más adelante vendrá de la API)
        private readonly List<JocItem> _jocs = new List<JocItem>();

        public UcJocs()
        {
            InitializeComponent();

            // Datos de prueba
            _jocs.Add(new JocItem
            {
                Nom = "El Poble de les Emocions",
                Identificador = "POBLE_EMOCIONS",
                Descripcio = "Reconeixement, comprensió i regulació emocional.",
                Actiu = true,
                Icono = TipoIcono.Corazon
            });
            _jocs.Add(new JocItem
            {
                Nom = "El Bosc dels Sons",
                Identificador = "BOSC_SONS",
                Descripcio = "Discriminació auditiva i consciència fonològica.",
                Actiu = false,
                Icono = TipoIcono.Sol
            });
            _jocs.Add(new JocItem
            {
                Nom = "Camins i Formes",
                Identificador = "CAMINS_FORMES",
                Descripcio = "Orientació espacial i pensament visual.",
                Actiu = false,
                Icono = TipoIcono.Estrella
            });

            btnAfegir.Click += BtnAfegir_Click;

            CargarTarjetas();
        }

        // Crea una tarjeta por cada juego de la lista
        private void CargarTarjetas()
        {
            flowJocs.Controls.Clear();

            foreach (JocItem joc in _jocs)
            {
                var card = new JocCard();
                card.Mostrar(joc);
                card.Margin = new Padding(0, 0, 16, 16);
                card.EditarClick += (s, e) => EditarJoc(joc);
                card.EliminarClick += (s, e) => EliminarJoc(joc);
                flowJocs.Controls.Add(card);
            }
        }

        // Botón "Afegir joc": abre la ventana vacía
        private void BtnAfegir_Click(object? sender, EventArgs e)
        {
            var dlg = new FrmAfegirJoc();

            if (dlg.ShowDialog(this) == DialogResult.OK && dlg.Resultat != null)
            {
                _jocs.Add(dlg.Resultat);
                CargarTarjetas();
            }
        }

        // Botón "Editar" de una tarjeta: abre la ventana con sus datos
        private void EditarJoc(JocItem joc)
        {
            var dlg = new FrmAfegirJoc(joc);

            // La ventana modifica el propio juego, así que basta con repintar
            if (dlg.ShowDialog(this) == DialogResult.OK)
                CargarTarjetas();
        }

        private void EliminarJoc(JocItem joc)
        {
            DialogResult confirmacio = MessageBox.Show(
                this,
                $"Segur que vols eliminar el joc «{joc.Nom}»?",
                "Eliminar joc",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacio == DialogResult.Yes)
            {
                _jocs.Remove(joc);
                CargarTarjetas();
            }
        }
    }
}
