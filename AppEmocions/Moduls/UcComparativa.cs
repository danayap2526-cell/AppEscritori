using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppEmocions.Controls;
using AppEmocions.Helpers;
using AppEmocions.Models.Dashboard;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public sealed class UcComparativa : UserControl, IModuloVista
{
    private TableLayoutPanel tlpGrupos = null!;
    private Label lblSubtituloGrafico = null!;
    private GraficoBarrasComparativa graficoNiveles = null!;
    private Guna2Panel cardTabla = null!;
    private Guna2DataGridView dgvDetalle = null!;

    private List<ComparativaGrupoDto> _grupos = new();
    private List<ComparativaNivelGrupoDto> _niveles = new();
    private string _columnaOrden = "Grupo";
    private bool _ordenAsc = true;

    private static readonly Color[] PaletaGrupos =
    [
        Color.FromArgb(46, 178, 166),
        Color.FromArgb(148, 108, 196),
        Color.FromArgb(232, 166, 72),
        Color.FromArgb(86, 148, 224)
    ];

    private static readonly Color[] PaletaFondosBadge =
    [
        Color.FromArgb(224, 243, 241),
        Color.FromArgb(238, 231, 247),
        Color.FromArgb(253, 244, 228),
        Color.FromArgb(228, 240, 253)
    ];

    public event Action<string, bool>? EstadoCambiado;

    public UcComparativa()
    {
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(32, 14, 32, 28),
            BackColor = TemaApp.Fondo
        };

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = TemaApp.Fondo
        };
        stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        tlpGrupos = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 162,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = TemaApp.Fondo,
            Margin = new Padding(0, 0, 0, 18)
        };
        tlpGrupos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        tlpGrupos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

        var cardGrafico = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 315,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(24, 20, 24, 18),
            Margin = new Padding(0, 0, 0, 18)
        };

        var cabGraf = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
        cabGraf.Controls.Add(new Label
        {
            Text = "Comparativa per nivell",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        lblSubtituloGrafico = new Label
        {
            Text = "Taxa d'encert mitjana",
            Location = new Point(0, 24),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        };
        cabGraf.Controls.Add(lblSubtituloGrafico);

        graficoNiveles = new GraficoBarrasComparativa
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        cardGrafico.Controls.Add(graficoNiveles);
        cardGrafico.Controls.Add(cabGraf);

        cardTabla = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 320,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(24, 20, 24, 16)
        };

        var cabTabla = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.White };
        cabTabla.Controls.Add(new Label
        {
            Text = "Detall comparatiu",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        cabTabla.Controls.Add(new Label
        {
            Text = "Ordena per qualsevol indicador",
            Location = new Point(0, 24),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        });

        cardGrafico.ShadowDecoration.Enabled = false; cardGrafico.BorderColor = Color.FromArgb(238, 240, 244);
        cardGrafico.ShadowDecoration.Depth = 8;
        cardGrafico.ShadowDecoration.Color = Color.FromArgb(18, 15, 23, 42);

        cardTabla.ShadowDecoration.Enabled = false; cardTabla.BorderColor = Color.FromArgb(238, 240, 244);
        cardTabla.ShadowDecoration.Depth = 8;
        cardTabla.ShadowDecoration.Color = Color.FromArgb(18, 15, 23, 42);

        dgvDetalle = new Guna2DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false,
            EnableHeadersVisualStyles = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.None,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            GridColor = Color.FromArgb(241, 245, 249),
            ScrollBars = ScrollBars.Vertical,
            Cursor = Cursors.Hand,
            RowTemplate = { Height = 54 }
        };
        dgvDetalle.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
        dgvDetalle.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvDetalle.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

        dgvDetalle.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvDetalle.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvDetalle.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
        dgvDetalle.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 116, 139);
        dgvDetalle.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvDetalle.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvDetalle.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvDetalle.ThemeStyle.HeaderStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvDetalle.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvDetalle.ThemeStyle.HeaderStyle.Height = 42;
        dgvDetalle.ThemeStyle.RowsStyle.BackColor = Color.White;
        dgvDetalle.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
        dgvDetalle.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvDetalle.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(30, 41, 59);
        dgvDetalle.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(241, 248, 247);
        dgvDetalle.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        dgvDetalle.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grupo", HeaderText = "GRUP", FillWeight = 16 });
        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sesiones", HeaderText = "SESSIONS", FillWeight = 15 });
        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "TasaAcierto", HeaderText = "ENCERT", FillWeight = 15 });
        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "RespMediaMs", HeaderText = "TEMPS DE RESPOSTA", FillWeight = 18 });
        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "TasaFinalizacion", HeaderText = "FINALITZACI\u00d3", FillWeight = 18 });
        dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tendencia", HeaderText = "TEND\u00c8NCIA", FillWeight = 18 });

        dgvDetalle.RowPostPaint += (s, e) =>
{
    using var pen = new Pen(Color.FromArgb(241, 245, 249), 1);
    e.Graphics.DrawLine(pen, e.RowBounds.Left + 8, e.RowBounds.Bottom - 1, e.RowBounds.Right - 8, e.RowBounds.Bottom - 1);
};
        dgvDetalle.ColumnHeaderMouseClick += DgvDetalle_ColumnHeaderMouseClick;
        dgvDetalle.CellPainting += DgvDetalle_CellPainting;

        cardTabla.Controls.Add(dgvDetalle);
        cardTabla.Controls.Add(cabTabla);

        stack.Controls.Add(tlpGrupos, 0, 0);
        stack.Controls.Add(cardGrafico, 0, 1);
        stack.Controls.Add(cardTabla, 0, 2);

        scroll.Controls.Add(stack); stack.SizeChanged += (s, e) => scroll.AutoScrollMinSize = new Size(0, stack.Height + 48);
        Controls.Add(scroll);
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct)
    {
        try
        {
            var cult = new CultureInfo("ca-ES");
            lblSubtituloGrafico.Text = $"Taxa d'encert mitjana \u00b7 {desde.ToString("d MMM", cult)} \u2013 {hasta.ToString("d MMM yyyy", cult)}";

            var tGrupos = ApiClient.ComparativaGruposAsync(desde, hasta, grupo, ct);
            var tNiveles = ApiClient.ComparativaNivelesAsync(desde, hasta, grupo, ct);
            await Task.WhenAll(tGrupos, tNiveles);

            _grupos = tGrupos.Result.OrderBy(g => g.Grupo).ToList();
            _niveles = tNiveles.Result;

            RenderizarTarjetasGrupos();
            graficoNiveles.EstablecerDatos(_grupos.Select(g => g.Grupo).ToList(), _niveles, PaletaGrupos);
            RenderizarTablaDetalle();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error carregant comparativa: " + ex.Message, true);
        }
    }

    private void RenderizarTarjetasGrupos()
    {
        tlpGrupos.Controls.Clear();
        tlpGrupos.ColumnStyles.Clear();

        int n = Math.Max(1, _grupos.Count);
        tlpGrupos.ColumnCount = n;
        for (int i = 0; i < n; i++)
            tlpGrupos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / n));

        if (_grupos.Count == 0)
        {
            var vacio = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 16,
                BorderThickness = 1,
                BorderColor = TemaApp.BordeSuave
            };
            vacio.Controls.Add(new Label
            {
                Text = "No hi ha dades de grups per al per\u00edode seleccionat.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10f),
                ForeColor = TemaApp.Gris
            });
            tlpGrupos.Controls.Add(vacio, 0, 0);
            return;
        }

        for (int i = 0; i < _grupos.Count; i++)
        {
            var g = _grupos[i];
            Color acento = PaletaGrupos[i % PaletaGrupos.Length];
            Color fondoBadge = PaletaFondosBadge[i % PaletaFondosBadge.Length];

            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 16,
                BorderThickness = 1,
                BorderColor = TemaApp.BordeSuave,
                Margin = new Padding(i == 0 ? 0 : 8, 0, i == _grupos.Count - 1 ? 0 : 8, 0)
            };

            card.ShadowDecoration.Enabled = false; card.BorderColor = Color.FromArgb(238, 240, 244);
            card.ShadowDecoration.Depth = 8;
            card.ShadowDecoration.Color = Color.FromArgb(18, 15, 23, 42);

            var badge = new Guna2Panel
            {
                Location = new Point(20, 18),
                Size = new Size(74, 32),
                FillColor = fondoBadge,
                BorderRadius = 9
            };
            badge.Controls.Add(new Label
            {
                Text = g.Grupo,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                ForeColor = acento,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold)
            });

            var lblNumInfants = new Label
            {
                Text = g.TotalInfants.ToString(),
                AutoSize = true,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = TemaApp.Texto
            };
            var lblTxtInfants = new Label
            {
                Text = "infants",
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = TemaApp.Gris
            };

            var divHoriz = new Panel
            {
                Location = new Point(20, 66),
                Height = 1,
                BackColor = TemaApp.BordeSuave
            };

            var kpiEncert = CrearKpiGrupo(card, $"{Math.Round(g.TasaAcierto)}%", "Encert mitj\u00e0");
            var sep1 = new Panel { Size = new Size(1, 36), BackColor = TemaApp.BordeSuave };
            var kpiResp = CrearKpiGrupo(card, $"{(g.RespMediaMs / 1000.0):0.0} s", "Resposta");
            var sep2 = new Panel { Size = new Size(1, 36), BackColor = TemaApp.BordeSuave };
            var kpiAband = CrearKpiGrupo(card, $"{Math.Round(g.TasaAbandono)}%", "Aband.");

            card.Controls.Add(badge);
            card.Controls.Add(lblNumInfants);
            card.Controls.Add(lblTxtInfants);
            card.Controls.Add(divHoriz);
            card.Controls.Add(sep1);
            card.Controls.Add(sep2);

            card.Resize += (s, e) =>
            {
                lblNumInfants.Location = new Point(card.Width - lblNumInfants.Width - 20, 16);
                lblTxtInfants.Location = new Point(card.Width - lblTxtInfants.Width - 20, 38);
                divHoriz.Width = Math.Max(40, card.Width - 40);

                int innerW = Math.Max(180, card.Width - 40);
                int colW = innerW / 3;
                int x1 = 20;
                int x2 = 20 + colW;
                int x3 = 20 + colW * 2;

                kpiEncert.Width = colW - 8;
                kpiResp.Width = colW - 8;
                kpiAband.Width = colW - 4;

                kpiEncert.Location = new Point(x1, 82);
                sep1.Location = new Point(x2 - 8, 86);
                kpiResp.Location = new Point(x2, 82);
                sep2.Location = new Point(x3 - 8, 86);
                kpiAband.Location = new Point(x3, 82);
            };

            tlpGrupos.Controls.Add(card, i, 0);
        }
    }

    private static Panel CrearKpiGrupo(Control parent, string valor, string etiqueta)
    {
        var box = new Panel { Size = new Size(92, 48), BackColor = Color.White };
        box.Controls.Add(new Label
        {
            Text = valor,
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        box.Controls.Add(new Label
        {
            Text = etiqueta,
            Location = new Point(0, 28),
            AutoSize = true,
            Font = new Font("Segoe UI", 8f),
            ForeColor = TemaApp.Gris
        });
        parent.Controls.Add(box);
        return box;
    }

    private void DgvDetalle_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        string col = dgvDetalle.Columns[e.ColumnIndex].Name;
        if (_columnaOrden == col)
            _ordenAsc = !_ordenAsc;
        else
        {
            _columnaOrden = col;
            _ordenAsc = true;
        }
        RenderizarTablaDetalle();
    }

    private void RenderizarTablaDetalle()
    {
        dgvDetalle.Rows.Clear();
        int filas = Math.Max(1, _grupos.Count);
        cardTabla.Height = 136 + filas * 56;

        if (_grupos.Count == 0) return;

        double mediaGlobal = _grupos.Average(g => g.TasaAcierto);

        IEnumerable<ComparativaGrupoDto> ordenados = _columnaOrden switch
        {
            "Sesiones" => _ordenAsc ? _grupos.OrderBy(g => g.Sesiones) : _grupos.OrderByDescending(g => g.Sesiones),
            "TasaAcierto" => _ordenAsc ? _grupos.OrderBy(g => g.TasaAcierto) : _grupos.OrderByDescending(g => g.TasaAcierto),
            "RespMediaMs" => _ordenAsc ? _grupos.OrderBy(g => g.RespMediaMs) : _grupos.OrderByDescending(g => g.RespMediaMs),
            "TasaFinalizacion" => _ordenAsc ? _grupos.OrderBy(g => g.TasaFinalizacion) : _grupos.OrderByDescending(g => g.TasaFinalizacion),
            "Tendencia" => _ordenAsc ? _grupos.OrderBy(g => g.TasaAcierto - mediaGlobal) : _grupos.OrderByDescending(g => g.TasaAcierto - mediaGlobal),
            _ => _ordenAsc ? _grupos.OrderBy(g => g.Grupo) : _grupos.OrderByDescending(g => g.Grupo)
        };

        foreach (var g in ordenados)
        {
            double diff = g.TasaAcierto - mediaGlobal;
            string tendStr = diff >= 0 ? $"\u25b2 {diff:0.0}%" : $"\u25bc {Math.Abs(diff):0.0}%";
            dgvDetalle.Rows.Add(
                g.Grupo,
                g.Sesiones,
                $"{Math.Round(g.TasaAcierto)}%",
                $"{(g.RespMediaMs / 1000.0):0.0} s",
                $"{Math.Round(g.TasaFinalizacion)}%",
                tendStr
            );
        }
    }

    private void DgvDetalle_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0) return;

        if (e.ColumnIndex == 0)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            string grp = e.Value?.ToString() ?? "";
            int idx = _grupos.FindIndex(x => x.Grupo == grp);
            if (idx < 0) idx = e.RowIndex;

            Color acento = PaletaGrupos[idx % PaletaGrupos.Length];
            Color fondo = PaletaFondosBadge[idx % PaletaFondosBadge.Length];

            var rect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + (e.CellBounds.Height - 26) / 2, 54, 26);
            using (var brBg = new SolidBrush(fondo))
                e.Graphics.FillRoundedRectangle(brBg, rect, new Size(7, 7));

            using (var f = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var brTxt = new SolidBrush(acento))
            {
                var sz = e.Graphics.MeasureString(grp, f);
                e.Graphics.DrawString(grp, f, brTxt, rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2);
            }
            e.Handled = true;
        }
        else if (e.ColumnIndex == 5)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            string val = e.Value?.ToString() ?? "";
            bool positivo = val.StartsWith("\u25b2");

            Color bg = positivo ? Color.FromArgb(228, 245, 242) : Color.FromArgb(253, 236, 234);
            Color fg = positivo ? TemaApp.Turquesa : Color.FromArgb(212, 78, 72);

            var rect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + (e.CellBounds.Height - 26) / 2, 72, 26);
            using (var brBg = new SolidBrush(bg))
                e.Graphics.FillRoundedRectangle(brBg, rect, new Size(7, 7));

            using (var f = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var brTxt = new SolidBrush(fg))
            {
                var sz = e.Graphics.MeasureString(val, f);
                e.Graphics.DrawString(val, f, brTxt, rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2);
            }
            e.Handled = true;
        }
    }

    private sealed class GraficoBarrasComparativa : Panel
    {
        private List<string> _grupos = new();
        private List<ComparativaNivelGrupoDto> _datos = new();
        private Color[] _paleta = Array.Empty<Color>();

        private static readonly Dictionary<byte, string> NombresNiveles = new()
        {
            [1] = "La Pla\u00e7a",
            [2] = "El Parc",
            [3] = "La Pla\u00e7a Gran",
            [4] = "El Jard\u00ed"
        };

        public GraficoBarrasComparativa()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        public void EstablecerDatos(List<string> grupos, List<ComparativaNivelGrupoDto> datos, Color[] paleta)
        {
            _grupos = grupos;
            _datos = datos;
            _paleta = paleta;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var niveles = _datos.Select(d => d.Nivel).Distinct().OrderBy(n => n).ToList();
            if (niveles.Count == 0 || _grupos.Count == 0)
            {
                using var f = new Font("Segoe UI", 9.5f);
                using var br = new SolidBrush(TemaApp.Gris);
                const string msg = "Sense dades per nivell en aquest per\u00edode";
                var sz = g.MeasureString(msg, f);
                g.DrawString(msg, f, br, (Width - sz.Width) / 2, (Height - sz.Height) / 2);
                return;
            }

            int top = 20;
            int bottom = Height - 64;
            int left = 24;
            int right = Width - 24;
            int chartH = Math.Max(40, bottom - top);

            using (var penGrid = new Pen(Color.FromArgb(241, 245, 249), 1))
            using (var fScale = new Font("Segoe UI", 7.5f))
            using (var brScale = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                foreach (int pctMark in new[] { 25, 50, 75, 100 })
                {
                    int gy = bottom - (int)Math.Round(chartH * pctMark / 100.0);
                    g.DrawLine(penGrid, left + 26, gy, right, gy);
                    g.DrawString($"{pctMark}%", fScale, brScale, left - 6, gy - 7);
                }
            }
            left += 26;
            using (var penBase = new Pen(TemaApp.BordeSuave, 1))
                g.DrawLine(penBase, left, bottom, right, bottom);

            int slotW = (right - left) / niveles.Count;
            int barW = 24;
            int gap = 8;

            using var fNiv = new Font("Segoe UI", 8.5f);
            using var fVal = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            using var brGris = new SolidBrush(TemaApp.Gris);

            for (int i = 0; i < niveles.Count; i++)
            {
                byte niv = niveles[i];
                int cx = left + slotW * i + slotW / 2;
                int totalGrupoW = _grupos.Count * barW + (_grupos.Count - 1) * gap;
                int startX = cx - totalGrupoW / 2;

                for (int gi = 0; gi < _grupos.Count; gi++)
                {
                    string grp = _grupos[gi];
                    var reg = _datos.FirstOrDefault(d => d.Nivel == niv && d.Grupo == grp);
                    double pct = reg?.TasaAcierto ?? 0;
                    int h = Math.Max(4, (int)Math.Round(chartH * Math.Clamp(pct, 0, 100) / 100.0));
                    int bx = startX + gi * (barW + gap);
                    int by = bottom - h;

                    Color col = _paleta[gi % _paleta.Length];
                    using (var brBar = new SolidBrush(col))
                        g.FillRoundedRectangle(brBar, new Rectangle(bx, by, barW, h), new Size(6, 6));

                    string txtPct = $"{Math.Round(pct)}%";
                    var szP = g.MeasureString(txtPct, fVal);
                    using (var brCol = new SolidBrush(col))
                        g.DrawString(txtPct, fVal, brCol, bx + (barW - szP.Width) / 2, by - 16);
                }

                string nomNiv = NombresNiveles.TryGetValue(niv, out var label) ? label : $"Nivell {niv}";
                var szN = g.MeasureString(nomNiv, fNiv);
                g.DrawString(nomNiv, fNiv, brGris, cx - szN.Width / 2, bottom + 8);
            }

            using var fLeg = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            int legItemW = 74;
            int legStartX = (Width - _grupos.Count * legItemW) / 2;
            int legY = Height - 24;

            for (int gi = 0; gi < _grupos.Count; gi++)
            {
                Color col = _paleta[gi % _paleta.Length];
                int lx = legStartX + gi * legItemW;
                using (var brDot = new SolidBrush(col))
                    g.FillEllipse(brDot, lx, legY + 4, 8, 8);
                using (var brTxt = new SolidBrush(TemaApp.Gris))
                    g.DrawString(_grupos[gi], fLeg, brTxt, lx + 14, legY);
            }
        }
    }
}