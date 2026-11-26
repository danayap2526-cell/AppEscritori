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

public sealed class UcAbandonament : UserControl, IModuloVista
{
    private Label lblSubtituloCont = null!;
    private GraficoContinuitatHorizontal graficoContinuitat = null!;
    private Guna2Panel cardTabla = null!;
    private Guna2DataGridView dgvInterrupcions = null!;
    private TableLayoutPanel stack = null!;
    private Panel scroll = null!;

    private List<AbandonoNivelGrupoDto> _datos = new();

    private static readonly Dictionary<byte, string> NombresNiveles = new()
    {
        [1] = "1 \u00b7 La Pla\u00e7a",
        [2] = "2 \u00b7 El Parc",
        [3] = "3 \u00b7 La Pla\u00e7a Gran",
        [4] = "4 \u00b7 El Jard\u00ed de la Calma"
    };

    public event Action<string, bool>? EstadoCambiado;

    public UcAbandonament()
    {
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(32, 14, 32, 28),
            BackColor = TemaApp.Fondo
        };

        stack = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = TemaApp.Fondo
        };
        stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        // 1. Banner informativo superior
        var cardInfo = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 72,
            FillColor = Color.FromArgb(238, 248, 246),
            BorderRadius = 14,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(196, 230, 224),
            Margin = new Padding(0, 0, 0, 18)
        };

        var pnlIcono = new Panel
        {
            Location = new Point(20, 24),
            Size = new Size(22, 22),
            BackColor = Color.Transparent
        };
        pnlIcono.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(TemaApp.Turquesa, 1.6f);
            e.Graphics.DrawEllipse(pen, 1, 1, 19, 19);
            using var f = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var br = new SolidBrush(TemaApp.Turquesa);
            var sz = e.Graphics.MeasureString("i", f);
            e.Graphics.DrawString("i", f, br, (22 - sz.Width) / 2, (22 - sz.Height) / 2);
        };

        cardInfo.Controls.Add(pnlIcono);
        cardInfo.Controls.Add(new Label
        {
            Text = "Com llegir aquestes dades",
            Location = new Point(54, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = TemaApp.Turquesa
        });
        cardInfo.Controls.Add(new Label
        {
            Text = "Una sortida pot ser una pausa, un canvi d'activitat o una interrupci\u00f3 de la tauleta.",
            Location = new Point(54, 37),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(88, 114, 110)
        });

        // 2. Tarjeta central: Continuitat per nivell
        var cardGrafico = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 320,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(238, 240, 244),
            Padding = new Padding(24, 20, 24, 20),
            Margin = new Padding(0, 0, 0, 18)
        };

        var cabGraf = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.White };
        cabGraf.Controls.Add(new Label
        {
            Text = "Continu\u00eftat per nivell",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        lblSubtituloCont = new Label
        {
            Text = "0 sessions iniciades",
            Location = new Point(0, 24),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        };
        cabGraf.Controls.Add(lblSubtituloCont);

        var pnlLeyenda = new Panel
        {
            Size = new Size(300, 24),
            BackColor = Color.White
        };
        AgregarItemLeyenda(pnlLeyenda, 0, Color.FromArgb(247, 198, 68), "Descansar");
        AgregarItemLeyenda(pnlLeyenda, 102, Color.FromArgb(226, 138, 91), "Sortida");
        AgregarItemLeyenda(pnlLeyenda, 190, Color.FromArgb(139, 100, 200), "Segon pla");

        cabGraf.Controls.Add(pnlLeyenda);
        cabGraf.Resize += (s, e) => pnlLeyenda.Location = new Point(cabGraf.Width - pnlLeyenda.Width, 32);

        graficoContinuitat = new GraficoContinuitatHorizontal
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        cardGrafico.Controls.Add(graficoContinuitat);
        cardGrafico.Controls.Add(cabGraf);

        // 3. Tarjeta inferior: Detall d'interrupcions
        cardTabla = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 360,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = Color.FromArgb(238, 240, 244),
            Padding = new Padding(24, 20, 24, 16)
        };

        var cabTabla = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White };
        cabTabla.Controls.Add(new Label
        {
            Text = "Detall d'interrupcions",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        cabTabla.Controls.Add(new Label
        {
            Text = "Per grup i nivell",
            Location = new Point(0, 24),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        });

        dgvInterrupcions = new Guna2DataGridView
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
            ScrollBars = ScrollBars.Vertical,
            RowTemplate = { Height = 54 }
        };
        dgvInterrupcions.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvInterrupcions.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

        dgvInterrupcions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvInterrupcions.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvInterrupcions.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
        dgvInterrupcions.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 116, 139);
        dgvInterrupcions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvInterrupcions.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvInterrupcions.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvInterrupcions.ThemeStyle.HeaderStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvInterrupcions.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvInterrupcions.ThemeStyle.HeaderStyle.Height = 42;
        dgvInterrupcions.ThemeStyle.RowsStyle.BackColor = Color.White;
        dgvInterrupcions.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
        dgvInterrupcions.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvInterrupcions.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(30, 41, 59);
        dgvInterrupcions.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(244, 249, 248);
        dgvInterrupcions.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        dgvInterrupcions.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nivell", HeaderText = "NIVELL", FillWeight = 22 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grup", HeaderText = "GRUP", FillWeight = 13 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Iniciades", HeaderText = "INICIADES", FillWeight = 13 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Completades", HeaderText = "COMPLETADES", FillWeight = 14 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descansar", HeaderText = "DESCANSAR", FillWeight = 13 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sortida", HeaderText = "SORTIDA", FillWeight = 12 });
        dgvInterrupcions.Columns.Add(new DataGridViewTextBoxColumn { Name = "SegonPla", HeaderText = "SEGON PLA", FillWeight = 13 });

        dgvInterrupcions.RowPostPaint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(241, 245, 249), 1);
            e.Graphics.DrawLine(pen, e.RowBounds.Left + 8, e.RowBounds.Bottom - 1, e.RowBounds.Right - 8, e.RowBounds.Bottom - 1);
        };
        dgvInterrupcions.CellPainting += DgvInterrupcions_CellPainting;

        cardTabla.Controls.Add(dgvInterrupcions);
        cardTabla.Controls.Add(cabTabla);

        stack.Controls.Add(cardInfo, 0, 0);
        stack.Controls.Add(cardGrafico, 0, 1);
        stack.Controls.Add(cardTabla, 0, 2);

        scroll.Controls.Add(stack);
        stack.SizeChanged += (s, e) => scroll.AutoScrollMinSize = new Size(0, stack.Height + 48);

        Controls.Add(scroll);
    }

    private static void AgregarItemLeyenda(Panel parent, int x, Color color, string texto)
    {
        var cuadro = new Guna2Panel
        {
            Location = new Point(x, 6),
            Size = new Size(10, 10),
            FillColor = color,
            BorderRadius = 3
        };
        var lbl = new Label
        {
            Text = texto,
            Location = new Point(x + 15, 3),
            AutoSize = true,
            Font = new Font("Segoe UI", 8f),
            ForeColor = TemaApp.Gris
        };
        parent.Controls.Add(cuadro);
        parent.Controls.Add(lbl);
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct)
    {
        try
        {
            _datos = await ApiClient.AbandonoPorNivelAsync(desde, hasta, grupo, ct);

            int totalIni = _datos.Sum(d => d.Iniciadas);
            var cult = new CultureInfo("ca-ES");
            lblSubtituloCont.Text = $"{totalIni} sessions iniciades \u00b7 {desde.ToString("MMMM yyyy", cult)}";

            var resumenNiveles = _datos
                .GroupBy(d => d.Nivel)
                .OrderBy(g => g.Key)
                .Select(g => new NivelContinuitatItem(
                    g.Key,
                    NombresNiveles.TryGetValue(g.Key, out var n) ? n : $"Nivell {g.Key}",
                    g.Sum(x => x.Iniciadas),
                    g.Sum(x => x.Completadas),
                    g.Sum(x => x.Descansar),
                    g.Sum(x => x.Sortida),
                    g.Sum(x => x.SegonPla)
                ))
                .ToList();

            graficoContinuitat.EstablecerDatos(resumenNiveles);
            RenderizarTabla();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error carregant abandonament: " + ex.Message, true);
        }
    }

    private void RenderizarTabla()
    {
        dgvInterrupcions.Rows.Clear();
        var ordenados = _datos.OrderBy(d => d.Nivel).ThenBy(d => d.Grupo).ToList();

        int filas = Math.Max(1, ordenados.Count);
        cardTabla.Height = 136 + filas * 54;
        scroll.AutoScrollMinSize = new Size(0, stack.Height + 48);

        foreach (var d in ordenados)
        {
            string nomNiv = NombresNiveles.TryGetValue(d.Nivel, out var label) ? label : $"Nivell {d.Nivel}";
            dgvInterrupcions.Rows.Add(
                nomNiv,
                d.Grupo,
                d.Iniciadas,
                d.Completadas,
                d.Descansar,
                d.Sortida,
                d.SegonPla
            );
        }
    }

    private void DgvInterrupcions_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0) return;

        if (e.ColumnIndex == 0)
        {
            e.PaintBackground(e.CellBounds, true);
            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            string val = e.Value?.ToString() ?? "";
            using var f = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var br = new SolidBrush(TemaApp.Texto);
            e.Graphics.DrawString(val, f, br, e.CellBounds.X + 8, e.CellBounds.Y + (e.CellBounds.Height - f.Height) / 2);
            e.Handled = true;
        }
        else if (e.ColumnIndex == 1)
        {
            e.PaintBackground(e.CellBounds, true);
            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            string grp = e.Value?.ToString() ?? "";
            bool esB = grp.EndsWith("B", StringComparison.OrdinalIgnoreCase);

            Color bg = esB ? Color.FromArgb(236, 232, 246) : Color.FromArgb(224, 243, 241);
            Color fg = esB ? Color.FromArgb(132, 98, 188) : TemaApp.Turquesa;

            var rect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 50, 24);
            using (var brBg = new SolidBrush(bg))
                e.Graphics.FillRoundedRectangle(brBg, rect, new Size(7, 7));

            using (var f = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brFg = new SolidBrush(fg))
            {
                var sz = e.Graphics.MeasureString(grp, f);
                e.Graphics.DrawString(grp, f, brFg, rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2);
            }
            e.Handled = true;
        }
    }

    private sealed record NivelContinuitatItem(
        byte Nivel,
        string Etiqueta,
        int Iniciadas,
        int Completadas,
        int Descansar,
        int Sortida,
        int SegonPla);

    private sealed class GraficoContinuitatHorizontal : Panel
    {
        private List<NivelContinuitatItem> _items = new();

        private static readonly Color ColCompletada = Color.FromArgb(46, 178, 166);
        private static readonly Color ColDescansar = Color.FromArgb(247, 198, 68);
        private static readonly Color ColSortida = Color.FromArgb(226, 138, 91);
        private static readonly Color ColSegonPla = Color.FromArgb(139, 100, 200);
        private static readonly Color ColPista = Color.FromArgb(241, 245, 249);

        public GraficoContinuitatHorizontal()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        public void EstablecerDatos(List<NivelContinuitatItem> items)
        {
            _items = items;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_items.Count == 0)
            {
                using var f = new Font("Segoe UI", 9.5f);
                using var br = new SolidBrush(TemaApp.Gris);
                const string msg = "Sense dades d'abandonament per al per\u00edode seleccionat";
                var sz = g.MeasureString(msg, f);
                g.DrawString(msg, f, br, (Width - sz.Width) / 2, (Height - sz.Height) / 2);
                return;
            }

            int labelColW = 195;
            int pctColW = 68;
            int barLeft = labelColW;
            int barRight = Width - pctColW - 12;
            int barMaxW = Math.Max(80, barRight - barLeft);

            int topPad = 14;
            int bottomPad = 14;
            int rowH = Math.Max(38, (Height - topPad - bottomPad) / _items.Count);
            int barH = 22;

            using var fLabel = new Font("Segoe UI", 9f);
            using var fPct = new Font("Segoe UI", 10f, FontStyle.Bold);
            using var brTexto = new SolidBrush(TemaApp.Texto);
            using var brGris = new SolidBrush(Color.FromArgb(71, 85, 105));

            for (int i = 0; i < _items.Count; i++)
            {
                var it = _items[i];
                int cy = topPad + i * rowH + rowH / 2;
                int by = cy - barH / 2;

                var szL = g.MeasureString(it.Etiqueta, fLabel);
                g.DrawString(it.Etiqueta, fLabel, brGris, 0, cy - szL.Height / 2);

                int tot = Math.Max(1, it.Iniciadas);
                double pctCont = it.Iniciadas > 0 ? it.Completadas * 100.0 / it.Iniciadas : 0;

                // Fondo pista redondeado
                using (var brPista = new SolidBrush(ColPista))
                    g.FillRoundedRectangle(brPista, new Rectangle(barLeft, by, barMaxW, barH), new Size(6, 6));

                // Segmentos apilados
                int wComp = (int)Math.Round(barMaxW * (it.Completadas / (double)tot));
                int wDesc = (int)Math.Round(barMaxW * (it.Descansar / (double)tot));
                int wSort = (int)Math.Round(barMaxW * (it.Sortida / (double)tot));
                int wSeg = (int)Math.Round(barMaxW * (it.SegonPla / (double)tot));

                int curX = barLeft;
                if (wComp > 0)
                {
                    using var br = new SolidBrush(ColCompletada);
                    g.FillRoundedRectangle(br, new Rectangle(curX, by, Math.Min(barMaxW, wComp), barH), new Size(6, 6));
                    curX += wComp;
                }
                if (wDesc > 0 && curX < barLeft + barMaxW)
                {
                    int w = Math.Min(barLeft + barMaxW - curX, Math.Max(4, wDesc));
                    using var br = new SolidBrush(ColDescansar);
                    g.FillRectangle(br, curX, by, w, barH);
                    curX += w;
                }
                if (wSort > 0 && curX < barLeft + barMaxW)
                {
                    int w = Math.Min(barLeft + barMaxW - curX, Math.Max(4, wSort));
                    using var br = new SolidBrush(ColSortida);
                    g.FillRectangle(br, curX, by, w, barH);
                    curX += w;
                }
                if (wSeg > 0 && curX < barLeft + barMaxW)
                {
                    int w = Math.Min(barLeft + barMaxW - curX, Math.Max(4, wSeg));
                    using var br = new SolidBrush(ColSegonPla);
                    g.FillRoundedRectangle(br, new Rectangle(curX, by, w, barH), new Size(5, 5));
                }

                string txtPct = $"{Math.Round(pctCont)}%";
                var szP = g.MeasureString(txtPct, fPct);
                g.DrawString(txtPct, fPct, brTexto, Width - szP.Width - 6, cy - szP.Height / 2);
            }
        }
    }
}