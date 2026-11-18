
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace AppEmocions.Forms
{
    public partial class FrmDashboard : Form
    {
        // COLORES
        private readonly Color azulMenu = Color.FromArgb(32, 45, 73);
        private readonly Color azulSeleccionado = Color.FromArgb(53, 69, 101);
        private readonly Color amarillo = Color.FromArgb(255, 198, 83);
        private readonly Color fondo = Color.FromArgb(248, 247, 244);
        private readonly Color turquesa = Color.FromArgb(54, 184, 177);
        private readonly Color texto = Color.FromArgb(35, 52, 79);
        private readonly Color gris = Color.FromArgb(130, 145, 165);

        // PANELES
        private Guna2Panel panelMenu = null!;
        private Guna2Panel panelContenido = null!;
        private Guna2Panel panelCabecera = null!;
        private Guna2Panel panelNavegacion = null!;

        private readonly List<Guna2Button> botonesMenu = new();

        public FrmDashboard()
        {
            InitializeComponent();

            ConfigurarFormulario();
            CrearContenido();
            CrearMenuLateral();
        }

        // =========================================
        // FORMULARIO
        // =========================================

        private void ConfigurarFormulario()
        {
            Text = "Poble de les Emocions";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = fondo;
            Font = new Font("Segoe UI", 9);
        }

        // =========================================
        // CONTENIDO PRINCIPAL
        // =========================================

        private void CrearContenido()
        {
            panelContenido = new Guna2Panel();
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.FillColor = fondo;
            panelContenido.BorderRadius = 0;

            Controls.Add(panelContenido);

            CrearCabeceraSuperior();
            CrearZonaDashboard();
        }

        // =========================================
        // MENÚ LATERAL
        // =========================================

        private void CrearMenuLateral()
        {
            panelMenu = new Guna2Panel();
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Width = 300;
            panelMenu.FillColor = azulMenu;
            panelMenu.BackColor = azulMenu;
            panelMenu.BorderRadius = 0;

            Controls.Add(panelMenu);
            panelMenu.BringToFront();

            CrearCabeceraMenu();
            CrearNavegacion();
            CrearPieMenu();
        }

        private void CrearCabeceraMenu()
        {
            Guna2Panel cabecera = new Guna2Panel();
            cabecera.Dock = DockStyle.Top;
            cabecera.Height = 88;
            cabecera.FillColor = azulMenu;
            cabecera.BackColor = azulMenu;

            Guna2Panel logo = new Guna2Panel();
            logo.Size = new Size(46, 46);
            logo.Location = new Point(22, 15);
            logo.FillColor = amarillo;
            logo.BackColor = azulMenu;
            logo.BorderRadius = 13;

            Label lblLogo = new Label();
            lblLogo.Text = "P";
            lblLogo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblLogo.ForeColor = azulMenu;
            lblLogo.BackColor = Color.Transparent;
            lblLogo.Dock = DockStyle.Fill;
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            logo.Controls.Add(lblLogo);

            Label titulo = new Label();
            titulo.Text = "Poble de les Emocions";
            titulo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.BackColor = Color.Transparent;
            titulo.AutoSize = true;
            titulo.Location = new Point(85, 18);

            Label subtitulo = new Label();
            subtitulo.Text = "Panell d'educadors";
            subtitulo.Font = new Font("Segoe UI", 9);
            subtitulo.ForeColor = Color.FromArgb(190, 205, 225);
            subtitulo.BackColor = Color.Transparent;
            subtitulo.AutoSize = true;
            subtitulo.Location = new Point(85, 43);

            cabecera.Controls.Add(logo);
            cabecera.Controls.Add(titulo);
            cabecera.Controls.Add(subtitulo);

            panelMenu.Controls.Add(cabecera);
        }

        private void CrearNavegacion()
        {
            panelNavegacion = new Guna2Panel();
            panelNavegacion.Dock = DockStyle.Fill;
            panelNavegacion.FillColor = azulMenu;
            panelNavegacion.BackColor = azulMenu;

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
                "▦", "♙", "▥", "☰",
                "♧", "♢", "▤", "⚙"
            };

            for (int i = 0; i < nombres.Length; i++)
            {
                Guna2Button boton = CrearBotonMenu(
                    nombres[i],
                    iconos[i],
                    i == 0
                );

                boton.Location = new Point(13, 14 + i * 59);
                panelNavegacion.Controls.Add(boton);
                botonesMenu.Add(boton);
            }
        }

        private Guna2Button CrearBotonMenu(
            string nombre,
            string icono,
            bool seleccionado)
        {
            Guna2Button boton = new Guna2Button();

            boton.Size = new Size(274, 53);
            boton.BorderRadius = 12;
            boton.BorderThickness = 0;
            boton.BorderColor = azulMenu;

            boton.FillColor = seleccionado
                ? azulSeleccionado
                : azulMenu;

            boton.BackColor = azulMenu;
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10);
            boton.Text = icono + "     " + nombre;
            boton.TextAlign = HorizontalAlignment.Left;
            boton.TextOffset = new Point(12, 0);

            boton.Cursor = Cursors.Hand;
            boton.UseTransparentBackground = false;
            boton.ShadowDecoration.Enabled = false;

            boton.HoverState.FillColor = azulSeleccionado;
            boton.HoverState.BorderColor = azulMenu;

            boton.Click += (s, e) => SeleccionarBoton(boton);

            return boton;
        }

        private void SeleccionarBoton(Guna2Button seleccionado)
        {
            foreach (Guna2Button boton in botonesMenu)
                boton.FillColor = azulMenu;

            seleccionado.FillColor = azulSeleccionado;
        }

        private void CrearPieMenu()
        {
            Guna2Panel pie = new Guna2Panel();

            pie.Dock = DockStyle.Bottom;
            pie.Height = 125;
            pie.FillColor = Color.FromArgb(39, 53, 79);
            pie.BackColor = azulMenu;
            pie.BorderRadius = 16;

            Label dibujo = new Label();
            dibujo.Text = "     ☀\n  ▥  ♡  ▥";
            dibujo.Font = new Font("Segoe UI Symbol", 19);
            dibujo.ForeColor = amarillo;
            dibujo.BackColor = Color.Transparent;
            dibujo.TextAlign = ContentAlignment.MiddleCenter;
            dibujo.Location = new Point(15, 5);
            dibujo.Size = new Size(240, 75);

            Label titulo = new Label();
            titulo.Text = "La Brisa t'acompanya";
            titulo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titulo.ForeColor = Color.White;
            titulo.BackColor = Color.Transparent;
            titulo.Location = new Point(15, 80);
            titulo.AutoSize = true;

            pie.Controls.Add(dibujo);
            pie.Controls.Add(titulo);

            panelMenu.Controls.Add(pie);
            pie.BringToFront();
        }

        // =========================================
        // CABECERA SUPERIOR
        // =========================================

        private void CrearCabeceraSuperior()
        {
            panelCabecera = new Guna2Panel();

            panelCabecera.Dock = DockStyle.Top;
            panelCabecera.Height = 94;
            panelCabecera.FillColor = Color.White;
            panelCabecera.BackColor = Color.White;
            panelCabecera.BorderRadius = 0;

            Label curso = new Label();
            curso.Text = "P4 · CURS 2026-27";
            curso.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            curso.ForeColor = gris;
            curso.AutoSize = true;
            curso.Location = new Point(32, 16);

            Label saludo = new Label();
            saludo.Text = "Bon dia";
            saludo.Font = new Font("Segoe UI", 21);
            saludo.ForeColor = texto;
            saludo.AutoSize = true;
            saludo.Location = new Point(31, 35);

            panelCabecera.Controls.Add(curso);
            panelCabecera.Controls.Add(saludo);

            FlowLayoutPanel controles = new FlowLayoutPanel();
            controles.FlowDirection = FlowDirection.LeftToRight;
            controles.WrapContents = false;
            controles.AutoScroll = false;
            controles.BackColor = Color.White;
            controles.Size = new Size(790, 55);
            controles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controles.Location = new Point(450, 19);

            Guna2ComboBox grupos = new Guna2ComboBox();
            grupos.Size = new Size(132, 45);
            grupos.BorderRadius = 10;
            grupos.FillColor = Color.White;
            grupos.ForeColor = texto;
            grupos.Font = new Font("Segoe UI", 9);
            grupos.Items.AddRange(new object[]
            {
                "P4-A i P4-B",
                "P4-A",
                "P4-B"
            });
            grupos.SelectedIndex = 0;
            grupos.Margin = new Padding(4, 0, 5, 0);

            Guna2Button fechas = new Guna2Button();
            fechas.Size = new Size(156, 45);
            fechas.BorderRadius = 10;
            fechas.FillColor = Color.White;
            fechas.ForeColor = texto;
            fechas.BorderColor = Color.FromArgb(225, 229, 235);
            fechas.BorderThickness = 1;
            fechas.Font = new Font("Segoe UI", 9);
            fechas.Text = "▣  1–30 nov. 2026";
            fechas.Margin = new Padding(4, 0, 5, 0);

            Guna2TextBox buscar = new Guna2TextBox();
            buscar.Size = new Size(188, 45);
            buscar.BorderRadius = 10;
            buscar.FillColor = Color.FromArgb(249, 250, 252);
            buscar.ForeColor = texto;
            buscar.Font = new Font("Segoe UI", 9);
            buscar.PlaceholderText = "Cercar...";
            buscar.Margin = new Padding(4, 0, 5, 0);

            Guna2Button notificaciones = new Guna2Button();
            notificaciones.Size = new Size(46, 45);
            notificaciones.BorderRadius = 10;
            notificaciones.FillColor = Color.White;
            notificaciones.ForeColor = gris;
            notificaciones.BorderColor = Color.FromArgb(225, 229, 235);
            notificaciones.BorderThickness = 1;
            notificaciones.Font = new Font("Segoe UI Symbol", 17);
            notificaciones.Text = "♧";
            notificaciones.Margin = new Padding(4, 0, 5, 0);

            Guna2Panel perfil = new Guna2Panel();
            perfil.Size = new Size(220, 48);
            perfil.FillColor = Color.White;
            perfil.BackColor = Color.White;
            perfil.BorderRadius = 0;
            perfil.Margin = new Padding(5, 0, 0, 0);

            Guna2Panel avatar = new Guna2Panel();
            avatar.Size = new Size(42, 42);
            avatar.Location = new Point(2, 2);
            avatar.BorderRadius = 21;
            avatar.FillColor = Color.FromArgb(220, 239, 238);

            Label iniciales = new Label();
            iniciales.Text = "MS";
            iniciales.Dock = DockStyle.Fill;
            iniciales.BackColor = Color.Transparent;
            iniciales.ForeColor = Color.FromArgb(31, 130, 130);
            iniciales.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            iniciales.TextAlign = ContentAlignment.MiddleCenter;

            avatar.Controls.Add(iniciales);

            Label nombre = new Label();
            nombre.Text = "Marta Soler";
            nombre.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            nombre.ForeColor = texto;
            nombre.AutoSize = true;
            nombre.Location = new Point(52, 4);

            Label rol = new Label();
            rol.Text = "Educadora";
            rol.Font = new Font("Segoe UI", 8);
            rol.ForeColor = gris;
            rol.AutoSize = true;
            rol.Location = new Point(52, 25);

            perfil.Controls.Add(avatar);
            perfil.Controls.Add(nombre);
            perfil.Controls.Add(rol);

            controles.Controls.Add(grupos);
            controles.Controls.Add(fechas);
            controles.Controls.Add(buscar);
            controles.Controls.Add(notificaciones);
            controles.Controls.Add(perfil);

            panelCabecera.Controls.Add(controles);

            panelCabecera.Resize += (s, e) =>
            {
                controles.Location = new Point(
                    Math.Max(450, panelCabecera.Width - controles.Width - 25),
                    19);
            };

            panelContenido.Controls.Add(panelCabecera);
            panelCabecera.BringToFront();
        }

        // =========================================
        // ZONA DEL DASHBOARD
        // =========================================

        private void CrearZonaDashboard()
        {
            Panel zona = new Panel();
            zona.Dock = DockStyle.Fill;
            zona.BackColor = fondo;
            zona.AutoScroll = true;

            panelContenido.Controls.Add(zona);
            zona.BringToFront();

            // AVISO
            Guna2Panel aviso = CrearTarjeta();
            aviso.Location = new Point(32, 24);
            aviso.Size = new Size(1200, 76);
            aviso.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            aviso.FillColor = Color.FromArgb(239, 250, 248);
            aviso.BorderColor = Color.FromArgb(195, 232, 228);
            aviso.BorderThickness = 1;
            aviso.ShadowDecoration.Enabled = false;

            Label iconoAviso = new Label();
            iconoAviso.Text = "ⓘ";
            iconoAviso.Font = new Font("Segoe UI", 19);
            iconoAviso.ForeColor = Color.FromArgb(28, 139, 136);
            iconoAviso.AutoSize = true;
            iconoAviso.Location = new Point(17, 19);

            Label tituloAviso = new Label();
            tituloAviso.Text = "Una dada per acompanyar, no per etiquetar";
            tituloAviso.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            tituloAviso.ForeColor = Color.FromArgb(30, 125, 128);
            tituloAviso.AutoSize = true;
            tituloAviso.Location = new Point(55, 14);

            Label textoAviso = new Label();
            textoAviso.Text = "Aquestes dades són senyals per observar, no un diagnòstic.";
            textoAviso.Font = new Font("Segoe UI", 9);
            textoAviso.ForeColor = gris;
            textoAviso.AutoSize = true;
            textoAviso.Location = new Point(55, 39);

            aviso.Controls.Add(iconoAviso);
            aviso.Controls.Add(tituloAviso);
            aviso.Controls.Add(textoAviso);

            // INDICADORES
            TableLayoutPanel indicadores = new TableLayoutPanel();
            indicadores.Location = new Point(32, 118);
            indicadores.Size = new Size(1200, 162);
            indicadores.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            indicadores.ColumnCount = 6;
            indicadores.RowCount = 1;
            indicadores.BackColor = fondo;
            indicadores.Padding = new Padding(0);
            indicadores.Margin = new Padding(0);

            for (int i = 0; i < 6; i++)
                indicadores.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 100f / 6));

            indicadores.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            string[] titulos =
            {
                "Infants actius",
                "Sessions",
                "Puntuació mitjana",
                "Taxa d'encert",
                "Temps de resposta",
                "Taxa d'abandonament"
            };

            string[] valores =
            {
                "24", "186", "8,4", "81%", "2,3 s", "6,8%"
            };

            string[] variaciones =
            {
                "▲ 4,2%", "▲ 12,6%", "▲ 3,1%",
                "▲ 2,8%", "▼ 0,4 s", "▼ 1,2%"
            };

            string[] iconos =
            {
                "☺", "▷", "◎", "✓", "◷", "☰"
            };

            Color[] fondosIconos =
            {
                Color.FromArgb(235, 248, 246),
                Color.FromArgb(255, 244, 222),
                Color.FromArgb(233, 239, 249),
                Color.FromArgb(230, 245, 238),
                Color.FromArgb(242, 235, 251),
                Color.FromArgb(249, 239, 233)
            };

            Color[] coloresIconos =
            {
                Color.FromArgb(32, 160, 153),
                Color.FromArgb(210, 140, 20),
                Color.FromArgb(80, 112, 166),
                Color.FromArgb(52, 145, 105),
                Color.FromArgb(135, 99, 181),
                Color.FromArgb(206, 104, 55)
            };

            for (int i = 0; i < 6; i++)
            {
                Guna2Panel tarjeta = CrearTarjeta();
                tarjeta.Dock = DockStyle.Fill;
                tarjeta.Margin = new Padding(0, 0, 12, 0);

                Guna2Panel icono = new Guna2Panel();
                icono.Size = new Size(42, 42);
                icono.Location = new Point(17, 17);
                icono.BorderRadius = 12;
                icono.FillColor = fondosIconos[i];

                Label simbolo = new Label();
                simbolo.Text = iconos[i];
                simbolo.Dock = DockStyle.Fill;
                simbolo.BackColor = Color.Transparent;
                simbolo.ForeColor = coloresIconos[i];
                simbolo.Font = new Font("Segoe UI Symbol", 18);
                simbolo.TextAlign = ContentAlignment.MiddleCenter;

                icono.Controls.Add(simbolo);

                Label valor = new Label();
                valor.Text = valores[i];
                valor.Font = new Font("Segoe UI", 22, FontStyle.Bold);
                valor.ForeColor = texto;
                valor.AutoSize = true;
                valor.Location = new Point(17, 75);

                Label titulo = new Label();
                titulo.Text = titulos[i];
                titulo.Font = new Font("Segoe UI", 9);
                titulo.ForeColor = gris;
                titulo.AutoSize = true;
                titulo.Location = new Point(17, 124);

                Label variacion = new Label();
                variacion.Text = variaciones[i];
                variacion.Font = new Font("Segoe UI", 7, FontStyle.Bold);
                variacion.ForeColor = Color.FromArgb(35, 145, 133);
                variacion.BackColor = Color.FromArgb(235, 248, 245);
                variacion.AutoSize = true;
                variacion.Padding = new Padding(5, 4, 5, 4);
                variacion.Location = new Point(145, 80);

                tarjeta.Controls.Add(icono);
                tarjeta.Controls.Add(valor);
                tarjeta.Controls.Add(titulo);
                tarjeta.Controls.Add(variacion);

                indicadores.Controls.Add(tarjeta, i, 0);
            }

            // PANELES INFERIORES
            TableLayoutPanel inferiores = new TableLayoutPanel();
            inferiores.Location = new Point(32, 300);
            inferiores.Size = new Size(1200, 390);
            inferiores.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inferiores.ColumnCount = 3;
            inferiores.RowCount = 1;
            inferiores.BackColor = fondo;

            inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
            inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            inferiores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            inferiores.Controls.Add(CrearPanelEvolucion(), 0, 0);
            inferiores.Controls.Add(CrearPanelNiveles(), 1, 0);
            inferiores.Controls.Add(CrearPanelAtencion(), 2, 0);

            zona.Controls.Add(aviso);
            zona.Controls.Add(indicadores);
            zona.Controls.Add(inferiores);

            zona.Resize += (s, e) =>
            {
                int ancho = Math.Max(700, zona.ClientSize.Width - 64);
                aviso.Width = ancho;
                indicadores.Width = ancho;
                inferiores.Width = ancho;
            };
        }

        // =========================================
        // TARJETA BASE
        // =========================================

        private Guna2Panel CrearTarjeta()
        {
            Guna2Panel tarjeta = new Guna2Panel();

            tarjeta.FillColor = Color.White;
            tarjeta.BackColor = fondo;
            tarjeta.BorderRadius = 16;
            tarjeta.BorderThickness = 0;
            tarjeta.ShadowDecoration.Enabled = true;
            tarjeta.ShadowDecoration.Depth = 3;
            tarjeta.ShadowDecoration.Color = Color.FromArgb(225, 225, 225);

            return tarjeta;
        }

        // =========================================
        // EVOLUCIÓN SEMANAL
        // =========================================

        private Guna2Panel CrearPanelEvolucion()
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0, 0, 14, 0);

            Label titulo = CrearTituloPanel(
                "Taxa d'encert per setmana", 20, 20);

            Label subtitulo = CrearSubtituloPanel(
                "Mitjana dels dos grups", 20, 50);

            Label enlace = new Label();
            enlace.Text = "Veure evolució  ›";
            enlace.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            enlace.ForeColor = Color.FromArgb(28, 145, 143);
            enlace.AutoSize = true;
            enlace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            enlace.Location = new Point(450, 24);

            ChartCanvas grafico = new ChartCanvas(EstilGrafico.Linea);
            grafico.Dock = DockStyle.Fill;
            grafico.Margin = new Padding(20, 85, 20, 25);

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(enlace);
            tarjeta.Controls.Add(grafico);

            return tarjeta;
        }

        // =========================================
        // NIVELES
        // =========================================

        private Guna2Panel CrearPanelNiveles()
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0, 0, 14, 0);

            Label titulo = CrearTituloPanel(
                "Encerts i errors per nivell", 18, 20);

            Label subtitulo = CrearSubtituloPanel(
                "Sessions puntuables", 18, 50);

            ChartCanvas grafico = new ChartCanvas(EstilGrafico.Barras);
            grafico.Dock = DockStyle.Fill;
            grafico.Margin = new Padding(15, 85, 15, 25);

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(grafico);

            return tarjeta;
        }

        // =========================================
        // PARTICIPANTES QUE MERECEN ATENCIÓN
        // =========================================

        private Guna2Panel CrearPanelAtencion()
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0);

            Label titulo = CrearTituloPanel(
                "Infants que mereixen atenció", 18, 20);

            Label subtitulo = CrearSubtituloPanel(
                "Senyals recents", 18, 50);

            Label enlace = new Label();
            enlace.Text = "Veure tots  ›";
            enlace.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            enlace.ForeColor = Color.FromArgb(28, 145, 143);
            enlace.AutoSize = true;
            enlace.Location = new Point(235, 24);

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(enlace);

            string[] codigos = { "LM-1093", "AR-7745", "NT-3308" };

            string[] motivos =
            {
                "Més temps de resposta",
                "2 sessions interrompudes",
                "Baixada d'encerts recent"
            };

            string[] iniciales = { "LM", "AR", "NT" };

            Color[] colores =
            {
                Color.FromArgb(220, 240, 239),
                Color.FromArgb(244, 235, 215),
                Color.FromArgb(237, 229, 245)
            };

            for (int i = 0; i < 3; i++)
            {
                int y = 85 + i * 80;

                Panel separador = new Panel();
                separador.BackColor = Color.FromArgb(235, 237, 240);
                separador.Size = new Size(320, 1);
                separador.Location = new Point(18, y);

                Guna2Panel avatar = new Guna2Panel();
                avatar.Size = new Size(48, 48);
                avatar.Location = new Point(18, y + 14);
                avatar.BorderRadius = 24;
                avatar.FillColor = colores[i];

                Label letras = new Label();
                letras.Text = iniciales[i];
                letras.Dock = DockStyle.Fill;
                letras.BackColor = Color.Transparent;
                letras.ForeColor = texto;
                letras.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                letras.TextAlign = ContentAlignment.MiddleCenter;

                avatar.Controls.Add(letras);

                Label codigo = new Label();
                codigo.Text = codigos[i];
                codigo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                codigo.ForeColor = texto;
                codigo.AutoSize = true;
                codigo.Location = new Point(77, y + 15);

                Label motivo = new Label();
                motivo.Text = motivos[i];
                motivo.Font = new Font("Segoe UI", 7);
                motivo.ForeColor = gris;
                motivo.AutoSize = true;
                motivo.Location = new Point(77, y + 40);

                Label estado = new Label();
                estado.Text = i == 0 ? "Observar" : "Revisar";
                estado.Font = new Font("Segoe UI", 7, FontStyle.Bold);
                estado.ForeColor = Color.FromArgb(150, 103, 31);
                estado.BackColor = Color.FromArgb(255, 244, 220);
                estado.AutoSize = true;
                estado.Padding = new Padding(7, 5, 7, 5);
                estado.Location = new Point(255, y + 23);

                tarjeta.Controls.Add(separador);
                tarjeta.Controls.Add(avatar);
                tarjeta.Controls.Add(codigo);
                tarjeta.Controls.Add(motivo);
                tarjeta.Controls.Add(estado);
            }

            return tarjeta;
        }

        // =========================================
        // ETIQUETAS AUXILIARES
        // =========================================

        private Label CrearTituloPanel(string contenido, int x, int y)
        {
            Label label = new Label();
            label.Text = contenido;
            label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            label.ForeColor = texto;
            label.AutoSize = true;
            label.Location = new Point(x, y);

            return label;
        }

        private Label CrearSubtituloPanel(string contenido, int x, int y)
        {
            Label label = new Label();
            label.Text = contenido;
            label.Font = new Font("Segoe UI", 8);
            label.ForeColor = gris;
            label.AutoSize = true;
            label.Location = new Point(x, y);

            return label;
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
        }
    }

    // =============================================
    // TIPO DE GRÁFICO
    // =============================================

    public enum EstilGrafico
    {
        Linea,
        Barras
    }

    // =============================================
    // CONTROL PERSONALIZADO DE GRÁFICOS
    // =============================================

    public class ChartCanvas : Panel
    {
        private readonly EstilGrafico _estil;

        private readonly Color turquesa =
            Color.FromArgb(54, 184, 177);

        private readonly Color naranja =
            Color.FromArgb(235, 194, 168);

        public ChartCanvas(EstilGrafico estil)
        {
            _estil = estil;
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_estil == EstilGrafico.Linea)
                DibujarLinea(g);
            else
                DibujarBarras(g);
        }

        private void DibujarLinea(Graphics g)
        {
            int ancho = ClientSize.Width;
            int alto = ClientSize.Height;

            if (ancho < 100 || alto < 100)
                return;

            int izquierda = 35;
            int derecha = ancho - 10;
            int arriba = 25;
            int abajo = alto - 45;

            using Pen rejilla = new Pen(
                Color.FromArgb(225, 232, 239), 1);

            rejilla.DashStyle = DashStyle.Dash;

            string[] porcentajes = { "100%", "75%", "50%", "25%" };

            for (int i = 0; i < 4; i++)
            {
                int y = arriba + i * (abajo - arriba) / 3;

                g.DrawLine(rejilla, izquierda, y, derecha, y);

                using Font fuente = new Font("Segoe UI", 7);
                using Brush pincel = new SolidBrush(
                    Color.FromArgb(145, 155, 175));

                g.DrawString(porcentajes[i], fuente, pincel, 0, y - 7);
            }

            float[] valores = { 40, 52, 51, 70, 68, 79, 76, 85, 84, 93 };
            PointF[] puntos = new PointF[valores.Length];

            for (int i = 0; i < valores.Length; i++)
            {
                float x = izquierda +
                    i * (derecha - izquierda) /
                    (float)(valores.Length - 1);

                float y = abajo -
                    valores[i] / 100f * (abajo - arriba);

                puntos[i] = new PointF(x, y);
            }

            using GraphicsPath camino = new GraphicsPath();
            camino.AddCurve(puntos, 0.35f);

            using GraphicsPath area = (GraphicsPath)camino.Clone();

            area.AddLine(
                puntos[puntos.Length - 1].X,
                puntos[puntos.Length - 1].Y,
                derecha, abajo);

            area.AddLine(derecha, abajo, izquierda, abajo);
            area.CloseFigure();

            using LinearGradientBrush degradado =
                new LinearGradientBrush(
                    new Rectangle(
                        izquierda, arriba,
                        derecha - izquierda,
                        abajo - arriba),
                    Color.FromArgb(75, 54, 184, 177),
                    Color.FromArgb(0, 54, 184, 177),
                    LinearGradientMode.Vertical);

            g.FillPath(degradado, area);

            using Pen linea = new Pen(turquesa, 3);
            linea.LineJoin = LineJoin.Round;
            g.DrawPath(linea, camino);

            foreach (PointF punto in puntos)
            {
                g.FillEllipse(
                    Brushes.White,
                    punto.X - 4, punto.Y - 4, 8, 8);

                using Pen borde = new Pen(turquesa, 2.5f);

                g.DrawEllipse(
                    borde,
                    punto.X - 4, punto.Y - 4, 8, 8);
            }

            string[] semanas =
            {
                "Set. 1", "Set. 2", "Set. 3",
                "Set. 4", "Set. 5"
            };

            using Font fuenteSemana = new Font("Segoe UI", 7);
            using Brush pincelSemana =
                new SolidBrush(Color.FromArgb(135, 145, 165));

            for (int i = 0; i < semanas.Length; i++)
            {
                float x = izquierda +
                    i * (derecha - izquierda) / 4f;

                g.DrawString(
                    semanas[i],
                    fuenteSemana,
                    pincelSemana,
                    x - 15, abajo + 15);
            }
        }

        private void DibujarBarras(Graphics g)
        {
            int ancho = ClientSize.Width;
            int alto = ClientSize.Height;

            if (ancho < 100 || alto < 100)
                return;

            int izquierda = 10;
            int derecha = ancho - 8;
            int arriba = 25;
            int abajo = alto - 30;

            using Pen rejilla = new Pen(
                Color.FromArgb(225, 232, 239), 1);

            rejilla.DashStyle = DashStyle.Dash;

            for (int i = 0; i < 4; i++)
            {
                int y = arriba + i * (abajo - arriba) / 3;
                g.DrawLine(rejilla, izquierda, y, derecha, y);
            }

            string[] niveles =
            {
                "La Plaça",
                "El Parc",
                "Plaça Gran",
                "El Jardí"
            };

            float[] aciertos = { 76, 68, 86, 81 };
            float[] errores = { 18, 25, 0, 0 };

            int espacio = (derecha - izquierda) / 4;

            using Font fuente = new Font("Segoe UI", 7);
            using Brush textoBrush =
                new SolidBrush(Color.FromArgb(135, 145, 165));

            for (int i = 0; i < 4; i++)
            {
                int centro = izquierda + espacio * i + espacio / 2;

                int alturaAciertos =
                    (int)((abajo - arriba) * aciertos[i] / 100);

                int alturaErrores =
                    (int)((abajo - arriba) * errores[i] / 100);

                Rectangle barraAciertos = new Rectangle(
                    centro - 15,
                    abajo - alturaAciertos,
                    17,
                    alturaAciertos);

                using SolidBrush pincelAciertos =
                    new SolidBrush(turquesa);

                g.FillRoundedRectangle(
                    pincelAciertos, barraAciertos, 6);

                if (errores[i] > 0)
                {
                    Rectangle barraErrores = new Rectangle(
                        centro + 7,
                        abajo - alturaErrores,
                        17,
                        alturaErrores);

                    using SolidBrush pincelErrores =
                        new SolidBrush(naranja);

                    g.FillRoundedRectangle(
                        pincelErrores, barraErrores, 6);
                }

                SizeF medida = g.MeasureString(niveles[i], fuente);

                g.DrawString(
                    niveles[i],
                    fuente,
                    textoBrush,
                    centro - medida.Width / 2,
                    abajo + 10);
            }
        }
    }

    // =============================================
    // EXTENSIÓN PARA BARRAS REDONDEADAS
    // =============================================

    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(
            this Graphics graphics,
            Brush brush,
            Rectangle bounds,
            int radius)
        {
            using GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(
                bounds.X, bounds.Y,
                diameter, diameter, 180, 90);

            path.AddArc(
                bounds.Right - diameter, bounds.Y,
                diameter, diameter, 270, 90);

            path.AddArc(
                bounds.Right - diameter,
                bounds.Bottom - diameter,
                diameter, diameter, 0, 90);

            path.AddArc(
                bounds.X,
                bounds.Bottom - diameter,
                diameter, diameter, 90, 90);

            path.CloseFigure();

            graphics.FillPath(brush, path);
        }
    }
}
