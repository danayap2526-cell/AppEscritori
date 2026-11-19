using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace AppEmocions.Forms
{
    // =============================================
    // MODELO (más adelante vendrá de la API)
    // =============================================

    public record InfantAtencion(string Codigo, string Motivo, string Estado);

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
        private readonly Color bordeSuave = Color.FromArgb(225, 229, 235);

        // PANELES
        private Guna2Panel panelMenu = null!;
        private Guna2Panel panelContenido = null!;
        private Guna2Panel panelCabecera = null!;
        private Guna2Panel panelNavegacion = null!;

        private readonly List<Guna2Button> botonesMenu = new();

        // FILTROS DE LA CABECERA
        private Guna2ComboBox cmbGrupos = null!;
        private Guna2Button btnFechas = null!;
        private ToolStripDropDown ddCalendario = null!;
        private MonthCalendar calendario = null!;
        private DateTime fechaInicio = new DateTime(2026, 11, 1);
        private DateTime fechaFin = new DateTime(2026, 11, 30);

        private static readonly string[] MesesCortos =
        {
            "gen.", "febr.", "març", "abr.", "maig", "juny",
            "jul.", "ag.", "set.", "oct.", "nov.", "des."
        };

        // Evento para que otros formularios se enteren de qué infant se pulsó
        public event Action<string>? InfantSeleccionado;

        public FrmDashboard()
        {
            InitializeComponent();

            ConfigurarFormulario();
            CrearContenido();
            CrearMenuLateral();

            // El panel Fill debe ser el último en acoplarse
            panelContenido.BringToFront();
        }

        // =========================================
        // DATOS DE EJEMPLO
        // TODO: sustituir por llamadas a ApiClient cuando
        //       estén listos los endpoints del dashboard
        // =========================================

        private List<InfantAtencion> ObtenerInfantsAtencion()
        {
            return new List<InfantAtencion>
            {
                new("LM-1093", "Més temps de resposta", "Observar"),
                new("AR-7745", "2 sessions interrompudes", "Revisar"),
                new("NT-3308", "Baixada d'encerts recent", "Revisar")
            };
        }

        // Se llama cuando cambia el grupo o el rango de fechas
        private void FiltroCambiado()
        {
            // TODO: recargar KPIs, gráficas y lista con la API usando:
            //   cmbGrupos.SelectedItem, fechaInicio, fechaFin
        }

        // Se llama al pulsar una fila de la lista de infants
        private void AbrirInfant(string codigo)
        {
            InfantSeleccionado?.Invoke(codigo);

            // TODO: abrir el formulario de detalle, por ejemplo:
            // new FrmInfant(codigo).ShowDialog(this);
            MessageBox.Show($"Obrir fitxa de {codigo}", "Infant",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // =========================================
        // FORMULARIO
        // =========================================

        private void ConfigurarFormulario()
        {
            Text = "Poble de les Emocions";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1366, 750);
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

            CrearCabeceraMenu();
            CrearNavegacion();
            CrearPieMenu();

            panelNavegacion.BringToFront();
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

            string[] nombres =
            {
                "Tauler", "Infants", "Comparativa", "Abandonament",
                "Jocs", "Usuaris", "Dades", "Configuració"
            };

            string[] iconos =
            {
                "▦", "♙", "▥", "☰",
                "♧", "♢", "▤", "⚙"
            };

            for (int i = 0; i < nombres.Length; i++)
            {
                Guna2Button boton = CrearBotonMenu(nombres[i], iconos[i], i == 0);

                boton.Location = new Point(13, 14 + i * 59);
                panelNavegacion.Controls.Add(boton);
                botonesMenu.Add(boton);
            }
        }

        private Guna2Button CrearBotonMenu(string nombre, string icono, bool seleccionado)
        {
            Guna2Button boton = new Guna2Button();

            boton.Size = new Size(274, 53);
            boton.BorderRadius = 12;
            boton.BorderThickness = 0;
            boton.BorderColor = azulMenu;

            boton.FillColor = seleccionado ? azulSeleccionado : azulMenu;

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
            saludo.Text = "Bon dia, Marta";
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
            controles.Size = new Size(770, 55);
            controles.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controles.Location = new Point(450, 19);

            // ---- DROPDOWN DE GRUPOS ----
            cmbGrupos = new Guna2ComboBox();
            cmbGrupos.Size = new Size(150, 45);
            cmbGrupos.BorderRadius = 10;
            cmbGrupos.BorderThickness = 1;
            cmbGrupos.BorderColor = bordeSuave;
            cmbGrupos.FillColor = Color.White;
            cmbGrupos.BackColor = Color.White;
            cmbGrupos.ForeColor = texto;
            cmbGrupos.Font = new Font("Segoe UI", 9);
            cmbGrupos.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGrupos.HoverState.BorderColor = turquesa;
            cmbGrupos.FocusedState.BorderColor = turquesa;
            cmbGrupos.Items.AddRange(new object[]
            {
                "P4-A i P4-B",
                "P4-A",
                "P4-B"
            });
            cmbGrupos.SelectedIndex = 0;
            cmbGrupos.Margin = new Padding(4, 0, 5, 0);
            cmbGrupos.SelectedIndexChanged += (s, e) => FiltroCambiado();

            // ---- DROPDOWN CON CALENDARIO ----
            btnFechas = new Guna2Button();
            btnFechas.Size = new Size(185, 45);
            btnFechas.BorderRadius = 10;
            btnFechas.FillColor = Color.White;
            btnFechas.BackColor = Color.White;
            btnFechas.ForeColor = texto;
            btnFechas.BorderColor = bordeSuave;
            btnFechas.BorderThickness = 1;
            btnFechas.Font = new Font("Segoe UI", 9);
            btnFechas.Cursor = Cursors.Hand;
            btnFechas.HoverState.BorderColor = turquesa;
            btnFechas.Margin = new Padding(4, 0, 5, 0);
            ActualizarTextoFechas();

            CrearCalendarioDesplegable();
            btnFechas.Click += (s, e) =>
                ddCalendario.Show(btnFechas, new Point(0, btnFechas.Height + 4));

            Guna2TextBox buscar = new Guna2TextBox();
            buscar.Size = new Size(170, 45);
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
            notificaciones.BorderColor = bordeSuave;
            notificaciones.BorderThickness = 1;
            notificaciones.Font = new Font("Segoe UI Symbol", 17);
            notificaciones.Text = "♧";
            notificaciones.Cursor = Cursors.Hand;
            notificaciones.Margin = new Padding(4, 0, 5, 0);

            Guna2Panel perfil = new Guna2Panel();
            perfil.Size = new Size(200, 48);
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

            controles.Controls.Add(cmbGrupos);
            controles.Controls.Add(btnFechas);
            controles.Controls.Add(buscar);
            controles.Controls.Add(notificaciones);
            controles.Controls.Add(perfil);

            panelCabecera.Controls.Add(controles);

            panelCabecera.Resize += (s, e) =>
            {
                controles.Location = new Point(
                    Math.Max(270, panelCabecera.Width - controles.Width - 25),
                    19);
            };

            panelContenido.Controls.Add(panelCabecera);
            panelCabecera.BringToFront();
        }

        // ---- Calendario que se despliega bajo el botón de fechas ----
        private void CrearCalendarioDesplegable()
        {
            calendario = new MonthCalendar();
            calendario.MaxSelectionCount = 62;               // permite elegir un rango arrastrando
            calendario.FirstDayOfWeek = Day.Monday;
            calendario.ShowToday = false;
            calendario.ShowTodayCircle = false;
            calendario.TitleBackColor = azulMenu;
            calendario.TitleForeColor = Color.White;
            calendario.SetSelectionRange(fechaInicio, fechaFin);

            calendario.DateSelected += (s, e) =>
            {
                fechaInicio = e.Start.Date;
                fechaFin = e.End.Date;
                ActualizarTextoFechas();
                ddCalendario.Close();
                FiltroCambiado();
            };

            ToolStripControlHost host = new ToolStripControlHost(calendario)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = true
            };

            ddCalendario = new ToolStripDropDown
            {
                Padding = new Padding(4),
                AutoClose = true
            };
            ddCalendario.Items.Add(host);
        }

        private void ActualizarTextoFechas()
        {
            btnFechas.Text = "▣  " + FormatearRango(fechaInicio, fechaFin) + "  ▾";
        }

        private static string FormatearRango(DateTime ini, DateTime fin)
        {
            string mesIni = MesesCortos[ini.Month - 1];
            string mesFin = MesesCortos[fin.Month - 1];

            if (ini.Date == fin.Date)
                return $"{ini.Day} {mesIni} {ini.Year}";

            if (ini.Month == fin.Month && ini.Year == fin.Year)
                return $"{ini.Day}–{fin.Day} {mesIni} {ini.Year}";

            return $"{ini.Day} {mesIni} – {fin.Day} {mesFin} {fin.Year}";
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

            // ---------- AVISO ----------
            Color colorAviso = Color.FromArgb(239, 250, 248);

            Guna2Panel aviso = CrearTarjeta();
            aviso.Location = new Point(32, 24);
            aviso.Size = new Size(1200, 76);
            aviso.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            aviso.FillColor = colorAviso;
            aviso.BorderColor = Color.FromArgb(195, 232, 228);
            aviso.BorderThickness = 1;
            aviso.ShadowDecoration.Enabled = false;
            aviso.Padding = new Padding(4);

            // Zona central (Fill). Se añade primero para que se acople la última.
            Guna2Panel zonaTextos = new Guna2Panel();
            zonaTextos.Dock = DockStyle.Fill;
            zonaTextos.FillColor = colorAviso;
            zonaTextos.BackColor = colorAviso;

            Label tituloAviso = new Label();
            tituloAviso.Text = "Una dada per acompanyar, no per etiquetar";
            tituloAviso.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            tituloAviso.ForeColor = Color.FromArgb(30, 125, 128);
            tituloAviso.BackColor = Color.Transparent;
            tituloAviso.AutoSize = true;
            tituloAviso.Location = new Point(0, 14);

            Label textoAviso = new Label();
            textoAviso.Text = "Aquestes dades són senyals per observar, no un diagnòstic.";
            textoAviso.Font = new Font("Segoe UI", 9);
            textoAviso.ForeColor = gris;
            textoAviso.BackColor = Color.Transparent;
            textoAviso.AutoSize = true;
            textoAviso.Location = new Point(0, 39);

            zonaTextos.Controls.Add(tituloAviso);
            zonaTextos.Controls.Add(textoAviso);

            // Icono (izquierda)
            Guna2Panel zonaIcono = new Guna2Panel();
            zonaIcono.Dock = DockStyle.Left;
            zonaIcono.Width = 52;
            zonaIcono.FillColor = colorAviso;
            zonaIcono.BackColor = colorAviso;

            Label iconoAviso = new Label();
            iconoAviso.Text = "ⓘ";
            iconoAviso.Font = new Font("Segoe UI Symbol", 19);
            iconoAviso.ForeColor = Color.FromArgb(28, 139, 136);
            iconoAviso.BackColor = Color.Transparent;
            iconoAviso.Dock = DockStyle.Fill;
            iconoAviso.TextAlign = ContentAlignment.MiddleCenter;
            zonaIcono.Controls.Add(iconoAviso);

            // Botón de cerrar (derecha)
            Guna2Button btnCerrarAviso = new Guna2Button();
            btnCerrarAviso.Dock = DockStyle.Right;
            btnCerrarAviso.Width = 52;
            btnCerrarAviso.FillColor = colorAviso;
            btnCerrarAviso.BackColor = colorAviso;
            btnCerrarAviso.ForeColor = gris;
            btnCerrarAviso.Font = new Font("Segoe UI Symbol", 12);
            btnCerrarAviso.Text = "✕";
            btnCerrarAviso.BorderRadius = 12;
            btnCerrarAviso.Cursor = Cursors.Hand;
            btnCerrarAviso.HoverState.FillColor = Color.FromArgb(220, 240, 237);
            btnCerrarAviso.ShadowDecoration.Enabled = false;

            // Orden: primero el Fill, después los laterales
            aviso.Controls.Add(zonaTextos);
            aviso.Controls.Add(zonaIcono);
            aviso.Controls.Add(btnCerrarAviso);
            zonaTextos.BringToFront();

            // ---------- INDICADORES ----------
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
                indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));

            indicadores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            string[] titulos =
            {
                "Infants actius", "Sessions", "Puntuació mitjana",
                "Taxa d'encert", "Temps de resposta", "Taxa d'abandonament"
            };

            string[] valores = { "24", "186", "8,4", "81%", "2,3 s", "6,8%" };

            string[] variaciones =
            {
                "▲ 4,2%", "▲ 12,6%", "▲ 3,1%",
                "▲ 2,8%", "▼ 0,4 s", "▼ 1,2%"
            };

            string[] iconos = { "☺", "▷", "◎", "✓", "◷", "☰" };

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
                valor.BackColor = Color.Transparent;
                valor.AutoSize = true;
                valor.Location = new Point(17, 75);

                Label titulo = new Label();
                titulo.Text = titulos[i];
                titulo.Font = new Font("Segoe UI", 9);
                titulo.ForeColor = gris;
                titulo.BackColor = Color.Transparent;
                titulo.AutoSize = true;
                titulo.Location = new Point(17, 124);

                Label variacion = new Label();
                variacion.Text = variaciones[i];
                variacion.Font = new Font("Segoe UI", 7, FontStyle.Bold);
                variacion.ForeColor = Color.FromArgb(35, 145, 133);
                variacion.BackColor = Color.FromArgb(235, 248, 245);
                variacion.AutoSize = true;
                variacion.Padding = new Padding(5, 4, 5, 4);

                tarjeta.Controls.Add(icono);
                tarjeta.Controls.Add(valor);
                tarjeta.Controls.Add(titulo);
                tarjeta.Controls.Add(variacion);
                AnclarDerecha(tarjeta, variacion, 80, 17);

                indicadores.Controls.Add(tarjeta, i, 0);
            }

            // ---------- PANELES INFERIORES ----------
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

            // Cerrar el aviso: se oculta y el resto sube
            btnCerrarAviso.Click += (s, e) =>
            {
                int desplazamiento = aviso.Height + 18;
                aviso.Visible = false;
                indicadores.Top -= desplazamiento;
                inferiores.Top -= desplazamiento;
                AjustarZona(zona, aviso, indicadores, inferiores);
            };

            zona.Resize += (s, e) => AjustarZona(zona, aviso, indicadores, inferiores);
        }

        private void AjustarZona(Panel zona, Control aviso, Control indicadores, Control inferiores)
        {
            int ancho = Math.Max(700, zona.ClientSize.Width - 64);
            aviso.Width = ancho;
            indicadores.Width = ancho;
            inferiores.Width = ancho;
            inferiores.Height = Math.Max(390, zona.ClientSize.Height - inferiores.Top - 24);
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

        // Mantiene un control pegado al borde derecho de su contenedor
        private void AnclarDerecha(Control contenedor, Control c, int top, int margen = 20)
        {
            void Colocar() => c.Location = new Point(contenedor.Width - c.Width - margen, top);
            contenedor.Resize += (s, e) => Colocar();
            Colocar();
        }

        // =========================================
        // EVOLUCIÓN SEMANAL
        // =========================================

        private Guna2Panel CrearPanelEvolucion()
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0, 0, 14, 0);
            tarjeta.Padding = new Padding(20, 85, 20, 25);

            Label titulo = CrearTituloPanel("Taxa d'encert per setmana", 20, 20);
            Label subtitulo = CrearSubtituloPanel("Mitjana dels dos grups", 20, 50);

            Label enlace = new Label();
            enlace.Text = "Veure evolució  ›";
            enlace.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            enlace.ForeColor = Color.FromArgb(28, 145, 143);
            enlace.BackColor = Color.Transparent;
            enlace.Cursor = Cursors.Hand;
            enlace.AutoSize = true;

            ChartCanvas grafico = new ChartCanvas(EstilGrafico.Linea);
            grafico.Dock = DockStyle.Fill;

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(enlace);
            tarjeta.Controls.Add(grafico);
            AnclarDerecha(tarjeta, enlace, 24);

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
            tarjeta.Padding = new Padding(15, 85, 15, 25);

            Label titulo = CrearTituloPanel("Encerts i errors per nivell", 18, 20);
            Label subtitulo = CrearSubtituloPanel("Sessions puntuables", 18, 50);

            ChartCanvas grafico = new ChartCanvas(EstilGrafico.Barras);
            grafico.Dock = DockStyle.Fill;

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(grafico);

            return tarjeta;
        }

        // =========================================
        // INFANTS QUE MEREIXEN ATENCIÓ (filas clicables)
        // =========================================

        private Guna2Panel CrearPanelAtencion()
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0);

            Label titulo = CrearTituloPanel("Infants que mereixen atenció", 18, 20);
            Label subtitulo = CrearSubtituloPanel("Senyals recents", 18, 50);

            Label enlace = new Label();
            enlace.Text = "Veure tots  ›";
            enlace.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            enlace.ForeColor = Color.FromArgb(28, 145, 143);
            enlace.BackColor = Color.Transparent;
            enlace.Cursor = Cursors.Hand;
            enlace.AutoSize = true;

            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(subtitulo);
            tarjeta.Controls.Add(enlace);
            AnclarDerecha(tarjeta, enlace, 24);

            Color[] colores =
            {
                Color.FromArgb(220, 240, 239),
                Color.FromArgb(244, 235, 215),
                Color.FromArgb(237, 229, 245)
            };

            List<InfantAtencion> infants = ObtenerInfantsAtencion();

            for (int i = 0; i < infants.Count; i++)
            {
                InfantAtencion infant = infants[i];
                int y = 85 + i * 78;

                Guna2Panel fila = CrearFilaInfant(infant, colores[i % colores.Length]);
                fila.Location = new Point(14, y);
                fila.Width = Math.Max(100, tarjeta.Width - 28);

                tarjeta.Controls.Add(fila);
                tarjeta.Resize += (s, e) => fila.Width = Math.Max(100, tarjeta.Width - 28);
            }

            return tarjeta;
        }

        // Cada fila se comporta como un botón: cursor de mano, resalte al pasar
        // el ratón y un único método (AbrirInfant) al hacer clic en cualquier parte.
        private Guna2Panel CrearFilaInfant(InfantAtencion infant, Color colorAvatar)
        {
            Color normal = Color.White;
            Color resaltado = Color.FromArgb(244, 248, 249);

            Guna2Panel fila = new Guna2Panel();
            fila.Size = new Size(320, 70);
            fila.BorderRadius = 12;
            fila.FillColor = normal;
            fila.BackColor = normal;
            fila.Tag = infant.Codigo;

            Guna2Panel avatar = new Guna2Panel();
            avatar.Size = new Size(48, 48);
            avatar.Location = new Point(8, 11);
            avatar.BorderRadius = 24;
            avatar.FillColor = colorAvatar;

            Label letras = new Label();
            letras.Text = infant.Codigo.Substring(0, 2);
            letras.Dock = DockStyle.Fill;
            letras.BackColor = Color.Transparent;
            letras.ForeColor = texto;
            letras.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            letras.TextAlign = ContentAlignment.MiddleCenter;
            avatar.Controls.Add(letras);

            Label codigo = new Label();
            codigo.Text = infant.Codigo;
            codigo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            codigo.ForeColor = texto;
            codigo.BackColor = Color.Transparent;
            codigo.AutoSize = true;
            codigo.Location = new Point(68, 14);

            Label motivo = new Label();
            motivo.Text = infant.Motivo;
            motivo.Font = new Font("Segoe UI", 7);
            motivo.ForeColor = gris;
            motivo.BackColor = Color.Transparent;
            motivo.AutoSize = true;
            motivo.Location = new Point(68, 39);

            Label estado = new Label();
            estado.Text = infant.Estado;
            estado.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            estado.ForeColor = Color.FromArgb(150, 103, 31);
            estado.BackColor = Color.FromArgb(255, 244, 220);
            estado.AutoSize = true;
            estado.Padding = new Padding(7, 5, 7, 5);

            Label flecha = new Label();
            flecha.Text = "›";
            flecha.Font = new Font("Segoe UI", 16);
            flecha.ForeColor = texto;
            flecha.BackColor = Color.Transparent;
            flecha.AutoSize = true;

            fila.Controls.Add(avatar);
            fila.Controls.Add(codigo);
            fila.Controls.Add(motivo);
            fila.Controls.Add(estado);
            fila.Controls.Add(flecha);

            AnclarDerecha(fila, flecha, 18, 12);
            AnclarDerecha(fila, estado, 22, 36);

            // Clic y resalte en la fila y en todos sus hijos
            void Enlazar(Control c)
            {
                c.Cursor = Cursors.Hand;
                c.Click += (s, e) => AbrirInfant(infant.Codigo);
                c.MouseEnter += (s, e) => fila.FillColor = resaltado;
                c.MouseLeave += (s, e) =>
                {
                    if (!fila.ClientRectangle.Contains(fila.PointToClient(Cursor.Position)))
                        fila.FillColor = normal;
                };

                foreach (Control hijo in c.Controls)
                    Enlazar(hijo);
            }

            Enlazar(fila);

            return fila;
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
            label.BackColor = Color.Transparent;
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
            label.BackColor = Color.Transparent;
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

        private readonly Color turquesa = Color.FromArgb(54, 184, 177);
        private readonly Color naranja = Color.FromArgb(235, 194, 168);

        public ChartCanvas(EstilGrafico estil)
        {
            _estil = estil;
            DoubleBuffered = true;
            ResizeRedraw = true;
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

            using Pen rejilla = new Pen(Color.FromArgb(225, 232, 239), 1);
            rejilla.DashStyle = DashStyle.Dash;

            string[] porcentajes = { "100%", "75%", "50%", "25%" };

            for (int i = 0; i < 4; i++)
            {
                int y = arriba + i * (abajo - arriba) / 3;

                g.DrawLine(rejilla, izquierda, y, derecha, y);

                using Font fuente = new Font("Segoe UI", 7);
                using Brush pincel = new SolidBrush(Color.FromArgb(145, 155, 175));

                g.DrawString(porcentajes[i], fuente, pincel, 0, y - 7);
            }

            float[] valores = { 40, 52, 51, 70, 68, 79, 76, 85, 84, 93 };
            PointF[] puntos = new PointF[valores.Length];

            for (int i = 0; i < valores.Length; i++)
            {
                float x = izquierda + i * (derecha - izquierda) / (float)(valores.Length - 1);
                float y = abajo - valores[i] / 100f * (abajo - arriba);

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

            using LinearGradientBrush degradado = new LinearGradientBrush(
                new Rectangle(izquierda, arriba, derecha - izquierda, abajo - arriba),
                Color.FromArgb(75, 54, 184, 177),
                Color.FromArgb(0, 54, 184, 177),
                LinearGradientMode.Vertical);

            g.FillPath(degradado, area);

            using Pen linea = new Pen(turquesa, 3);
            linea.LineJoin = LineJoin.Round;
            g.DrawPath(linea, camino);

            foreach (PointF punto in puntos)
            {
                g.FillEllipse(Brushes.White, punto.X - 4, punto.Y - 4, 8, 8);

                using Pen borde = new Pen(turquesa, 2.5f);
                g.DrawEllipse(borde, punto.X - 4, punto.Y - 4, 8, 8);
            }

            string[] semanas = { "Set. 1", "Set. 2", "Set. 3", "Set. 4", "Set. 5" };

            using Font fuenteSemana = new Font("Segoe UI", 7);
            using Brush pincelSemana = new SolidBrush(Color.FromArgb(135, 145, 165));

            for (int i = 0; i < semanas.Length; i++)
            {
                float x = izquierda + i * (derecha - izquierda) / 4f;
                g.DrawString(semanas[i], fuenteSemana, pincelSemana, x - 15, abajo + 15);
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

            using Pen rejilla = new Pen(Color.FromArgb(225, 232, 239), 1);
            rejilla.DashStyle = DashStyle.Dash;

            for (int i = 0; i < 4; i++)
            {
                int y = arriba + i * (abajo - arriba) / 3;
                g.DrawLine(rejilla, izquierda, y, derecha, y);
            }

            string[] niveles = { "La Plaça", "El Parc", "Plaça Gran", "El Jardí" };

            float[] aciertos = { 76, 68, 86, 81 };
            float[] errores = { 18, 25, 0, 0 };

            int espacio = (derecha - izquierda) / 4;

            using Font fuente = new Font("Segoe UI", 7);
            using Brush textoBrush = new SolidBrush(Color.FromArgb(135, 145, 165));

            for (int i = 0; i < 4; i++)
            {
                int centro = izquierda + espacio * i + espacio / 2;

                int alturaAciertos = (int)((abajo - arriba) * aciertos[i] / 100);
                int alturaErrores = (int)((abajo - arriba) * errores[i] / 100);

                Rectangle barraAciertos = new Rectangle(
                    centro - 15, abajo - alturaAciertos, 17, alturaAciertos);

                using SolidBrush pincelAciertos = new SolidBrush(turquesa);
                g.FillRoundedRectangle(pincelAciertos, barraAciertos, 6);

                if (errores[i] > 0)
                {
                    Rectangle barraErrores = new Rectangle(
                        centro + 7, abajo - alturaErrores, 17, alturaErrores);

                    using SolidBrush pincelErrores = new SolidBrush(naranja);
                    g.FillRoundedRectangle(pincelErrores, barraErrores, 6);
                }

                SizeF medida = g.MeasureString(niveles[i], fuente);

                g.DrawString(
                    niveles[i], fuente, textoBrush,
                    centro - medida.Width / 2, abajo + 10);
            }
        }
    }

    // =============================================
    // EXTENSIÓN PARA BARRAS REDONDEADAS
    // =============================================

    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(
            this Graphics graphics, Brush brush, Rectangle bounds, int radius)
        {
            using GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();

            graphics.FillPath(brush, path);
        }
    }
}