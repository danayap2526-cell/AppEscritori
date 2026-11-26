using AppEmocions.Models;

namespace AppEmocions.Forms
{
    public partial class FrmAfegirJoc : Form
    {
        private readonly JocItem? _original;

        // El juego que se ha creado o editado (null si se cancela)
        public JocItem? Resultat { get; private set; }

        // Sin parámetro: ventana para AÑADIR. Con un juego: ventana para EDITAR.
        public FrmAfegirJoc(JocItem? joc = null)
        {
            InitializeComponent();
            _original = joc;

            if (joc != null)
            {
                Text = "Editar joc";
                txtNom.Text = joc.Nom;
                txtId.Text = joc.Identificador;
                txtDescripcio.Text = joc.Descripcio;
            }

            btnGuardar.Click += BtnGuardar_Click;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            // 1. Comprobar que están los campos obligatorios
            if (txtNom.Text.Trim() == "" || txtId.Text.Trim() == "")

            {
                MessageBox.Show("Cal omplir el nom i l'identificador.",
                    "Dades incompletes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Crear el juego (o modificar el que estamos editando)
            JocItem joc = _original ?? new JocItem();
            joc.Nom = txtNom.Text.Trim();
            joc.Identificador = txtId.Text.Trim();
            joc.Descripcio = txtDescripcio.Text.Trim();

            // 3. Guardarlo y cerrar la ventana diciendo "OK"
            Resultat = joc;
            DialogResult = DialogResult.OK;
        }
    }
}