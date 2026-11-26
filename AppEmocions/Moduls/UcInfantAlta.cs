using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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

public sealed class UcInfantAlta : UserControl, IModuloVista
{
    private Guna2TextBox txtNombre = null!;
    private Guna2TextBox txtContrasenaAlta = null!;
    private Guna2ComboBox cmbGrupoAlta = null!;
    private Guna2ComboBox cmbDocenteAlta = null!;

    private Guna2ToggleSwitch tglSoloSinAsignar = null!;
    private Guna2TextBox txtBuscarPend = null!;
    private FlowLayoutPanel flpPendientes = null!;

    private List<InfantListadoDto> _participantes = new();
    private List<DocenteDto> _docentes = new();
    private DateTime _desde = DateTime.Today.AddYears(-1);
    private DateTime _hasta = DateTime.Today;

    private static readonly string[] GruposDisponibles = ["P4-A", "P4-B", "P5-A", "P5-B"];

    public event Action? VolverSolicitado;
    public event Action<string, bool>? EstadoCambiado;

    public UcInfantAlta()
    {
        Dock = DockStyle.Fill;
        BackColor = TemaApp.Fondo;
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        var marco = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 16, 32, 24), BackColor = TemaApp.Fondo };

        var barraVolver = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = TemaApp.Fondo };
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

        var split = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = TemaApp.Fondo
        };
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38f));
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62f));

        // Columna izquierda: Crear nou participant
        var cardAlta = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(24),
            Margin = new Padding(0, 0, 12, 0)
        };

        cardAlta.Controls.Add(new Label
        {
            Text = "Nou participant",
            Location = new Point(24, 22),
            AutoSize = true,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        cardAlta.Controls.Add(new Label
        {
            Text = "Dona d'alta un infant i assigna el seu grup i docent.",
            Location = new Point(24, 48),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        });

        cardAlta.Controls.Add(new Label { Text = "Nom o inicials de l'infant", Location = new Point(24, 92), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        txtNombre = new Guna2TextBox
        {
            PlaceholderText = "Ex: Mart\u00ed Vila o MV",
            Location = new Point(24, 116),
            Size = new Size(300, 40),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Font = new Font("Segoe UI", 9.5f)
        };

        cardAlta.Controls.Add(new Label { Text = "Grup assignat", Location = new Point(24, 174), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        cmbGrupoAlta = new Guna2ComboBox
        {
            Location = new Point(24, 198),
            Size = new Size(300, 40),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f)
        };
        cmbGrupoAlta.Items.AddRange(GruposDisponibles);
        cmbGrupoAlta.SelectedIndex = 0;

        cardAlta.Controls.Add(new Label { Text = "Docent responsable", Location = new Point(24, 256), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        cmbDocenteAlta = new Guna2ComboBox
        {
            Location = new Point(24, 280),
            Size = new Size(300, 40),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5f)
        };

        cardAlta.Controls.Add(new Label { Text = "Contrasenya app mÃ²bil (opcional)", Location = new Point(24, 338), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold), ForeColor = TemaApp.Texto });
        txtContrasenaAlta = new Guna2TextBox
        {
            PlaceholderText = "Ex: 1234 (deixa en blanc si no en tÃ©)",
            Location = new Point(24, 362),
            Size = new Size(300, 40),
            BorderRadius = 10,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Font = new Font("Segoe UI", 9.5f),
            UseSystemPasswordChar = true
        };

        var btnCrear = new Guna2Button
        {
            Text = "Crear participant",
            Location = new Point(24, 424),
            Size = new Size(300, 42),
            BorderRadius = 10,
            FillColor = TemaApp.Turquesa,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnCrear.Click += async (s, e) => await CrearNuevoAsync();

        cardAlta.Controls.Add(txtNombre);
        cardAlta.Controls.Add(cmbGrupoAlta);
        cardAlta.Controls.Add(cmbDocenteAlta);
        cardAlta.Controls.Add(txtContrasenaAlta);
        cardAlta.Controls.Add(btnCrear);
        cardAlta.Resize += (s, e) =>
        {
            int w = Math.Max(180, cardAlta.Width - 48);
            txtNombre.Width = w;
            cmbGrupoAlta.Width = w;
            cmbDocenteAlta.Width = w;
            txtContrasenaAlta.Width = w;
            btnCrear.Width = w;
        };

        // Columna derecha: Assignacio de participants (app mobil / pendents)
        var cardAsignar = new Guna2Panel
        {
            Dock = DockStyle.Fill,
            FillColor = Color.White,
            BorderRadius = 16,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Padding = new Padding(24),
            Margin = new Padding(12, 0, 0, 0)
        };

        var cabAsig = new Panel { Dock = DockStyle.Top, Height = 96, BackColor = Color.White };
        cabAsig.Controls.Add(new Label
        {
            Text = "Assignaci\u00f3 de grup i docent",
            Location = new Point(0, 0),
            AutoSize = true,
            Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        });
        cabAsig.Controls.Add(new Label
        {
            Text = "Participants registrats des de l'app m\u00f2bil sense docent o grup assignat.",
            Location = new Point(0, 26),
            AutoSize = true,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = TemaApp.Gris
        });

        txtBuscarPend = new Guna2TextBox
        {
            PlaceholderText = "Cercar per codi o nom...",
            Location = new Point(0, 52),
            Size = new Size(230, 36),
            BorderRadius = 9,
            BorderThickness = 1,
            BorderColor = TemaApp.BordeSuave,
            Font = new Font("Segoe UI", 9f)
        };
        txtBuscarPend.TextChanged += (s, e) => RenderizarListaPendientes();

        tglSoloSinAsignar = new Guna2ToggleSwitch
        {
            Checked = true,
            Location = new Point(246, 60),
            Size = new Size(38, 20)
        };
        tglSoloSinAsignar.CheckedState.FillColor = TemaApp.Turquesa;
        tglSoloSinAsignar.CheckedChanged += (s, e) => RenderizarListaPendientes();

        var lblSoloSin = new Label
        {
            Text = "Nom\u00e9s pendents d'assignar",
            Location = new Point(292, 61),
            AutoSize = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = TemaApp.Texto
        };

        cabAsig.Controls.Add(txtBuscarPend);
        cabAsig.Controls.Add(tglSoloSinAsignar);
        cabAsig.Controls.Add(lblSoloSin);

        flpPendientes = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Color.White
        };
        flpPendientes.Resize += (s, e) =>
        {
            foreach (Control c in flpPendientes.Controls)
                c.Width = Math.Max(360, flpPendientes.ClientSize.Width - 10);
        };

        cardAsignar.Controls.Add(flpPendientes);
        cardAsignar.Controls.Add(cabAsig);

        split.Controls.Add(cardAlta, 0, 0);
        split.Controls.Add(cardAsignar, 1, 0);

        marco.Controls.Add(split);
        marco.Controls.Add(barraVolver);
        Controls.Add(marco);
    }

    public async Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct)
    {
        _desde = desde;
        _hasta = hasta;
        await CargarDatosAsync(ct);
    }

    private async Task CargarDatosAsync(CancellationToken ct)
    {
        try
        {
            var tDoc = ApiClient.ListarDocentesAsync(ct);
            var tInf = ApiClient.ListarInfantsAsync(_desde, _hasta, null, null, false, ct);
            await Task.WhenAll(tDoc, tInf);

            _docentes = tDoc.Result;
            _participantes = tInf.Result;

            cmbDocenteAlta.Items.Clear();
            cmbDocenteAlta.Items.Add("Sense docent assignat");
            foreach (var d in _docentes)
                cmbDocenteAlta.Items.Add($"{d.DocenteId} - {d.NombreVisible}");
            cmbDocenteAlta.SelectedIndex = _docentes.Count > 0 ? 1 : 0;

            RenderizarListaPendientes();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error carregant participants: " + ex.Message, true);
        }
    }

    private void RenderizarListaPendientes()
    {
        flpPendientes.Controls.Clear();
        string q = txtBuscarPend.Text.Trim();

        var filtrados = _participantes.Where(p =>
        {
            bool sinAsignar = p.DocenteId is null || string.IsNullOrWhiteSpace(p.Grupo) || p.Grupo == "SIN_ASIGNAR";
            if (tglSoloSinAsignar.Checked && !sinAsignar) return false;
            if (!string.IsNullOrEmpty(q))
            {
                bool matchCod = p.Codigo.Contains(q, StringComparison.OrdinalIgnoreCase);
                bool matchNom = !string.IsNullOrWhiteSpace(p.Nombre) && p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase);
                if (!matchCod && !matchNom) return false;
            }
            return true;
        }).ToList();

        if (filtrados.Count == 0)
        {
            flpPendientes.Controls.Add(new Label
            {
                Text = tglSoloSinAsignar.Checked
                    ? "Tots els participants tenen grup i docent assignats."
                    : "No s'han trobat participants amb aquest criteri.",
                AutoSize = true,
                Padding = new Padding(4, 24, 4, 12),
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = TemaApp.Gris
            });
            return;
        }

        int anchoFila = Math.Max(420, flpPendientes.ClientSize.Width - 10);

        foreach (var inf in filtrados)
        {
            var fila = new Guna2Panel
            {
                Size = new Size(anchoFila, 62),
                FillColor = Color.FromArgb(249, 250, 252),
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = TemaApp.BordeSuave,
                Margin = new Padding(0, 0, 0, 8)
            };

            string titulo = string.IsNullOrWhiteSpace(inf.Nombre) ? inf.Codigo : $"{inf.Codigo} \u00b7 {inf.Nombre}";
            var lblNom = new Label
            {
                Text = titulo,
                Location = new Point(14, 11),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = TemaApp.Texto
            };

            string estadoAct = inf.DocenteId is null ? "Sense docent" : $"Docent: {inf.NombreDocente}";
            var lblSub = new Label
            {
                Text = $"{estadoAct}  \u00b7  Grup actual: {(string.IsNullOrWhiteSpace(inf.Grupo) ? "Sense grup" : inf.Grupo)}",
                Location = new Point(14, 33),
                AutoSize = true,
                Font = new Font("Segoe UI", 8f),
                ForeColor = inf.DocenteId is null ? Color.FromArgb(196, 138, 42) : TemaApp.Gris
            };

            var cmbGrp = new Guna2ComboBox
            {
                Size = new Size(92, 34),
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = TemaApp.BordeSuave,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5f)
            };
            cmbGrp.Items.AddRange(GruposDisponibles);
            int idxGrp = Array.IndexOf(GruposDisponibles, inf.Grupo);
            cmbGrp.SelectedIndex = idxGrp >= 0 ? idxGrp : 0;

            var cmbDoc = new Guna2ComboBox
            {
                Size = new Size(165, 34),
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = TemaApp.BordeSuave,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5f)
            };
            cmbDoc.Items.Add("Sense docent");
            int selDocIdx = 0;
            for (int i = 0; i < _docentes.Count; i++)
            {
                cmbDoc.Items.Add($"{_docentes[i].DocenteId} - {_docentes[i].NombreVisible}");
                if (inf.DocenteId == _docentes[i].DocenteId)
                    selDocIdx = i + 1;
            }
            cmbDoc.SelectedIndex = selDocIdx;

            var btnAsig = new Guna2Button
            {
                Text = "Assignar",
                Size = new Size(88, 34),
                BorderRadius = 8,
                FillColor = TemaApp.Turquesa,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            int partId = inf.ParticipanteId;
            string codPart = inf.Codigo;
            btnAsig.Click += async (s, e) =>
            {
                int? docId = null;
                if (cmbDoc.SelectedIndex > 0)
                {
                    var partes = (cmbDoc.SelectedItem?.ToString() ?? "").Split('-');
                    if (partes.Length > 0 && int.TryParse(partes[0].Trim(), out int id))
                        docId = id;
                }
                string grpSel = cmbGrp.Text;

                try
                {
                    btnAsig.Enabled = false;
                    await ApiClient.AsignarDocenteInfantAsync(partId, docId, grpSel);
                    EstadoCambiado?.Invoke($"Assignat {codPart} al grup {grpSel}", false);
                    await CargarDatosAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    btnAsig.Enabled = true;
                    EstadoCambiado?.Invoke("Error assignant participant: " + ex.Message, true);
                }
            };

            fila.Controls.Add(lblNom);
            fila.Controls.Add(lblSub);
            fila.Controls.Add(cmbGrp);
            fila.Controls.Add(cmbDoc);
            fila.Controls.Add(btnAsig);

            void PosicionarControles()
            {
                btnAsig.Location = new Point(fila.Width - btnAsig.Width - 12, 14);
                cmbDoc.Location = new Point(btnAsig.Left - cmbDoc.Width - 8, 14);
                cmbGrp.Location = new Point(cmbDoc.Left - cmbGrp.Width - 8, 14);
            }
            PosicionarControles();
            fila.Resize += (s, e) => PosicionarControles();

            flpPendientes.Controls.Add(fila);
        }
    }

    private async Task CrearNuevoAsync()
    {
        string nom = txtNombre.Text.Trim();
        if (string.IsNullOrWhiteSpace(nom))
        {
            EstadoCambiado?.Invoke("Introdueix el nom de l'infant.", true);
            return;
        }

        int? docId = null;
        if (cmbDocenteAlta.SelectedIndex > 0)
        {
            var partes = (cmbDocenteAlta.SelectedItem?.ToString() ?? "").Split('-');
            if (partes.Length > 0 && int.TryParse(partes[0].Trim(), out int id))
                docId = id;
        }

        string clave = txtContrasenaAlta.Text.Trim();
        string? hashMovil = string.IsNullOrEmpty(clave)
            ? null
            : Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(nom + clave))).ToLowerInvariant();

        try
        {
            var res = await ApiClient.CrearInfantAsync(new CrearInfantRequest(nom, cmbGrupoAlta.Text, docId, hashMovil));
            txtNombre.Clear();
            txtContrasenaAlta.Clear();
            EstadoCambiado?.Invoke($"Participant creat correctament: {res.Codigo}", false);
            await CargarDatosAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            EstadoCambiado?.Invoke("Error creant participant: " + ex.Message, true);
        }
    }
}