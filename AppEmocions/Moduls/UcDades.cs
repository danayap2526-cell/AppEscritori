using AppEmocions.Helpers;
using AppEmocions.Services;
using Guna.UI2.WinForms;
using System.Globalization;

namespace AppEmocions.Moduls;

public sealed class UcDades : UserControl, IModuloVista
{
    private Label lblCoherencia = null!; private Guna2CheckBox chkIncluirSimuladas = null!; private Guna2DataGridView dgvSesiones = null!;
    public event Action<string, bool>? EstadoCambiado;
    public UcDades() { Dock = DockStyle.Fill; BackColor = TemaApp.Fondo; ConstruirInterfaz(); }
    private void ConstruirInterfaz()
    {
        var cont = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.White, BorderRadius = 14, BorderThickness = 1, BorderColor = TemaApp.BordeSuave, Padding = new Padding(22) };
        var marco = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 16, 24, 20), BackColor = TemaApp.Fondo };
        var barra = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
        var lbl = new Label { Text = "Registre de sessions i coherència de dades", AutoSize = true, Location = new Point(0, 8), Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = TemaApp.Texto };
        chkIncluirSimuladas = new Guna2CheckBox { Text = "Incloure sessions simulades", AutoSize = true, Location = new Point(430, 12), ForeColor = TemaApp.Texto };
        chkIncluirSimuladas.CheckedChanged += async (s, e) => await CargarDatosAsync(CancellationToken.None);
        barra.Controls.Add(lbl); barra.Controls.Add(chkIncluirSimuladas);
        lblCoherencia = new Label { Dock = DockStyle.Top, Height = 32, Text = "Comprovant integritat...", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = TemaApp.Turquesa };
        dgvSesiones = new Guna2DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
        dgvSesiones.ThemeStyle.HeaderStyle.BackColor = TemaApp.Fondo; dgvSesiones.ThemeStyle.HeaderStyle.ForeColor = TemaApp.Gris; dgvSesiones.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold); dgvSesiones.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9.5f);
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "SesionId", HeaderText = "ID Sessió", Width = 140 });
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "Data", HeaderText = "Data", Width = 130 });
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "Infant", HeaderText = "Infant", Width = 110 });
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "Joc", HeaderText = "Joc", Width = 150 });
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "Origen", HeaderText = "Origen", Width = 95 });
        dgvSesiones.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estat", HeaderText = "Estat", Width = 105 });
        cont.Controls.Add(dgvSesiones); cont.Controls.Add(lblCoherencia); cont.Controls.Add(barra); marco.Controls.Add(cont); Controls.Add(marco);
    }
    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct) => await CargarDatosAsync(ct);
    private async Task CargarDatosAsync(CancellationToken ct) { try { var tS = ApiClient.ListarSesionesDadesAsync(chkIncluirSimuladas.Checked, ct); var tC = ApiClient.ObtenerCoherenciaDatosAsync(ct); await Task.WhenAll(tS, tC); var ses = await tS; var coh = await tC; if (coh is not null) lblCoherencia.Text = $"Total sessions: {coh.TotalSesiones} - Sense incidències: {coh.SesionesSinIncidencias} ({coh.PorcentajeCalidad:0.0}%) - Incidències: {coh.Incidencias.Count}"; dgvSesiones.Rows.Clear(); foreach (var s in ses) dgvSesiones.Rows.Add(s.SesionId, s.Data.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture), s.CodigoInfant, s.Joc, s.Simulada ? "Simulada" : "Real", s.Estat); } catch (OperationCanceledException) { } catch (Exception ex) { EstadoCambiado?.Invoke("Error dades: " + ex.Message, true); } }
}
