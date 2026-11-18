
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace AppEmocions.Forms
{
    public partial class FrmDashboard : Form
    {
        // Paneles principales
        private Guna2Panel panelMenu;
        private Guna2Panel panelContenido;
        private Guna2Panel panelCabeceraMenu;
        private Guna2Panel panelNavegacion;

        // Botones del menú
        private readonly List<Guna2Button> botonesMenu = new();

        // Colores
        private readonly Color azulMenu = Color.FromArgb(45, 65, 99);
        private readonly Color azulSeleccionado = Color.FromArgb(59, 82, 120);
        private readonly Color amarillo = Color.FromArgb(255, 198, 83);
        private readonly Color fondo = Color.FromArgb(248, 247, 244);
        private readonly Color textoMenu = Color.FromArgb(220, 229, 242);

        public FrmDashboard()
        {
            InitializeComponent();

            ConfigurarFormulario();
            CrearPanelContenido();
            CrearMenuLateral();
        }

        // -----------------------------------------
        // CONFIGURACIÓN DEL FORMULARIO
        // -----------------------------------------

        private void ConfigurarFormulario()
        {
            Text = "Poble de les Emocions";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1200, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            Font = new Font("Segoe UI", 10);
        }

        // -----------------------------------------
        // PANEL PRINCIPAL DE CONTENIDO
        // -----------------------------------------

        private void CrearPanelContenido()
        {
            panelContenido = new Guna2Panel();

            panelContenido.Dock = DockStyle.Fill;
            panelContenido.FillColor = fondo;
            panelContenido.BorderRadius = 0;

            Controls.Add(panelContenido);
        }

        // -----------------------------------------
        // MENÚ LATERAL
        // -----------------------------------------

        private void CrearMenuLateral()
        {
            panelMenu = new Guna2Panel();

            panelMenu.Dock = DockStyle.Left;
            panelMenu.Width = 300;
            panelMenu.FillColor = azulMenu;
            panelMenu.BorderRadius = 0;

            Controls.Add(panelMenu);
            panelMenu.BringToFront();

            CrearCabeceraMenu();
            CrearNavegacion();
        }

        // -----------------------------------------
        // CABECERA DEL MENÚ
        // -----------------------------------------

        private void CrearCabeceraMenu()
        {
            panelCabeceraMenu = new Guna2Panel();

            panelCabeceraMenu.Dock = DockStyle.Top;
            panelCabeceraMenu.Height = 90;
            panelCabeceraMenu.FillColor = azulMenu;
            panelCabeceraMenu.BorderRadius = 0;

            // Logotipo
            Guna2Panel logo = new Guna2Panel();

            logo.Size = new Size(46, 46);
            logo.Location = new Point(18, 18);
            logo.FillColor = amarillo;
            logo.BorderRadius = 12;

            Label lblLogo = new Label();

            lblLogo.Text = "P";
            lblLogo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblLogo.ForeColor = azulMenu;
            lblLogo.BackColor = Color.Transparent;
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            logo.Controls.Add(lblLogo);

            // Nombre del proyecto
            Label lblTitulo = new Label();

            lblTitulo.Text = "Poble de les Emocions";
            lblTitulo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.AutoSize = false;
            lblTitulo.Size = new Size(220, 25);
            lblTitulo.Location = new Point(76, 18);

            // Subtítulo
            Label lblSubtitulo = new Label();

            lblSubtitulo.Text = "Panell d'educadors";
            lblSubtitulo.Font = new Font("Segoe UI", 9);
            lblSubtitulo.ForeColor = Color.FromArgb(205, 216, 235);
            lblSubtitulo.BackColor = Color.Transparent;
            lblSubtitulo.AutoSize = true;
            lblSubtitulo.Location = new Point(76, 45);

            panelCabeceraMenu.Controls.Add(logo);
            panelCabeceraMenu.Controls.Add(lblTitulo);
            panelCabeceraMenu.Controls.Add(lblSubtitulo);

            panelMenu.Controls.Add(panelCabeceraMenu);
        }

        // -----------------------------------------
        // NAVEGACIÓN
        // -----------------------------------------

        private void CrearNavegacion()
        {
            panelNavegacion = new Guna2Panel();

            panelNavegacion.Dock = DockStyle.Fill;
            panelNavegacion.FillColor = azulMenu;
            panelNavegacion.BorderRadius = 0;

            panelMenu.Controls.Add(panelNavegacion);
            panelNavegacion.BringToFront();

            string[] nombres =
            {
                "Tauler",
                "Infants",
                "Comparativa",
                "Abandonament",
                "Jocs",
                "Usuaris",
                "Dades",
                "Configuració"
            };

            string[] iconos =
            {
                "▦",
                "♙",
                "▥",
                "☰",
                "🎮",
                "♢",
                "▤",
                "⚙"
            };

            for (int i = 0; i < nombres.Length; i++)
            {
                Guna2Button boton = CrearBotonMenu(
                    nombres[i],
                    iconos[i],
                    i == 0
                );

                boton.Location = new Point(13, 15 + (i * 62));

                panelNavegacion.Controls.Add(boton);
                botonesMenu.Add(boton);
            }
        }

        // -----------------------------------------
        // CREAR BOTÓN DEL MENÚ
        // -----------------------------------------


        private Guna2Button CrearBotonMenu(
            string texto,
            string icono,
            bool seleccionado)
        {
            Guna2Button boton = new Guna2Button();

            boton.Size = new Size(274, 52);
            boton.BorderRadius = 12;

            boton.FillColor = seleccionado
                ? azulSeleccionado
                : azulMenu;

            boton.BackColor = azulMenu;

            // Eliminar bordes y sombras
            boton.BorderThickness = 0;
            boton.BorderColor = azulMenu;
            boton.ShadowDecoration.Enabled = false;
            boton.UseTransparentBackground = false;

            // Texto
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10);
            boton.Text = icono + "    " + texto;
            boton.TextAlign = HorizontalAlignment.Left;
            boton.TextOffset = new Point(12, 0);

            // Cursor
            boton.Cursor = Cursors.Hand;

            // Efecto al pasar el ratón
            boton.HoverState.FillColor =
                Color.FromArgb(59, 82, 120);

            boton.HoverState.BorderColor = azulMenu;

            boton.PressedColor = amarillo;

            boton.Click += (sender, e) =>
            {
                SeleccionarBoton(boton);
            };

            return boton;
        }


        // -----------------------------------------
        // SELECCIONAR BOTÓN
        // -----------------------------------------

        private void SeleccionarBoton(Guna2Button seleccionado)
        {
            foreach (Guna2Button boton in botonesMenu)
            {
                boton.FillColor = azulMenu;
            }

            seleccionado.FillColor = azulSeleccionado;

            // Más adelante cambiaremos aquí
            // el contenido según la pantalla elegida.
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
        }
    }
}
