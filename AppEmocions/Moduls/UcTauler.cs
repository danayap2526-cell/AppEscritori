using System.Drawing;
using System.Windows.Forms;
using AppEmocions.Controls;
using AppEmocions.Helpers;
using AppEmocions.Models.Dashboard;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public class UcTauler : UserControl, IModuloVista
{
    private readonly Label[] lblValores = new Label[6];
    private readonly Label[] lblVariaciones = new Label[6];
    private ChartCanvas graficoLinea = null!;
    private ChartCanvas graficoBarras = null!;
    private Label lblSubEvolucion = null!;
    private Guna2Panel panelAtencion = null!;
    private Label lblSinInfants = null!;
    private readonly List<Guna2Panel> filasAtencion = new();

    public event Action<string>? InfantSeleccionado;
    public event Action<string, bool>? EstadoCambiado;
    public event Action<string>? NavegarASeccion;

    public UcTauler()
    {
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        AutoScroll = true;
        ConstruirInterfaz();
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default)
    {
        DateTime ini = desde.Date;
        DateTime fin = hasta.Date;
        int dias = (fin - ini).Days + 1;
        DateTime finPrev = ini.AddDays(-1);
        DateTime iniPrev = ini.AddDays(-dias);

        lblSubEvolucion.Text = grupo == null ? "Mitjana dels dos grups" : $"Grup {grupo}";
        EstadoCambiado?.Invoke("Carregant dades…", false);

        try
        {
            var tResumen = ApiClient.ResumenAsync(ini, fin, grupo, ct);
            var tPrevio = ApiClient.ResumenAsync(iniPrev, finPrev, grupo, ct);
            var tSemanas = ApiClient.SemanasAsync(ini, fin, grupo, ct);
            var tNiveles = ApiClient.NivelesAsync(ini, fin, grupo, ct);
            var tAtencion = ApiClient.AtencionAsync(ini, fin, grupo, ct);

            await Task.WhenAll(tResumen, tPrevio, tSemanas, tNiveles, tAtencion);
            if (ct.IsCancellationRequested || IsDisposed) return;

            PintarKpis(DashboardCalculator.CalcularKpis(await tResumen, await tPrevio));
            graficoLinea.SetLinea(DashboardCalculator.SerieSemanas(await tSemanas ?? new()));
            graficoBarras.SetBarras(DashboardCalculator.SerieNiveles(await tNiveles ?? new()));
            CargarFilasAtencion((await tAtencion ?? new()).Select(a => new InfantAtencion(a.Codigo, a.Motivo, a.Estado)).ToList());
            EstadoCambiado?.Invoke("", false);
        }
        catch (Exception) when (ct.IsCancellationRequested) { }
        catch (Exception)
        {
            if (IsDisposed) return;
            LimpiarDatos();
            EstadoCambiado?.Invoke("● Sense connexió amb el servidor", true);
        }
    }

    private void LimpiarDatos()
    {
        for (int i = 0;
            i < 6;
            i++)
        {
            lblValores[i].Text = "–";
            lblVariaciones[i].Visible = false;
        }
        graficoLinea.SetLinea(Array.Empty<(string, float)>());
        graficoBarras.SetBarras(Array.Empty<(string, float, float)>());
        CargarFilasAtencion(new List<InfantAtencion>(), mostrarVacio: false);
    }

    private void PintarKpis(KpiResultado[] resultados)
    {
        for (int i = 0; i < 6; i++)
        {
            var r = resultados[i];
            lblValores[i].Text = r.ValorTexto;
            Label lbl = lblVariaciones[i];
            lbl.Visible = r.MostrarVariacion;
            if (!r.MostrarVariacion) continue;
            lbl.Text = r.VariacionTexto;
            if (r.EstadoVariacion == 0)
            {
                lbl.ForeColor = TemaApp.Gris;
                lbl.BackColor = Color.FromArgb(240, 242, 245);
            }
            else if (r.EstadoVariacion > 0)
            {
                lbl.ForeColor = TemaApp.VerdePositivo;
                lbl.BackColor = TemaApp.VerdePositivoFondo;
            }
            else
            {
                lbl.ForeColor = TemaApp.NaranjaNegativo;
                lbl.BackColor = TemaApp.NaranjaNegativoFondo;
            }
        }
    }

    private void ConstruirInterfaz()
    {
        Color colorAviso = Color.FromArgb(239, 250, 248);
        Guna2Panel aviso = CrearTarjeta();
        aviso.Location = new Point(32, 24);
        aviso.Size = new Size(1200, 76);
        aviso.FillColor = colorAviso;
        aviso.BorderColor = Color.FromArgb(195, 232, 228);
        aviso.BorderThickness = 1;
        aviso.ShadowDecoration.Enabled = false;
        aviso.Padding = new Padding(4);
        Guna2Panel zonaTextos = new()
        {
            Dock = DockStyle.Fill,
            FillColor = colorAviso,
            BackColor = colorAviso
        };
        zonaTextos.Controls.Add(new Label
        {
            Text = "Una dada per acompanyar, no per etiquetar",
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 125, 128),
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(0, 14)
        });
        zonaTextos.Controls.Add(new Label
        {
            Text = "Aquestes dades són senyals per observar, no un diagnòstic.",
            Font = new Font("Segoe UI", 9),
            ForeColor = TemaApp.Gris,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(0, 39)
        });
        Guna2Panel zonaIcono = new()
        {
            Dock = DockStyle.Left,
            Width = 52,
            FillColor = colorAviso,
            BackColor = colorAviso
        };
        zonaIcono.Controls.Add(new Label
        {
            Text = "ⓘ",
            Font = new Font("Segoe UI Symbol", 19),
            ForeColor = Color.FromArgb(28, 139, 136),
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        });
        Guna2Button btnCerrarAviso = new()
        {
            Dock = DockStyle.Right,
            Width = 52,
            FillColor = colorAviso,
            BackColor = colorAviso,
            ForeColor = TemaApp.Gris,
            Font = new Font("Segoe UI Symbol", 12),
            Text = "✕",
            BorderRadius = 12,
            Cursor = Cursors.Hand
        };
        btnCerrarAviso.HoverState.FillColor = Color.FromArgb(220, 240, 237);
        btnCerrarAviso.ShadowDecoration.Enabled = false;
        aviso.Controls.Add(zonaTextos);
        aviso.Controls.Add(zonaIcono);
        aviso.Controls.Add(btnCerrarAviso);
        zonaTextos.BringToFront();

        TableLayoutPanel indicadores = new()
        {
            Location = new Point(32, 118),
            Size = new Size(1200, 162),
            ColumnCount = 6,
            RowCount = 1,
            BackColor = TemaApp.Fondo
        };
        for (int i = 0;
            i < 6;
            i++) indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 6));
        indicadores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        string[] titulos = { "Infants actius", "Sessions", "Puntuació mitjana", "Taxa d'encert", "Temps de resposta", "Taxa d'abandonament" };
        string[] iconos = { "☺", "▷", "◎", "✓", "◷", "☰" };
        Color[] fondosIconos = { Color.FromArgb(235, 248, 246), Color.FromArgb(255, 244, 222), Color.FromArgb(233, 239, 249), Color.FromArgb(230, 245, 238), Color.FromArgb(242, 235, 251), Color.FromArgb(249, 239, 233) };
        Color[] coloresIconos = { Color.FromArgb(32, 160, 153), Color.FromArgb(210, 140, 20), Color.FromArgb(80, 112, 166), Color.FromArgb(52, 145, 105), Color.FromArgb(135, 99, 181), Color.FromArgb(206, 104, 55) };
        for (int i = 0; i < 6; i++)
        {
            Guna2Panel tarjeta = CrearTarjeta();
            tarjeta.Dock = DockStyle.Fill;
            tarjeta.Margin = new Padding(0, 0, 12, 0);
            Guna2Panel icono = new()
            {
                Size = new Size(42, 42),
                Location = new Point(17, 17),
                BorderRadius = 12,
                FillColor = fondosIconos[i]
            };
            icono.Controls.Add(new Label
            {
                Text = iconos[i],
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = coloresIconos[i],
                Font = new Font("Segoe UI Symbol", 18),
                TextAlign = ContentAlignment.MiddleCenter
            });
            Label valor = new()
            {
                Text = "–",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = TemaApp.Texto,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(17, 75)
            };
            Label titulo = new()
            {
                Text = titulos[i],
                Font = new Font("Segoe UI", 9),
                ForeColor = TemaApp.Gris,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(17, 124)
            };
            Label variacion = new()
            {
                Text = "",
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(5, 4, 5, 4),
                Visible = false
            };
            lblValores[i] = valor; lblVariaciones[i] = variacion;
            tarjeta.Controls.Add(icono);
            tarjeta.Controls.Add(valor);
            tarjeta.Controls.Add(titulo);
            tarjeta.Controls.Add(variacion);
            AnclarDerecha(tarjeta, variacion, 80, 17); indicadores.Controls.Add(tarjeta, i, 0);
        }

        TableLayoutPanel inferiores = new()
        {
            Location = new Point(32, 300),
            Size = new Size(1200, 390),
            ColumnCount = 3,
            RowCount = 1,
            BackColor = TemaApp.Fondo
        };
        inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
        inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
        inferiores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
        inferiores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        inferiores.Controls.Add(CrearPanelEvolucion(), 0, 0);
        inferiores.Controls.Add(CrearPanelNiveles(), 1, 0);
        inferiores.Controls.Add(CrearPanelAtencion(), 2, 0);
        Controls.Add(aviso); Controls.Add(indicadores); Controls.Add(inferiores);
        void Ajustar()
        {
            int ancho = Math.Max(700, ClientSize.Width - 64);
            aviso.Width = ancho;
            indicadores.Width = ancho;
            inferiores.Width = ancho;
            inferiores.Height = Math.Max(390, ClientSize.Height - inferiores.Top - 24);
        }
        btnCerrarAviso.Click += (s, e) =>
        {
            int d = aviso.Height + 18;
            aviso.Visible = false;
            indicadores.Top -= d;
            inferiores.Top -= d;
            Ajustar();
        };
        Resize += (s, e) => Ajustar();
    }

    private Guna2Panel CrearPanelEvolucion()
    {
        Guna2Panel t = CrearTarjeta();
        t.Dock = DockStyle.Fill;
        t.Margin = new Padding(0, 0, 14, 0);
        t.Padding = new Padding(20, 85, 20, 25);
        lblSubEvolucion = CrearSubtituloPanel("Mitjana dels dos grups", 20, 50);
        Label enlace = new()
        {
            Text = "Veure evolució  ›",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.FromArgb(28, 145, 143),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            AutoSize = true
        };
        enlace.Click += (s, e) => NavegarASeccion?.Invoke("Comparativa");
        graficoLinea = new ChartCanvas(EstilGrafico.Linea) { Dock = DockStyle.Fill };
        t.Controls.Add(CrearTituloPanel("Taxa d'encert per setmana", 20, 20));
        t.Controls.Add(lblSubEvolucion);
        t.Controls.Add(enlace);
        t.Controls.Add(graficoLinea);
        AnclarDerecha(t, enlace, 24); return t;
    }

    private Guna2Panel CrearPanelNiveles()
    {
        Guna2Panel t = CrearTarjeta();
        t.Dock = DockStyle.Fill;
        t.Margin = new Padding(0, 0, 14, 0);
        t.Padding = new Padding(15, 85, 15, 25);
        graficoBarras = new ChartCanvas(EstilGrafico.Barras) { Dock = DockStyle.Fill };
        t.Controls.Add(CrearTituloPanel("Encerts i errors per nivell", 18, 20));
        t.Controls.Add(CrearSubtituloPanel("Sessions puntuables", 18, 50));
        t.Controls.Add(graficoBarras);
        return t;
    }

    private Guna2Panel CrearPanelAtencion()
    {
        Guna2Panel t = CrearTarjeta();
        t.Dock = DockStyle.Fill;
        t.Margin = new Padding(0);
        panelAtencion = t;
        Label enlace = new()
        {
            Text = "Veure tots  ›",
            Font = new Font("Segoe UI", 8, FontStyle.Bold),
            ForeColor = Color.FromArgb(28, 145, 143),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            AutoSize = true
        };
        enlace.Click += (s, e) => NavegarASeccion?.Invoke("Infants");
        lblSinInfants = new Label
        {
            Text = "Cap senyal destacat en aquest període.",
            Font = new Font("Segoe UI", 9),
            ForeColor = TemaApp.Gris,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(18, 100),
            Visible = false
        };
        t.Controls.Add(CrearTituloPanel("Infants que mereixen atenció", 18, 20));
        t.Controls.Add(CrearSubtituloPanel("Senyals recents", 18, 50));
        t.Controls.Add(enlace);
        t.Controls.Add(lblSinInfants);
        AnclarDerecha(t, enlace, 24);
        t.Resize += (s, e) => { foreach (Guna2Panel fila in filasAtencion) fila.Width = Math.Max(100, t.Width - 28); };
        return t;
    }

    private void CargarFilasAtencion(List<InfantAtencion> infants, bool mostrarVacio = true)
    {
        foreach (Guna2Panel fila in filasAtencion)
        {
            panelAtencion.Controls.Remove(fila);
            fila.Dispose();
        }
        filasAtencion.Clear(); lblSinInfants.Visible = mostrarVacio && infants.Count == 0;
        Color[] colores = { Color.FromArgb(220, 240, 239), Color.FromArgb(244, 235, 215), Color.FromArgb(237, 229, 245) };
        for (int i = 0; i < infants.Count; i++)
        {
            Guna2Panel fila = CrearFilaInfant(infants[i], colores[i % colores.Length]);
            fila.Location = new Point(14, 85 + i * 78);
            fila.Width = Math.Max(100, panelAtencion.Width - 28);
            panelAtencion.Controls.Add(fila); filasAtencion.Add(fila);
        }
    }

    private Guna2Panel CrearFilaInfant(InfantAtencion infant, Color colorAvatar)
    {
        Color normal = Color.White, resaltado = Color.FromArgb(244, 248, 249);
        Guna2Panel fila = new()
        {
            Size = new Size(320, 70),
            BorderRadius = 12,
            FillColor = normal,
            BackColor = normal,
            Tag = infant.Codigo
        };
        Guna2Panel avatar = new()
        {
            Size = new Size(48, 48),
            Location = new Point(8, 11),
            BorderRadius = 24,
            FillColor = colorAvatar
        };
        avatar.Controls.Add(new Label
        {
            Text = infant.Codigo.Length >= 2 ? infant.Codigo.Substring(0, 2) : infant.Codigo,
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        });
        Label codigo = new()
        {
            Text = infant.Codigo,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = TemaApp.Texto,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(68, 14)
        };
        Label motivo = new()
        {
            Text = infant.Motivo,
            Font = new Font("Segoe UI", 7),
            ForeColor = TemaApp.Gris,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(68, 39)
        };
        Label estado = new()
        {
            Text = infant.Estado,
            Font = new Font("Segoe UI", 7, FontStyle.Bold),
            ForeColor = Color.FromArgb(150, 103, 31),
            BackColor = Color.FromArgb(255, 244, 220),
            AutoSize = true,
            Padding = new Padding(7, 5, 7, 5)
        };
        Label flecha = new()
        {
            Text = "›",
            Font = new Font("Segoe UI", 16),
            ForeColor = TemaApp.Texto,
            BackColor = Color.Transparent,
            AutoSize = true
        };
        fila.Controls.Add(avatar);
        fila.Controls.Add(codigo);
        fila.Controls.Add(motivo);
        fila.Controls.Add(estado);
        fila.Controls.Add(flecha);
        AnclarDerecha(fila, flecha, 18, 12); AnclarDerecha(fila, estado, 22, 36);
        void Enlazar(Control c)
        {
            c.Cursor = Cursors.Hand;
            c.Click += (s, e) => InfantSeleccionado?.Invoke(infant.Codigo);
            c.MouseEnter += (s, e) => fila.FillColor = resaltado;
            c.MouseLeave += (s, e) => { if (!fila.ClientRectangle.Contains(fila.PointToClient(Cursor.Position))) fila.FillColor = normal; };
            foreach (Control hijo in c.Controls) Enlazar(hijo);
        }
        Enlazar(fila); return fila;
    }

    private static Guna2Panel CrearTarjeta()
    {
        Guna2Panel t = new()
        {
            FillColor = Color.White,
            BackColor = TemaApp.Fondo,
            BorderRadius = 16,
            BorderThickness = 0
        };
        t.ShadowDecoration.Enabled = true;
        t.ShadowDecoration.Depth = 3;
        t.ShadowDecoration.Color = Color.FromArgb(225, 225, 225);
        return t;
    }
    private static void AnclarDerecha(Control cont, Control c, int top, int margen = 20)
    {
        void Col() => c.Location = new Point(cont.Width - c.Width - margen, top);
        cont.Resize += (s, e) => Col();
        c.SizeChanged += (s, e) => Col();
        Col();
    }
    private static Label CrearTituloPanel(string txt, int x, int y) => new()
    {
        Text = txt,
        Font = new Font("Segoe UI", 11, FontStyle.Bold),
        ForeColor = TemaApp.Texto,
        BackColor = Color.Transparent,
        AutoSize = true,
        Location = new Point(x, y)
    };
    private static Label CrearSubtituloPanel(string txt, int x, int y) => new()
    {
        Text = txt,
        Font = new Font("Segoe UI", 8),
        ForeColor = TemaApp.Gris,
        BackColor = Color.Transparent,
        AutoSize = true,
        Location = new Point(x, y)
    };
}
