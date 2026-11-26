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
using AppEmocions.Helpers;
using AppEmocions.Models.Infants;
using AppEmocions.Models.Usuaris;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public sealed class UcInfants : UserControl, IModuloVista
{
    private DateTime _desde = DateTime.Today.AddDays(-30);
    private DateTime _hasta = DateTime.Today;
    private string? _grupoFiltroGlobal;

    private Guna2TextBox txtBuscar = null!;
    private Guna2ComboBox cmbGrupoLocal = null!;
    private Guna2ToggleSwitch tglActivos = null!;
    private Guna2Panel pnlSubAlta = null!;

    private Guna2TextBox txtNuevoNombre = null!;
    private Guna2ComboBox cmbNuevoGrupo = null!;
    private Guna2ComboBox cmbNuevoDocente = null!;

    private Guna2DataGridView dgvInfants = null!;
    private Label lblPaginacionInfo = null!;
    private FlowLayoutPanel flpPaginas = null!;

    private List<InfantListadoDto> _todos = new();
    private List<InfantListadoDto> _paginaActual = new();
    private List<DocenteDto> _docentes = new();
    private int _pagina = 1;
    private const int TamPagina = 8;

    public event Action<string, bool>? EstadoCambiado;
    public event Action<string>? InfantSeleccionado;
    public event Action? NuevoInfantSolicitado;

    public UcInfants()
    {
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        var marco = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 16, 32, 24), BackColor = TemaApp.Fondo };

        var barraFiltros = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = TemaApp.Fondo };

        txtBuscar = new Guna2TextBox
        {
            PlaceholderText = "Cercar per codi...",
            Size = new Size(250, 40),
            Location = new Point(0, 6),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            FillColor = Color.White,
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 9.5f)
        };
        txtBuscar.TextChanged += async (s, e) => { _pagina = 1; await CargarDatosAsync(CancellationToken.None); };

        cmbGrupoLocal = new Guna2ComboBox
        {
            Size = new Size(155, 40),
            Location = new Point(264, 6),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            FillColor = Color.White,
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 9f),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbGrupoLocal.Items.AddRange(new object[] { "Tots els grups", "P4-A", "P4-B", "P5-A", "P5-B" });
        cmbGrupoLocal.SelectedIndex = 0;
        cmbGrupoLocal.SelectedIndexChanged += async (s, e) => { _pagina = 1; await CargarDatosAsync(CancellationToken.None); };

        tglActivos = new Guna2ToggleSwitch
        {
            Checked = true,
            Location = new Point(436, 15),
            Size = new Size(40, 22)
        };
        tglActivos.CheckedState.FillColor = TemaApp.Turquesa;
        tglActivos.CheckedChanged += async (s, e) => { _pagina = 1; await CargarDatosAsync(CancellationToken.None); };

        var lblActivos = new Label
        {
            Text = "Nom\u00e9s actius",
            Location = new Point(484, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        };

        var btnExportar = new Guna2Button
        {
            Text = "\u2193  Exportar",
            Size = new Size(124, 40),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            FillColor = Color.White,
            ForeColor = TemaApp.Texto,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnExportar.Click += (s, e) => ExportarCsv();

        var btnToggleAlta = new Guna2Button
        {
            Text = "+  Nou infant",
            Size = new Size(132, 40),
            BorderRadius = 10,
            FillColor = TemaApp.Turquesa,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnToggleAlta.Click += (s, e) => NuevoInfantSolicitado?.Invoke();

        barraFiltros.Controls.Add(txtBuscar);
        barraFiltros.Controls.Add(cmbGrupoLocal);
        barraFiltros.Controls.Add(tglActivos);
        barraFiltros.Controls.Add(lblActivos);
        barraFiltros.Controls.Add(btnExportar);
        barraFiltros.Controls.Add(btnToggleAlta);
        barraFiltros.Resize += (s, e) =>
        {
            btnExportar.Location = new Point(barraFiltros.Width - btnExportar.Width, 6);
            btnToggleAlta.Location = new Point(btnExportar.Left - btnToggleAlta.Width - 10, 6);
        };

        pnlSubAlta = new Guna2Panel
        {
            Dock = DockStyle.Top,
            Height = 76,
            Visible = false,
            FillColor = Color.White,
            BorderRadius = 14,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(18, 16, 18, 16)
        };

        txtNuevoNombre = new Guna2TextBox
        {
            PlaceholderText = "Nom o inicials de l'infant...",
            Location = new Point(18, 18),
            Size = new Size(250, 38),
            BorderRadius = 9,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave
        };

        cmbNuevoGrupo = new Guna2ComboBox
        {
            Location = new Point(280, 18),
            Size = new Size(130, 38),
            BorderRadius = 9,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbNuevoGrupo.Items.AddRange(new object[] { "P4-A", "P4-B", "P5-A", "P5-B" });
        cmbNuevoGrupo.SelectedIndex = 0;

        cmbNuevoDocente = new Guna2ComboBox
        {
            Location = new Point(422, 18),
            Size = new Size(220, 38),
            BorderRadius = 9,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        var btnGuardar = new Guna2Button
        {
            Text = "Crear infant",
            Location = new Point(656, 18),
            Size = new Size(125, 38),
            BorderRadius = 9,
            FillColor = TemaApp.Turquesa,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnGuardar.Click += async (s, e) => await CrearInfantAsync();

        var btnCerrarAlta = new Guna2Button
        {
            Text = "Cancel\u00b7lar",
            Location = new Point(790, 18),
            Size = new Size(106, 38),
            BorderRadius = 9,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            FillColor = Color.White,
            ForeColor = TemaApp.Gris,
            Cursor = Cursors.Hand
        };
        btnCerrarAlta.Click += (s, e) => pnlSubAlta.Visible = false;

        pnlSubAlta.Controls.Add(txtNuevoNombre);
        pnlSubAlta.Controls.Add(cmbNuevoGrupo);
        pnlSubAlta.Controls.Add(cmbNuevoDocente);
        pnlSubAlta.Controls.Add(btnGuardar);
        pnlSubAlta.Controls.Add(btnCerrarAlta);

        var cardTabla = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(0, 4, 0, 10)
        };

        cardTabla.ShadowDecoration.Enabled = false; cardTabla.BorderColor = Color.FromArgb(238, 240, 244);
        cardTabla.ShadowDecoration.Depth = 8;
        cardTabla.ShadowDecoration.Color = Color.FromArgb(18, 15, 23, 42);
        cardTabla.Padding = new Padding(12, 8, 12, 10);

        dgvInfants = new Guna2DataGridView
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
            Cursor = Cursors.Hand,
            RowTemplate = { Height = 58 }
        };
        dgvInfants.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
        dgvInfants.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        dgvInfants.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

        dgvInfants.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvInfants.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvInfants.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
        dgvInfants.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(100, 116, 139);
        dgvInfants.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvInfants.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        dgvInfants.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(248, 250, 252);
        dgvInfants.ThemeStyle.HeaderStyle.ForeColor = Color.FromArgb(100, 116, 139);
        dgvInfants.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        dgvInfants.ThemeStyle.HeaderStyle.Height = 44;
        dgvInfants.ThemeStyle.RowsStyle.BackColor = Color.White;
        dgvInfants.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
        dgvInfants.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvInfants.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(30, 41, 59);
        dgvInfants.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(241, 248, 247);
        dgvInfants.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
        dgvInfants.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

        int hoveredRow = -1;
        dgvInfants.CellMouseEnter += (s, e) =>
        {
            if (e.RowIndex >= 0 && e.RowIndex != hoveredRow)
            {
                int prev = hoveredRow;
                hoveredRow = e.RowIndex;
                if (prev >= 0 && prev < dgvInfants.Rows.Count) dgvInfants.InvalidateRow(prev);
                dgvInfants.InvalidateRow(hoveredRow);
            }
        };
        dgvInfants.MouseLeave += (s, e) =>
        {
            if (hoveredRow >= 0 && hoveredRow < dgvInfants.Rows.Count)
            {
                int prev = hoveredRow;
                hoveredRow = -1;
                dgvInfants.InvalidateRow(prev);
            }
        };
        dgvInfants.RowPrePaint += (s, e) =>
        {
            if (e.RowIndex == hoveredRow && !dgvInfants.Rows[e.RowIndex].Selected)
                dgvInfants.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            else if (!dgvInfants.Rows[e.RowIndex].Selected)
                dgvInfants.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
        };

        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Infant", HeaderText = "INFANT", FillWeight = 16 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grup", HeaderText = "GRUP", FillWeight = 10 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sessions", HeaderText = "SESSIONS", FillWeight = 10 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiesActius", HeaderText = "DIES ACTIUS", FillWeight = 11 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "UltimaSessio", HeaderText = "\u00daLTIMA SESSI\u00d3", FillWeight = 14 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "TaxaEncert", HeaderText = "TAXA D'ENCERT", FillWeight = 16 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "TempsResposta", HeaderText = "TEMPS RESPOSTA", FillWeight = 12 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Aband", HeaderText = "ABAND.", FillWeight = 8 });
        dgvInfants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Accion", HeaderText = "", FillWeight = 4 });

        dgvInfants.RowPostPaint += (s, e) =>
{
    using var pen = new Pen(Color.FromArgb(241, 245, 249), 1);
    e.Graphics.DrawLine(pen, e.RowBounds.Left + 8, e.RowBounds.Bottom - 1, e.RowBounds.Right - 8, e.RowBounds.Bottom - 1);
};
        dgvInfants.CellPainting += DgvInfants_CellPainting;
        dgvInfants.CellClick += DgvInfants_CellClick;

        var piePag = new Panel { Dock = DockStyle.Bottom, Height = 52, Padding = new Padding(24, 10, 24, 6), BackColor = Color.White };
        lblPaginacionInfo = new Label
        {
            Text = "Mostrant 0 de 0 infants",
            Location = new Point(24, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 9f),
            ForeColor = TemaApp.Gris
        };
        flpPaginas = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            BackColor = Color.White
        };
        piePag.Controls.Add(lblPaginacionInfo);
        piePag.Controls.Add(flpPaginas);
        piePag.Resize += (s, e) => flpPaginas.Location = new Point(piePag.Width - flpPaginas.Width - 24, 10);

        cardTabla.Controls.Add(dgvInfants);
        cardTabla.Controls.Add(piePag);

        marco.Controls.Add(cardTabla);
        marco.Controls.Add(pnlSubAlta);
        marco.Controls.Add(barraFiltros);
        Controls.Add(marco);
    }

    private void DgvInfants_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _paginaActual.Count) return;
        InfantSeleccionado?.Invoke(_paginaActual[e.RowIndex].Codigo);
    }

    private void DgvInfants_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _paginaActual.Count) return;
        var item = _paginaActual[e.RowIndex];

        if (e.ColumnIndex == 0)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            int d = 34;
            int y = e.CellBounds.Y + (e.CellBounds.Height - d) / 2;
            int x = e.CellBounds.X + 12;
            using (var br = new SolidBrush(Color.FromArgb(224, 243, 241)))
                e.Graphics.FillEllipse(br, x, y, d, d);

            string ini = item.Codigo.Length >= 2 ? item.Codigo[..2].ToUpperInvariant() : "IN";
            using (var fIni = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var brTxt = new SolidBrush(TemaApp.Turquesa))
            {
                var sz = e.Graphics.MeasureString(ini, fIni);
                e.Graphics.DrawString(ini, fIni, brTxt, x + (d - sz.Width) / 2, y + (d - sz.Height) / 2);
            }

            using (var fCod = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (var brCod = new SolidBrush(TemaApp.Texto))
                e.Graphics.DrawString(item.Codigo, fCod, brCod, x + d + 12, e.CellBounds.Y + (e.CellBounds.Height - fCod.Height) / 2);

            e.Handled = true;
        }
        else if (e.ColumnIndex == 1)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            bool esB = item.Grupo.EndsWith("B", StringComparison.OrdinalIgnoreCase);
            Color bg = esB ? Color.FromArgb(236, 232, 246) : Color.FromArgb(224, 243, 241);
            Color fg = esB ? Color.FromArgb(132, 98, 188) : TemaApp.Turquesa;

            var rect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + (e.CellBounds.Height - 24) / 2, 50, 24);
            using (var brBg = new SolidBrush(bg))
                e.Graphics.FillRoundedRectangle(brBg, rect, new Size(7, 7));
            using (var fGrp = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brFg = new SolidBrush(fg))
            {
                var sz = e.Graphics.MeasureString(item.Grupo, fGrp);
                e.Graphics.DrawString(item.Grupo, fGrp, brFg, rect.X + (rect.Width - sz.Width) / 2, rect.Y + (rect.Height - sz.Height) / 2);
            }
            e.Handled = true;
        }
        else if (e.ColumnIndex == 5)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            int barW = 68;
            int barH = 7;
            int bx = e.CellBounds.X + 6;
            int by = e.CellBounds.Y + (e.CellBounds.Height - barH) / 2;
            using (var brBg = new SolidBrush(Color.FromArgb(232, 236, 240)))
                e.Graphics.FillRoundedRectangle(brBg, new Rectangle(bx, by, barW, barH), new Size(3, 3));

            int fillW = (int)Math.Round(barW * Math.Clamp(item.TaxaEncert, 0, 100) / 100.0);
            if (fillW > 0)
            {
                using (var brFg = new SolidBrush(TemaApp.Turquesa))
                    e.Graphics.FillRoundedRectangle(brFg, new Rectangle(bx, by, Math.Max(4, fillW), barH), new Size(3, 3));
            }

            using (var fPct = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var brTxt = new SolidBrush(TemaApp.Texto))
                e.Graphics.DrawString($"{Math.Round(item.TaxaEncert)}%", fPct, brTxt, bx + barW + 10, e.CellBounds.Y + (e.CellBounds.Height - fPct.Height) / 2);

            e.Handled = true;
        }
        else if (e.ColumnIndex == 7)
        {
            e.PaintBackground(e.CellBounds, true);

            e.Graphics!.SmoothingMode = SmoothingMode.AntiAlias;
            int d = 24;
            int bx = e.CellBounds.X + 8;
            int by = e.CellBounds.Y + (e.CellBounds.Height - d) / 2;
            Color bg = item.Abandonaments >= 3 ? Color.FromArgb(253, 243, 224) : Color.FromArgb(241, 244, 248);
            Color fg = item.Abandonaments >= 3 ? Color.FromArgb(196, 138, 42) : TemaApp.Gris;
            using (var br = new SolidBrush(bg))
                e.Graphics.FillEllipse(br, bx, by, d, d);
            string txt = item.Abandonaments.ToString();
            using (var f = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (var brTxt = new SolidBrush(fg))
            {
                var sz = e.Graphics.MeasureString(txt, f);
                e.Graphics.DrawString(txt, f, brTxt, bx + (d - sz.Width) / 2, by + (d - sz.Height) / 2);
            }
            e.Handled = true;
        }
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct)
    {
        _desde = desde;
        _hasta = hasta;
        _grupoFiltroGlobal = grupo;
        await CargarDocentesAsync(ct);
        await CargarDatosAsync(ct);
    }

    private async Task CargarDocentesAsync(CancellationToken ct)
    {
        if (_docentes.Count > 0) return;
        try
        {
            _docentes = await ApiClient.ListarDocentesAsync(ct);
            cmbNuevoDocente.Items.Clear();
            cmbNuevoDocente.Items.Add("Sense docent assignat");
            foreach (var d in _docentes)
                cmbNuevoDocente.Items.Add($"{d.DocenteId} - {d.NombreVisible}");
            cmbNuevoDocente.SelectedIndex = 0;
        }
        catch { }
    }

    private async Task CargarDatosAsync(CancellationToken ct)
    {
        try
        {
            string? grupoEfectivo = cmbGrupoLocal.SelectedIndex > 0
                ? cmbGrupoLocal.SelectedItem?.ToString()
                : _grupoFiltroGlobal;

            _todos = await ApiClient.ListarInfantsAsync(
                _desde, _hasta, grupoEfectivo, txtBuscar.Text, tglActivos.Checked, ct);

            RenderizarPagina();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error carregant infants: " + ex.Message, true);
        }
    }

    private void RenderizarPagina()
    {
        int total = _todos.Count;
        int totalPags = Math.Max(1, (int)Math.Ceiling(total / (double)TamPagina));
        _pagina = Math.Clamp(_pagina, 1, totalPags);

        int skip = (_pagina - 1) * TamPagina;
        _paginaActual = _todos.Skip(skip).Take(TamPagina).ToList();

        dgvInfants.Rows.Clear();
        var cult = new CultureInfo("ca-ES");
        foreach (var inf in _paginaActual)
        {
            string ult = inf.UltimaSessio.HasValue ? inf.UltimaSessio.Value.ToString("d MMM yyyy", cult) : "-";
            dgvInfants.Rows.Add(
                inf.Codigo,
                inf.Grupo,
                inf.Sessions,
                inf.DiesActius,
                ult,
                $"{Math.Round(inf.TaxaEncert)}%",
                $"{(inf.TempsRespostaMs / 1000.0):0.0} s",
                inf.Abandonaments,
                "\u203a"
            );
        }

        int desdeIdx = total == 0 ? 0 : skip + 1;
        int hastaIdx = skip + _paginaActual.Count;
        lblPaginacionInfo.Text = $"Mostrant {desdeIdx}\u2013{hastaIdx} de {total} infants";

        flpPaginas.Controls.Clear();
        flpPaginas.Controls.Add(CrearBotonPagina("Anterior", _pagina > 1, false, () => { _pagina--; RenderizarPagina(); }));
        for (int p = 1; p <= totalPags; p++)
        {
            int num = p;
            flpPaginas.Controls.Add(CrearBotonPagina(num.ToString(), true, num == _pagina, () => { _pagina = num; RenderizarPagina(); }));
        }
        flpPaginas.Controls.Add(CrearBotonPagina("Seg\u00fcent", _pagina < totalPags, false, () => { _pagina++; RenderizarPagina(); }));
    }

    private static Guna2Button CrearBotonPagina(string texto, bool habilitado, bool activo, Action onClick)
    {
        var btn = new Guna2Button
        {
            Text = texto,
            AutoSize = true,
            Height = 32,
            MinimumSize = new Size(34, 32),
            BorderRadius = 8,
            BorderThickness = 1,
            BorderColor = activo ? TemaApp.AzulMenu : TemaApp.BordeSuave,
            FillColor = activo ? TemaApp.AzulMenu : Color.White,
            ForeColor = activo ? Color.White : (habilitado ? TemaApp.Texto : TemaApp.Gris),
            Font = new Font("Segoe UI", 8.5f, activo ? FontStyle.Bold : FontStyle.Regular),
            Enabled = habilitado,
            Cursor = habilitado ? Cursors.Hand : Cursors.Default,
            Margin = new Padding(3, 0, 3, 0)
        };
        if (habilitado) btn.Click += (s, e) => onClick();
        return btn;
    }

    private async Task CrearInfantAsync()
    {
        string nom = txtNuevoNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nom))
        {
            EstadoCambiado?.Invoke("Introdueix el nom de l'infant.", true);
            return;
        }

        int? docId = null;
        if (cmbNuevoDocente.SelectedIndex > 0)
        {
            var sel = cmbNuevoDocente.SelectedItem?.ToString() ?? "";
            var partes = sel.Split('-');
            if (partes.Length > 0 && int.TryParse(partes[0].Trim(), out int id))
                docId = id;
        }

        try
        {
            var res = await ApiClient.CrearInfantAsync(new CrearInfantRequest(nom, cmbNuevoGrupo.Text, docId));
            txtNuevoNombre.Clear();
            pnlSubAlta.Visible = false;
            EstadoCambiado?.Invoke($"Infant creat: {res.Codigo}", false);
            await CargarDatosAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error creant infant: " + ex.Message, true);
        }
    }

    private void ExportarCsv()
    {
        using var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "Llista_Infants.csv" };
        if (sfd.ShowDialog() != DialogResult.OK) return;

        var sb = new StringBuilder();
        sb.AppendLine("Codi;Grup;Sessions;DiesActius;UltimaSessio;TaxaEncert;TempsRespostaMs;Abandonaments");
        foreach (var i in _todos)
            sb.AppendLine($"{i.Codigo};{i.Grupo};{i.Sessions};{i.DiesActius};{i.UltimaSessio:yyyy-MM-dd};{i.TaxaEncert:0.0};{i.TempsRespostaMs:0};{i.Abandonaments}");

        File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
        EstadoCambiado?.Invoke($"Exportat: {Path.GetFileName(sfd.FileName)}", false);
    }
}