using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppEmocions.Controls;
using AppEmocions.Helpers;
using AppEmocions.Models.Infants;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public sealed class UcInfantDetall : UserControl, IModuloVista
{
    private readonly string _codigo;
    private InfantDetalleCompletoDto? _detalle;
    private string _tabActiva = "Resum";

    private Panel pnlAvatar = null!;
    private Label lblAvatarIni = null!;
    private Label lblCodigo = null!;
    private Label lblGrupoBadge = null!;
    private Label lblFechasMeta = null!;
    private Label lblKpiSessions = null!;
    private Label lblKpiEncert = null!;
    private Label lblKpiResposta = null!;

    private readonly Dictionary<string, Guna2Button> _tabBtns = new();
    private Panel pnlTabContenido = null!;

    public event Action? VolverSolicitado;
    public event Action<string, bool>? EstadoCambiado;

    public UcInfantDetall(string codigo)
    {
        _codigo = codigo;
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        var marco = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 16, 32, 24), BackColor = TemaApp.Fondo };

        var barraVolver = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = TemaApp.Fondo };
        var btnVolver = new Guna2Button
        {
            Text = "\u2190  Tornar a infants",
            Size = new Size(156, 32),
            BorderRadius = 8,
            FillColor = Color.FromArgb(236, 240, 245),
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Location = new Point(0, 2)
        };
        btnVolver.Click += (s, e) => VolverSolicitado?.Invoke();
        barraVolver.Controls.Add(btnVolver);

        var cardCab = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 114,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(24, 16, 24, 16)
        };

        pnlAvatar = new Panel { Size = new Size(64, 64), Location = new Point(24, 24), BackColor = Color.Transparent };
        pnlAvatar.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var br = new SolidBrush(Color.FromArgb(222, 242, 240));
            e.Graphics.FillEllipse(br, 0, 0, 63, 63);
        };
        lblAvatarIni = new Label
        {
            Text = _codigo.Length >= 2 ? _codigo[..2].ToUpperInvariant() : "IN",
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ForeColor = TemaApp.Turquesa,
            Font = new Font("Segoe UI", 15f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlAvatar.Controls.Add(lblAvatarIni);

        var lblTag = new Label { Text = "INFANT", Location = new Point(108, 16), AutoSize = true, Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = TemaApp.Gris };
        lblCodigo = new Label { Text = _codigo, Location = new Point(106, 33), AutoSize = true, Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        lblGrupoBadge = new Label
        {
            Text = "P4-A",
            Location = new Point(108, 72),
            AutoSize = true,
            Padding = new Padding(8, 2, 8, 2),
            BackColor = Color.FromArgb(226, 244, 242),
            ForeColor = TemaApp.Turquesa,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold)
        };
        lblFechasMeta = new Label
        {
            Text = "Carregant dates...",
            Location = new Point(165, 74),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        };

        var pnlDerecha = new Panel { Dock = DockStyle.Right, Width = 460, BackColor = Color.White };
        pnlDerecha.Controls.Add(new Panel { Location = new Point(0, 14), Size = new Size(1, 56), BackColor = TemaApp.BordeSuave });

        lblKpiSessions = CrearBloqueKpi(pnlDerecha, 20, "0", "Sessions");
        lblKpiEncert = CrearBloqueKpi(pnlDerecha, 115, "0%", "Encert");
        lblKpiResposta = CrearBloqueKpi(pnlDerecha, 210, "0,0 s", "Resposta");

        var btnInforme = new Guna2Button
        {
            Text = "\u2193  Informe",
            Size = new Size(124, 38),
            Location = new Point(316, 22),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            FillColor = Color.White,
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnInforme.Click += (s, e) => ExportarInformeCsv();
        pnlDerecha.Controls.Add(btnInforme);

        cardCab.Controls.Add(pnlDerecha);
        cardCab.Controls.Add(pnlAvatar);
        cardCab.Controls.Add(lblTag);
        cardCab.Controls.Add(lblCodigo);
        cardCab.Controls.Add(lblGrupoBadge);
        cardCab.Controls.Add(lblFechasMeta);

        var barraTabs = new Panel { Dock = DockStyle.Top, Height = 58, Padding = new Padding(0, 12, 0, 6), BackColor = TemaApp.Fondo };
        var contenedorTabs = new Guna2Panel
        {
            Size = new Size(412, 40),
            Location = new Point(0, 10),
            FillColor = Color.FromArgb(236, 239, 244),
            BorderRadius = 11,
            Padding = new Padding(4)
        };

        string[] tabs = ["Resum", "Evoluci\u00f3", "Per nivell", "Sessions"];
        for (int i = 0; i < tabs.Length; i++)
        {
            string tName = tabs[i];
            var btnTab = new Guna2Button
            {
                Text = tName,
                Size = new Size(98, 32),
                Location = new Point(4 + i * 101, 4),
                BorderRadius = 8,
                FillColor = tName == _tabActiva ? Color.White : Color.Transparent,
                ForeColor = tName == _tabActiva ? TemaApp.Texto : TemaApp.Gris,
                Font = new Font("Segoe UI", 9f, tName == _tabActiva ? FontStyle.Bold : FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnTab.Click += (s, e) => SeleccionarTab(tName);
            contenedorTabs.Controls.Add(btnTab);
            _tabBtns[tName] = btnTab;
        }
        barraTabs.Controls.Add(contenedorTabs);

        pnlTabContenido = new Panel { Dock = DockStyle.Fill, BackColor = TemaApp.Fondo };

        marco.Controls.Add(pnlTabContenido);
        marco.Controls.Add(barraTabs);
        marco.Controls.Add(cardCab);
        marco.Controls.Add(barraVolver);
        Controls.Add(marco);
    }

    private static Label CrearBloqueKpi(Panel parent, int x, string val, string sub)
    {
        var lblVal = new Label
        {
            Text = val,
            Location = new Point(x, 14),
            Size = new Size(88, 30),
            Font = new Font("Segoe UI", 14.5f, FontStyle.Bold),
            ForeColor = TemaApp.Texto,
            TextAlign = ContentAlignment.MiddleCenter
        };
        var lblSub = new Label
        {
            Text = sub,
            Location = new Point(x, 46),
            Size = new Size(88, 20),
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris,
            TextAlign = ContentAlignment.MiddleCenter
        };
        parent.Controls.Add(lblVal);
        parent.Controls.Add(lblSub);
        return lblVal;
    }

    private void SeleccionarTab(string tab)
    {
        _tabActiva = tab;
        foreach (var kv in _tabBtns)
        {
            bool act = kv.Key == tab;
            kv.Value.FillColor = act ? Color.White : Color.Transparent;
            kv.Value.ForeColor = act ? TemaApp.Texto : TemaApp.Gris;
            kv.Value.Font = new Font("Segoe UI", 9f, act ? FontStyle.Bold : FontStyle.Regular);
        }
        RenderizarTab();
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct)
    {
        try
        {
            _detalle = await ApiClient.ObtenerDetalleInfantAsync(_codigo, desde, hasta, ct);
            if (_detalle is null)
            {
                EstadoCambiado?.Invoke($"No s'ha trobat l'infant {_codigo}", true);
                return;
            }

            var c = _detalle.Cabecera;
            lblCodigo.Text = string.IsNullOrWhiteSpace(c.Nombre) ? c.Codigo : $"{c.Codigo} \u00b7 {c.Nombre}";
            lblGrupoBadge.Text = c.Grupo;
            lblFechasMeta.Location = new Point(lblGrupoBadge.Right + 14, 74);

            string pStr = c.PrimeraSessio.HasValue ? c.PrimeraSessio.Value.ToString("d MMM yyyy", new CultureInfo("ca-ES")) : "-";
            string uStr = c.UltimaSessio.HasValue ? c.UltimaSessio.Value.ToString("d MMM yyyy", new CultureInfo("ca-ES")) : "-";
            lblFechasMeta.Text = $"Primera sessi\u00f3: {pStr}     \u00daltima sessi\u00f3: {uStr}";

            lblKpiSessions.Text = c.Sessions.ToString();
            lblKpiEncert.Text = $"{Math.Round(c.TaxaEncert)}%";
            lblKpiResposta.Text = $"{(c.TempsRespostaMs / 1000.0):0.0} s";

            RenderizarTab();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error carregant detall: " + ex.Message, true);
        }
    }

    private void RenderizarTab()
    {
        pnlTabContenido.Controls.Clear();
        if (_detalle is null) return;

        if (_tabActiva == "Resum")
            ConstruirVistaResum();
        else if (_tabActiva == "Evoluci\u00f3")
            ConstruirVistaEvolucion();
        else if (_tabActiva == "Per nivell")
            ConstruirVistaPerNivell();
        else
            ConstruirVistaSessions();
    }

    private void ConstruirVistaResum()
    {
        var split = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = TemaApp.Fondo,
            Margin = new Padding(0, 6, 0, 0)
        };
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66f));
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34f));

        var cardIzq = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(22),
            Margin = new Padding(0, 0, 12, 0)
        };

        var cabIzq = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
        cabIzq.Controls.Add(new Label { Text = "Evoluci\u00f3 recent", Location = new Point(0, 0), AutoSize = true, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        cabIzq.Controls.Add(new Label { Text = "Taxa d'encert a les \u00faltimes sessions", Location = new Point(0, 24), AutoSize = true, Font = new Font("Segoe UI", 8.5f), ForeColor = TemaApp.Gris });

        var lnkEvol = new Label { Text = "Veure evoluci\u00f3 >", AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = TemaApp.Turquesa, Cursor = Cursors.Hand };
        lnkEvol.Click += (s, e) => SeleccionarTab("Evoluci\u00f3");
        cabIzq.Controls.Add(lnkEvol);
        cabIzq.Resize += (s, e) => lnkEvol.Location = new Point(cabIzq.Width - lnkEvol.Width - 4, 4);

        cardIzq.Controls.Add(CrearGraficoEvolucion());
        cardIzq.Controls.Add(cabIzq);

        var cardDer = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(22),
            Margin = new Padding(12, 0, 0, 0)
        };

        cardDer.Controls.Add(new Label { Text = "Lectura r\u00e0pida", Location = new Point(22, 20), AutoSize = true, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        cardDer.Controls.Add(new Label { Text = "\u00daltims 14 dies", Location = new Point(22, 44), AutoSize = true, Font = new Font("Segoe UI", 8.5f), ForeColor = TemaApp.Gris });

        var sesOrdenadas = _detalle!.Sesiones.OrderBy(x => x.IniciadaEn).ToList();
        double deltaEncert = 0;
        if (sesOrdenadas.Count >= 2)
        {
            var mitad = Math.Max(1, sesOrdenadas.Count / 2);
            double p1 = sesOrdenadas.Take(mitad).Average(s => (s.Aciertos + s.Errores) > 0 ? s.Aciertos * 100.0 / (s.Aciertos + s.Errores) : 0);
            double p2 = sesOrdenadas.Skip(mitad).Average(s => (s.Aciertos + s.Errores) > 0 ? s.Aciertos * 100.0 / (s.Aciertos + s.Errores) : 0);
            deltaEncert = p2 - p1;
        }

        string tituloProgres = deltaEncert >= 0 ? "Progr\u00e9s constant" : "Ritme irregular";
        string descProgres = $"{(deltaEncert >= 0 ? "+" : "")}{deltaEncert:0}% d'encert respecte a l'inici del per\u00edode.";
        var boxVerde = CrearTarjetaLectura(tituloProgres, descProgres, Color.FromArgb(232, 246, 243), TemaApp.Turquesa, "\u2197", 78);

        double segs = _detalle.Cabecera.TempsRespostaMs / 1000.0;
        string tituloTiempo = segs <= 3.0 ? "Temps estable" : "Temps pausat";
        string descTiempo = $"Respon al voltant de {segs:0.0} segons de mitjana.";
        var boxAmbar = CrearTarjetaLectura(tituloTiempo, descTiempo, Color.FromArgb(253, 246, 232), Color.FromArgb(196, 138, 42), "\u25f7", 162);

        cardDer.Controls.Add(boxVerde);
        cardDer.Controls.Add(boxAmbar);
        cardDer.Resize += (s, e) =>
        {
            boxVerde.Width = cardDer.Width - 44;
            boxAmbar.Width = cardDer.Width - 44;
        };

        split.Controls.Add(cardIzq, 0, 0);
        split.Controls.Add(cardDer, 1, 0);
        pnlTabContenido.Controls.Add(split);
    }

    private static Guna2Panel CrearTarjetaLectura(string titulo, string subtitulo, Color fondo, Color acento, string icono, int y)
    {
        var box = new Guna2Panel
        {
            Location = new Point(22, y),
            Size = new Size(320, 72),
            FillColor = fondo,
            BorderRadius = 12,
            Padding = new Padding(14, 12, 14, 12)
        };
        box.Controls.Add(new Label { Text = icono, Location = new Point(14, 14), Size = new Size(24, 24), Font = new Font("Segoe UI Symbol", 12f, FontStyle.Bold), ForeColor = acento });
        box.Controls.Add(new Label { Text = titulo, Location = new Point(44, 13), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        box.Controls.Add(new Label { Text = subtitulo, Location = new Point(44, 36), AutoSize = true, Font = new Font("Segoe UI", 8.5f), ForeColor = TemaApp.Gris });
        return box;
    }

    private ChartCanvas CrearGraficoEvolucion()
    {
        var chart = new ChartCanvas(EstilGrafico.Linea) { Dock = DockStyle.Fill, BackColor = Color.White };
        var sesiones = _detalle!.Sesiones.OrderBy(s => s.IniciadaEn).ToList();
        var puntos = new List<(string, float)>();

        if (sesiones.Count == 0)
        {
            puntos.Add(("S1", (float)_detalle.Cabecera.TaxaEncert));
        }
        else
        {
            foreach (var s in sesiones)
            {
                int tot = s.Aciertos + s.Errores;
                float pct = tot > 0 ? (float)(s.Aciertos * 100.0 / tot) : (float)_detalle.Cabecera.TaxaEncert;
                puntos.Add((s.IniciadaEn.ToString("dd/MM"), pct));
            }
        }

        chart.SetLinea(puntos);
        return chart;
    }

    private void ConstruirVistaEvolucion()
    {
        var card = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 16, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(24) };
        card.Controls.Add(CrearGraficoEvolucion());
        card.Controls.Add(new Label { Dock = DockStyle.Top, Height = 36, Text = "Evoluci\u00f3 completa de la taxa d'encert (%)", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        pnlTabContenido.Controls.Add(card);
    }

    private void ConstruirVistaPerNivell()
    {
        var card = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 16, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(22) };
        var dgv = CrearTablaBase();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "NIVELL", Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "REGISTRES", Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "COMPLETATS", Width = 110 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ENCERTS", Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ERRORS", Width = 100 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "TAXA ENCERT", Width = 120 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "TEMPS MITJ\u00c0", Width = 120 });

        foreach (var n in _detalle!.PorNivel.OrderBy(x => x.Nivel))
        {
            int tot = n.Aciertos + n.Errores;
            double pct = tot > 0 ? n.Aciertos * 100.0 / tot : 0;
            dgv.Rows.Add($"Nivell {n.Nivel}", n.Registros, n.Completados, n.Aciertos, n.Errores, $"{pct:0}%", $"{(n.RespMediaMs / 1000.0):0.0} s");
        }

        card.Controls.Add(dgv);
        pnlTabContenido.Controls.Add(card);
    }

    private void ConstruirVistaSessions()
    {
        var card = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 16, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(22) };
        var dgv = CrearTablaBase();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DATA", Width = 135 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "JOC", Width = 170 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "NIVELL", Width = 90 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ENCERTS / ERRORS", Width = 130 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DURADA", Width = 95 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ESTAT", Width = 120 });

        foreach (var s in _detalle!.Sesiones.OrderByDescending(x => x.IniciadaEn))
        {
            string estat = s.Abandonada ? "Abandonada" : (s.Completada ? "Completada" : "Incompleta");
            string dur = s.DuracionS.HasValue ? $"{s.DuracionS.Value} s" : "-";
            dgv.Rows.Add(s.IniciadaEn.ToString("dd/MM/yyyy HH:mm"), s.NombreJuego, s.NivelAlcanzado?.ToString() ?? "-", $"{s.Aciertos} / {s.Errores}", dur, estat);
        }

        card.Controls.Add(dgv);
        pnlTabContenido.Controls.Add(card);
    }

    private static Guna2DataGridView CrearTablaBase()
    {
        var dgv = new Guna2DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            GridColor = Color.FromArgb(240, 243, 246),
            RowTemplate = { Height = 46 }
        };
        dgv.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(248, 249, 251);
        dgv.ThemeStyle.HeaderStyle.ForeColor = TemaApp.Gris;
        dgv.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        dgv.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
        dgv.ThemeStyle.RowsStyle.BackColor = Color.White;
        dgv.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
        dgv.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgv.ThemeStyle.RowsStyle.ForeColor = TemaApp.Texto;
        dgv.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(244, 249, 248);
        dgv.ThemeStyle.RowsStyle.SelectionForeColor = TemaApp.Texto;
        return dgv;
    }

    private void ExportarInformeCsv()
    {
        if (_detalle is null) return;
        using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = $"Informe_{_codigo}.csv" };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        var sb = new StringBuilder();
        sb.AppendLine("Data;Joc;Nivell;Encerts;Errors;DuradaS;Completada;Abandonada");
        foreach (var s in _detalle.Sesiones)
            sb.AppendLine($"{s.IniciadaEn:yyyy-MM-dd HH:mm};{s.NombreJuego};{s.NivelAlcanzado};{s.Aciertos};{s.Errores};{s.DuracionS};{s.Completada};{s.Abandonada}");

        File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
        EstadoCambiado?.Invoke($"Informe exportat: {Path.GetFileName(sfd.FileName)}", false);
    }
}