using AppEmocions.Helpers;
using AppEmocions.Models.Usuaris;
using AppEmocions.Services;
using Guna.UI2.WinForms;

namespace AppEmocions.Moduls;

public sealed class UcUsuaris : UserControl, IModuloVista
{
    private Guna2DataGridView dgvDocentes = null!;
    private Guna2TextBox txtUsuario = null!, txtNombreVisible = null!, txtEmail = null!, txtContrasena = null!, txtGrupos = null!;
    private Guna2ComboBox cmbRol = null!; private Guna2Button btnCrearDocente = null!, btnAlternarActivo = null!;
    public event Action<string, bool>? EstadoCambiado;
    public UcUsuaris() { Dock = DockStyle.Fill; BackColor = TemaApp.Fondo; ConstruirInterfaz(); }
    private void ConstruirInterfaz()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(24, 16, 24, 20), BackColor = TemaApp.Fondo };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62f)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
        var panelLista = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 14, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(18), Margin = new Padding(0, 0, 12, 0) };
        var lblTitulo = new Label { Text = "Equip docent i usuaris", Dock = DockStyle.Top, Height = 34, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        dgvDocentes = new Guna2DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
        dgvDocentes.ThemeStyle.HeaderStyle.BackColor = TemaApp.Fondo; dgvDocentes.ThemeStyle.HeaderStyle.ForeColor = TemaApp.Gris; dgvDocentes.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold); dgvDocentes.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvDocentes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Usuario", HeaderText = "Usuari", Width = 110 });
        dgvDocentes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nom visible", Width = 155 });
        dgvDocentes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", Width = 95 });
        dgvDocentes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Grupos", HeaderText = "Grups", Width = 120 });
        dgvDocentes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estat", Width = 75 });
        dgvDocentes.SelectionChanged += (s, e) => btnAlternarActivo.Enabled = dgvDocentes.SelectedRows.Count > 0;
        panelLista.Controls.Add(dgvDocentes); panelLista.Controls.Add(lblTitulo);
        var panelForm = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 14, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(20) };
        var lblForm = new Label { Text = "Alta de nou docent", Dock = DockStyle.Top, Height = 32, Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        txtUsuario = new Guna2TextBox { PlaceholderText = "Usuari (login)...", Dock = DockStyle.Top, Height = 38, BorderRadius = 8 };
        txtNombreVisible = new Guna2TextBox { PlaceholderText = "Nom i cognoms...", Dock = DockStyle.Top, Height = 38, BorderRadius = 8 };
        txtEmail = new Guna2TextBox { PlaceholderText = "Email (opcional)...", Dock = DockStyle.Top, Height = 38, BorderRadius = 8 };
        txtContrasena = new Guna2TextBox { PlaceholderText = "Contrasenya (mín. 10 caràcters)...", UseSystemPasswordChar = true, Dock = DockStyle.Top, Height = 38, BorderRadius = 8 };
        cmbRol = new Guna2ComboBox { Dock = DockStyle.Top, Height = 38, BorderRadius = 8 }; cmbRol.Items.AddRange(new object[] { "docent", "admin" }); cmbRol.SelectedIndex = 0;
        txtGrupos = new Guna2TextBox { PlaceholderText = "Grups (ex: P4-A, P4-B)", Text = "P4-A, P4-B", Dock = DockStyle.Top, Height = 38, BorderRadius = 8 };
        btnCrearDocente = new Guna2Button { Text = "+ Crear docent", Dock = DockStyle.Top, Height = 42, BorderRadius = 8, FillColor = TemaApp.Turquesa, ForeColor = Color.White, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        btnCrearDocente.Click += async (s, e) => await CrearDocenteAsync();
        btnAlternarActivo = new Guna2Button { Text = "Activar / Desactivar seleccionat", Dock = DockStyle.Top, Height = 38, BorderRadius = 8, FillColor = TemaApp.Texto, ForeColor = Color.White, Enabled = false };
        btnAlternarActivo.Click += async (s, e) => { if (dgvDocentes.SelectedRows.Count > 0 && dgvDocentes.SelectedRows[0].Tag is DocenteDto d) { await ApiClient.CambiarEstadoDocenteAsync(d.DocenteId, !d.Activo); await CargarDocentesAsync(CancellationToken.None); } };
        panelForm.Controls.Add(btnAlternarActivo); panelForm.Controls.Add(btnCrearDocente); panelForm.Controls.Add(txtGrupos); panelForm.Controls.Add(cmbRol); panelForm.Controls.Add(txtContrasena); panelForm.Controls.Add(txtEmail); panelForm.Controls.Add(txtNombreVisible); panelForm.Controls.Add(txtUsuario); panelForm.Controls.Add(lblForm);
        layout.Controls.Add(panelLista, 0, 0); layout.Controls.Add(panelForm, 1, 0); Controls.Add(layout);
    }
    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct) => await CargarDocentesAsync(ct);
    private async Task CargarDocentesAsync(CancellationToken ct) { try { var list = await ApiClient.ListarDocentesAsync(ct); dgvDocentes.Rows.Clear(); foreach (var d in list) { int idx = dgvDocentes.Rows.Add(d.Usuario, d.NombreVisible, d.Rol, string.Join(", ", d.Grupos), d.Activo ? "Actiu" : "Inactiu"); dgvDocentes.Rows[idx].Tag = d; } } catch (OperationCanceledException) { } catch (Exception ex) { EstadoCambiado?.Invoke("Error docents: " + ex.Message, true); } }
    private async Task CrearDocenteAsync() { if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtNombreVisible.Text) || txtContrasena.Text.Length < 10) { EstadoCambiado?.Invoke("Revisa usuari, nom i contrasenya (mín. 10).", true); return; } var grupos = txtGrupos.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(); try { await ApiClient.CrearDocenteAsync(new CrearDocenteRequest(txtUsuario.Text.Trim(), txtNombreVisible.Text.Trim(), string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(), txtContrasena.Text, cmbRol.SelectedItem?.ToString() ?? "docent", grupos)); txtUsuario.Clear(); txtNombreVisible.Clear(); txtEmail.Clear(); txtContrasena.Clear(); EstadoCambiado?.Invoke("Docent creat.", false); await CargarDocentesAsync(CancellationToken.None); } catch (Exception ex) { EstadoCambiado?.Invoke("Error: " + ex.Message, true); } }
}

