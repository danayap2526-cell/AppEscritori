using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppEmocions.Helpers;
using AppEmocions.Models.Usuaris;
using AppEmocions.Moduls;
using Guna.UI2.WinForms;

namespace AppEmocions.Forms
{
    public partial class FrmDashboard : Form
    {
        private Guna2Panel panelMenu = null!;
        private Guna2Panel panelContenido = null!;
        private Guna2Panel panelCabecera = null!;
        private Guna2Panel panelNavegacion = null!;
        private Panel panelVista = null!;

        private readonly Dictionary<string, Guna2Button> botonesMenu = new();
        private readonly Dictionary<string, UserControl> vistasCache = new();
        private IModuloVista? vistaActiva;

        private Guna2ComboBox cmbGrupos = null!;
        private Guna2Button btnFechas = null!;
        private ToolStripDropDown ddCalendario = null!;
        private MonthCalendar calendario = null!;
        private DateTime fechaInicio;
        private DateTime fechaFin;
        private Label lblEstado = null!; private Label lblTituloCabecera = null!; private Label lblAvatarIniciales = null!; private Label lblNombrePerfil = null!; private Label lblRolPerfil = null!;
        private CancellationTokenSource? _cts;

        public event Action<string>? InfantSeleccionado;

        public FrmDashboard(DocenteDto usuario) : this()
        {
            SesionActual.Iniciar(usuario);
            AplicarSesionActual();
        }

        public FrmDashboard()
        {
            InitializeComponent();
            DateTime hoy = DateTime.Today;
            fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            ConfigurarFormulario();
            CrearContenido();
            CrearMenuLateral();
            panelContenido.BringToFront();
            NavegarA("Tauler");
        }

        protected override void OnShown(EventArgs e) { base.OnShown(e); _ = RefrescarVistaActualAsync(); }
        private string? GrupoSeleccionado()
        {
            if (!SesionActual.EstaAutenticado || SesionActual.EsAdmin)
                return cmbGrupos.SelectedIndex <= 0 ? null : cmbGrupos.SelectedItem?.ToString();

            return cmbGrupos.SelectedItem?.ToString()
                ?? (SesionActual.GruposPermitidos.Count > 0 ? SesionActual.GruposPermitidos[0] : "__SENSE_GRUP__");
        }
        private void FiltroCambiado() => _ = RefrescarVistaActualAsync();

        private async Task RefrescarVistaActualAsync()
        {
            if (vistaActiva == null) return;
            _cts?.Cancel();
            var cts = _cts = new CancellationTokenSource();
            await vistaActiva.ActualizarFiltrosAsync(fechaInicio, fechaFin, GrupoSeleccionado(), cts.Token);
        }

        private void NavegarA(string seccion)
        {
            foreach (var kv in botonesMenu) kv.Value.FillColor = kv.Key == seccion ? TemaApp.AzulSeleccionado : TemaApp.AzulMenu;
            if (!vistasCache.TryGetValue(seccion, out var control))
            {
                control = CrearModulo(seccion);
                vistasCache[seccion] = control;
            }
            panelVista.Controls.Clear();
            panelVista.Controls.Add(control);
            vistaActiva = control as IModuloVista; if (lblTituloCabecera != null) lblTituloCabecera.Text = seccion == "Tauler" ? $"Bon dia, {SesionActual.PrimerNombre}" : (seccion == "Infants" ? "Llista d'infants" : (seccion == "Comparativa" ? "Comparativa de grup" : (seccion == "Abandonament" ? "Abandonament per nivell" : seccion)));
            _ = RefrescarVistaActualAsync();
        }

        private UserControl CrearModulo(string seccion)
        {
            if (seccion == "Infants") return CrearModuloConEstado(new UcInfants()); if (seccion == "Comparativa") return CrearModuloConEstado(new UcComparativa()); if (seccion == "Abandonament") return CrearModuloConEstado(new UcAbandonament()); if (seccion == "Jocs") return CrearModuloConEstado(new UcJocs()); if (seccion == "Usuaris") return CrearModuloConEstado(new UcUsuaris()); if (seccion == "Dades") return CrearModuloConEstado(new UcDades()); if (seccion == "Tauler")
            {
                var tauler = new UcTauler();
                tauler.EstadoCambiado += MostrarEstado;
                tauler.InfantSeleccionado += AbrirInfant;
                tauler.NavegarASeccion += NavegarA;
                return tauler;
            }
            var ph = new UserControl { Dock = DockStyle.Fill, BackColor = TemaApp.Fondo };
            ph.Controls.Add(new Label
            {
                Text = $"Mòdul «{seccion}» pendent d'implementar",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = TemaApp.Gris,
                AutoSize = true,
                Location = new Point(40, 40)
            });
            return ph;
        }

        private void MostrarEstado(string msg, bool err) { lblEstado.Text = msg; lblEstado.ForeColor = err ? TemaApp.RojoError : TemaApp.Gris; lblEstado.Visible = msg.Length > 0; }
        private void AbrirAltaInfant()
        {
            foreach (var kv in botonesMenu)
                kv.Value.FillColor = kv.Key == "Infants" ? TemaApp.AzulSeleccionado : TemaApp.AzulMenu;
            lblTituloCabecera.Text = "Nou infant i assignaci\u00f3";
            var alta = new UcInfantAlta();
            alta.EstadoCambiado += MostrarEstado;
            alta.VolverSolicitado += () => NavegarA("Infants");
            panelVista.Controls.Clear();
            panelVista.Controls.Add(alta);
            vistaActiva = alta;
            _ = RefrescarVistaActualAsync();
        }
        private void AbrirInfant(string cod) { InfantSeleccionado?.Invoke(cod); foreach (var kv in botonesMenu) kv.Value.FillColor = kv.Key == "Infants" ? TemaApp.AzulSeleccionado : TemaApp.AzulMenu; lblTituloCabecera.Text = "Detall de l\x27infant"; var det = new UcInfantDetall(cod); det.EstadoCambiado += MostrarEstado; det.VolverSolicitado += () => NavegarA("Infants"); panelVista.Controls.Clear(); panelVista.Controls.Add(det); vistaActiva = det; _ = RefrescarVistaActualAsync(); }

        private void ConfigurarFormulario()
        {
            Text = "Poble de les Emocions";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1366, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = TemaApp.Fondo;
            Font = new Font("Segoe UI", 9);
        }

        private void CrearContenido()
        {
            panelContenido = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = TemaApp.Fondo,
                BorderRadius = 0
            };
            Controls.Add(panelContenido);
            CrearCabeceraSuperior();
            panelVista = new Panel { Dock = DockStyle.Fill, BackColor = TemaApp.Fondo };
            panelContenido.Controls.Add(panelVista);
            panelVista.BringToFront();
        }

        private void CrearMenuLateral()
        {
            panelMenu = new Guna2Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                FillColor = TemaApp.AzulMenu,
                BackColor = TemaApp.AzulMenu,
                BorderRadius = 0
            };
            Controls.Add(panelMenu);
            Guna2Panel cab = new()
            {
                Dock = DockStyle.Top,
                Height = 88,
                FillColor = TemaApp.AzulMenu,
                BackColor = TemaApp.AzulMenu
            };
            Guna2Panel logo = new()
            {
                Size = new Size(46, 46),
                Location = new Point(22, 15),
                FillColor = TemaApp.Amarillo,
                BackColor = TemaApp.AzulMenu,
                BorderRadius = 13
            };
            logo.Controls.Add(new Label
            {
                Text = "P",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = TemaApp.AzulMenu,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            cab.Controls.Add(logo);
            cab.Controls.Add(new Label
            {
                Text = "Poble de les Emocions",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(85, 18)
            });
            cab.Controls.Add(new Label
            {
                Text = "Panell d'educadors",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(190, 205, 225),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(85, 43)
            });
            panelMenu.Controls.Add(cab);

            panelNavegacion = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = TemaApp.AzulMenu,
                BackColor = TemaApp.AzulMenu
            };
            panelMenu.Controls.Add(panelNavegacion);
            string[] nombres = { "Tauler", "Infants", "Comparativa", "Abandonament", "Jocs", "Usuaris", "Dades", "Configuració" };
            string[] iconos = { "▦", "♙", "▥", "☰", "♧", "♢", "▤", "⚙" };
            for (int i = 0; i < nombres.Length; i++)
            {
                string nom = nombres[i];
                Guna2Button btn = new()
                {
                    Size = new Size(274, 53),
                    BorderRadius = 12,
                    BorderThickness = 0,
                    BorderColor = TemaApp.AzulMenu,
                    FillColor = i == 0 ? TemaApp.AzulSeleccionado : TemaApp.AzulMenu,
                    BackColor = TemaApp.AzulMenu,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Text = iconos[i] + "     " + nom,
                    TextAlign = HorizontalAlignment.Left,
                    TextOffset = new Point(12, 0),
                    Cursor = Cursors.Hand,
                    UseTransparentBackground = false,
                    Location = new Point(13, 14 + i * 59)
                };
                btn.ShadowDecoration.Enabled = false;
                btn.HoverState.FillColor = TemaApp.AzulSeleccionado;
                btn.HoverState.BorderColor = TemaApp.AzulMenu;
                btn.Click += (s, e) => NavegarA(nom);
                panelNavegacion.Controls.Add(btn); botonesMenu[nom] = btn;
            }

            Guna2Panel pie = new()
            {
                Dock = DockStyle.Bottom,
                Height = 125,
                FillColor = Color.FromArgb(39, 53, 79),
                BackColor = TemaApp.AzulMenu,
                BorderRadius = 16
            };
            pie.Controls.Add(new Label
            {
                Text = "     ☀\n  ▥  ♡  ▥",
                Font = new Font("Segoe UI Symbol", 19),
                ForeColor = TemaApp.Amarillo,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(15, 5),
                Size = new Size(240, 75)
            });
            pie.Controls.Add(new Label
            {
                Text = "La Brisa t'acompanya",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(15, 80),
                AutoSize = true
            });
            panelMenu.Controls.Add(pie);
            panelNavegacion.BringToFront();
        }

        private void CrearCabeceraSuperior()
        {
            panelCabecera = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 94,
                FillColor = Color.White,
                BackColor = Color.White,
                BorderRadius = 0
            };
            lblEstado = new Label
            {
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(190, 17),
                Visible = false
            };
            panelCabecera.Controls.Add(new Label
            {
                Text = "P4 · CURS 2026-27",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = TemaApp.Gris,
                AutoSize = true,
                Location = new Point(32, 16)
            });
            panelCabecera.Controls.Add(lblEstado);
            lblTituloCabecera = new Label { Text = $"Bon dia, {SesionActual.PrimerNombre}", Font = new Font("Segoe UI", 21), ForeColor = TemaApp.Texto, AutoSize = true, Location = new Point(31, 35) }; panelCabecera.Controls.Add(lblTituloCabecera);

            FlowLayoutPanel controles = new() { FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoScroll = false, BackColor = Color.White, Size = new Size(595, 55), Anchor = AnchorStyles.Top | AnchorStyles.Right, Location = new Point(450, 19) };
            cmbGrupos = new Guna2ComboBox { Size = new Size(160, 45), BorderRadius = 10, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, FillColor = Color.White, BackColor = Color.White, ForeColor = TemaApp.Texto, Font = new Font("Segoe UI", 9), DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(4, 0, 5, 0) };
            cmbGrupos.HoverState.BorderColor = TemaApp.Turquesa; cmbGrupos.FocusedState.BorderColor = TemaApp.Turquesa;
            cmbGrupos.Items.AddRange(new object[] { "P4-A i P4-B", "P5-A i P5-B", "P4-A", "P4-B", "P5-A", "P5-B" });
            cmbGrupos.SelectedIndex = 0;
            cmbGrupos.SelectedIndexChanged += (s, e) => FiltroCambiado();

            btnFechas = new Guna2Button
            {
                Size = new Size(200, 45),
                BorderRadius = 10,
                FillColor = Color.White,
                BackColor = Color.White,
                ForeColor = TemaApp.Texto,
                BorderColor = TemaApp.BordeSuave,
                BorderThickness = 1,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                Margin = new Padding(4, 0, 5, 0)
            };
            btnFechas.HoverState.BorderColor = TemaApp.Turquesa;
            btnFechas.Text = "▣  " + TemaApp.FormatearRangoFechas(fechaInicio, fechaFin) + "  ▾";

            calendario = new MonthCalendar
            {
                MaxSelectionCount = 92,
                FirstDayOfWeek = Day.Monday,
                ShowToday = false,
                ShowTodayCircle = false,
                TitleBackColor = TemaApp.AzulMenu,
                TitleForeColor = Color.White
            };
            calendario.SetSelectionRange(fechaInicio, fechaFin);
            calendario.DateSelected += (s, e) =>
            {
                fechaInicio = e.Start.Date;
                fechaFin = e.End.Date;
                btnFechas.Text = "▣  " + TemaApp.FormatearRangoFechas(fechaInicio, fechaFin) + "  ▾";
                ddCalendario.Close();
                FiltroCambiado();
            };
            ddCalendario = new ToolStripDropDown
            {
                Padding = new Padding(4),
                AutoClose = true
            };
            ddCalendario.Items.Add(new ToolStripControlHost(calendario)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = true
            });
            btnFechas.Click += (s, e) => ddCalendario.Show(btnFechas, new Point(0, btnFechas.Height + 4));

            Guna2Panel perfil = new() { Size = new Size(215, 48), FillColor = Color.White, BackColor = Color.White, BorderRadius = 0, Margin = new Padding(5, 0, 0, 0) };
            Guna2Panel avatar = new() { Size = new Size(42, 42), Location = new Point(2, 2), BorderRadius = 21, FillColor = Color.FromArgb(220, 239, 238) };
            lblAvatarIniciales = new Label { Text = SesionActual.Iniciales, Dock = DockStyle.Fill, BackColor = Color.Transparent, ForeColor = Color.FromArgb(31, 130, 130), Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter };
            avatar.Controls.Add(lblAvatarIniciales);
            perfil.Controls.Add(avatar);
            lblNombrePerfil = new Label { Text = SesionActual.NombreCompleto, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TemaApp.Texto, AutoSize = true, Location = new Point(52, 4) };
            lblRolPerfil = new Label { Text = SesionActual.RolVisible, Font = new Font("Segoe UI", 8), ForeColor = TemaApp.Gris, AutoSize = true, Location = new Point(52, 25) };
            perfil.Controls.Add(lblNombrePerfil);
            perfil.Controls.Add(lblRolPerfil);

            controles.Controls.Add(cmbGrupos);
            controles.Controls.Add(btnFechas);
            controles.Controls.Add(perfil);
            panelCabecera.Controls.Add(controles);
            panelCabecera.Resize += (s, e) => controles.Location = new Point(Math.Max(270, panelCabecera.Width - controles.Width - 25), 19);
            panelContenido.Controls.Add(panelCabecera);
            panelCabecera.BringToFront();
        }

        private void FrmDashboard_Load(object sender, EventArgs e) { AplicarSesionActual(); }

        public void AplicarSesionActual()
        {
            if (lblAvatarIniciales != null) lblAvatarIniciales.Text = SesionActual.Iniciales;
            if (lblNombrePerfil != null) lblNombrePerfil.Text = SesionActual.NombreCompleto;
            if (lblRolPerfil != null) lblRolPerfil.Text = SesionActual.RolVisible;
            if (lblTituloCabecera != null && vistaActiva is UcTauler)
                lblTituloCabecera.Text = $"Bon dia, {SesionActual.PrimerNombre}";

            if (cmbGrupos != null && SesionActual.EstaAutenticado)
            {
                cmbGrupos.Items.Clear();
                if (SesionActual.EsAdmin)
                {
                    cmbGrupos.Items.AddRange(new object[] { "Tots els grups", "P4-A", "P4-B", "P5-A", "P5-B" });
                }
                else if (SesionActual.GruposPermitidos.Count > 0)
                {
                    foreach (var g in SesionActual.GruposPermitidos)
                        cmbGrupos.Items.Add(g);
                }
                else
                {
                    cmbGrupos.Items.Add("Sense grups assignats");
                }
                cmbGrupos.SelectedIndex = 0;
            }

            foreach (var kv in botonesMenu)
            {
                if (kv.Key is "Usuaris" or "Dades" or "ConfiguraciÃ³")
                    kv.Value.Visible = !SesionActual.EstaAutenticado || SesionActual.EsAdmin;
            }

            _ = RefrescarVistaActualAsync();
        }

        private UserControl CrearModuloConEstado(UserControl modulo)
        {
            if (modulo is UcInfants uInf) { uInf.EstadoCambiado += MostrarEstado; uInf.InfantSeleccionado += AbrirInfant; uInf.NuevoInfantSolicitado += AbrirAltaInfant; }
            if (modulo is UcComparativa uComp) { uComp.EstadoCambiado += MostrarEstado; }
            else if (modulo is UcAbandonament uAband) { uAband.EstadoCambiado += MostrarEstado; }
            else if (modulo is UcJocs uJoc) uJoc.EstadoCambiado += MostrarEstado;
            else if (modulo is UcUsuaris uUsu) uUsu.EstadoCambiado += MostrarEstado;
            else if (modulo is UcDades uDad) uDad.EstadoCambiado += MostrarEstado;
            return modulo;
        }
    }
}

