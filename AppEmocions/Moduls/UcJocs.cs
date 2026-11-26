using AppEmocions.Helpers;
using AppEmocions.Models.Jocs;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public sealed class UcJocs : UserControl, IModuloVista
{
    private int? _juegoEditandoId;
    private Guna2DataGridView dgvJocs = null!; private Label lblFormTitulo = null!;
    private Guna2TextBox txtNombre = null!, txtDescripcion = null!; private Guna2CheckBox chkActivo = null!;
    private Guna2Button btnGuardar = null!, btnNuevo = null!, btnAlternarEstado = null!;
    public event Action<string, bool>? EstadoCambiado;
    public UcJocs() { Dock = DockStyle.Fill; BackColor = TemaApp.Fondo; ConstruirInterfaz(); }
    private void ConstruirInterfaz()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(24, 16, 24, 20), BackColor = TemaApp.Fondo };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62f)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
        var panelLista = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 14, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(18), Margin = new Padding(0, 0, 12, 0) };
        var lblTitulo = new Label { Text = "Catàleg de jocs educatius", Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        dgvJocs = new Guna2DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
        dgvJocs.ThemeStyle.HeaderStyle.BackColor = TemaApp.Fondo; dgvJocs.ThemeStyle.HeaderStyle.ForeColor = TemaApp.Gris; dgvJocs.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold); dgvJocs.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvJocs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 });
        dgvJocs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nom del joc", Width = 170 });
        dgvJocs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Descripcion", HeaderText = "Descripció", Width = 240 });
        dgvJocs.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estat", Width = 85 });
        dgvJocs.SelectionChanged += (s, e) => { if (dgvJocs.SelectedRows.Count > 0 && dgvJocs.SelectedRows[0].Tag is JuegoDto j) { _juegoEditandoId = j.JuegoId; lblFormTitulo.Text = $"Editant joc #{j.JuegoId}"; txtNombre.Text = j.Nombre; txtDescripcion.Text = j.Descripcion ?? ""; chkActivo.Checked = j.Activo; btnAlternarEstado.Enabled = true; } };
        panelLista.Controls.Add(dgvJocs); panelLista.Controls.Add(lblTitulo);
        var panelForm = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 14, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(20) };
        lblFormTitulo = new Label { Text = "Nou joc", Dock = DockStyle.Top, Height = 32, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        txtNombre = new Guna2TextBox { PlaceholderText = "Nom del joc...", Dock = DockStyle.Top, Height = 40, BorderRadius = 8, Margin = new Padding(0, 8, 0, 12) };
        txtDescripcion = new Guna2TextBox { PlaceholderText = "Descripció pedagògica...", Dock = DockStyle.Top, Height = 100, Multiline = true, BorderRadius = 8, Margin = new Padding(0, 12, 0, 12) };
        chkActivo = new Guna2CheckBox { Text = "Joc actiu disponible", Checked = true, Dock = DockStyle.Top, Height = 34, ForeColor = TemaApp.Texto };
        btnGuardar = new Guna2Button { Text = "Desar joc", Dock = DockStyle.Top, Height = 42, BorderRadius = 8, FillColor = TemaApp.Turquesa, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        btnGuardar.Click += async (s, e) => await GuardarJuegoAsync();
        btnNuevo = new Guna2Button { Text = "Netejar / Nou", Dock = DockStyle.Top, Height = 38, BorderRadius = 8, FillColor = TemaApp.Fondo, ForeColor = TemaApp.Texto };
        btnNuevo.Click += (s, e) => { _juegoEditandoId = null; lblFormTitulo.Text = "Nou joc"; txtNombre.Clear(); txtDescripcion.Clear(); chkActivo.Checked = true; btnAlternarEstado.Enabled = false; dgvJocs.ClearSelection(); };
        btnAlternarEstado = new Guna2Button { Text = "Activar / Desactivar", Dock = DockStyle.Top, Height = 38, BorderRadius = 8, FillColor = TemaApp.Texto, ForeColor = Color.White, Enabled = false };
        btnAlternarEstado.Click += async (s, e) => { if (dgvJocs.SelectedRows.Count > 0 && dgvJocs.SelectedRows[0].Tag is JuegoDto j) { await ApiClient.CambiarEstadoJocAsync(j.JuegoId, !j.Activo); await CargarJuegosAsync(CancellationToken.None); } };
        panelForm.Controls.Add(btnAlternarEstado); panelForm.Controls.Add(btnNuevo); panelForm.Controls.Add(btnGuardar); panelForm.Controls.Add(chkActivo); panelForm.Controls.Add(txtDescripcion); panelForm.Controls.Add(txtNombre); panelForm.Controls.Add(lblFormTitulo);
        layout.Controls.Add(panelLista, 0, 0); layout.Controls.Add(panelForm, 1, 0); Controls.Add(layout);
    }
    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct) => await CargarJuegosAsync(ct);
    private async Task CargarJuegosAsync(CancellationToken ct) { try { var list = await ApiClient.ListarJocsAsync(ct); dgvJocs.Rows.Clear(); foreach (var j in list) { int idx = dgvJocs.Rows.Add(j.JuegoId, j.Nombre, j.Descripcion ?? "", j.Activo ? "Actiu" : "Inactiu"); dgvJocs.Rows[idx].Tag = j; } } catch (OperationCanceledException) { } catch (Exception ex) { EstadoCambiado?.Invoke("Error jocs: " + ex.Message, true); } }
    private async Task GuardarJuegoAsync() { if (string.IsNullOrWhiteSpace(txtNombre.Text)) return; var req = new GuardarJuegoRequest(txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), chkActivo.Checked); try { if (_juegoEditandoId is null) await ApiClient.CrearJocAsync(req); else await ApiClient.ActualizarJocAsync(_juegoEditandoId.Value, req); EstadoCambiado?.Invoke("Joc desat.", false); await CargarJuegosAsync(CancellationToken.None); } catch (Exception ex) { EstadoCambiado?.Invoke("Error: " + ex.Message, true); } }
}
