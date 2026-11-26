using System;
using System.Collections.Generic;
using AppEmocions.Models.Usuaris;

namespace AppEmocions.Helpers;

public static class SesionActual
{
    public static DocenteDto? Usuario { get; private set; }

    public static bool EstaAutenticado => Usuario is not null;

    public static bool EsAdmin =>
        Usuario is not null && string.Equals(Usuario.Rol, "admin", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<string> GruposPermitidos =>
        Usuario?.Grupos ?? (IReadOnlyList<string>)Array.Empty<string>();

    public static string NombreCompleto =>
        !string.IsNullOrWhiteSpace(Usuario?.NombreVisible) ? Usuario!.NombreVisible.Trim() : "Marta Soler";

    public static string PrimerNombre
    {
        get
        {
            var partes = NombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length > 0 ? partes[0] : "Docent";
        }
    }

    public static string Iniciales
    {
        get
        {
            var partes = NombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 1)
                return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpperInvariant();
            return $"{char.ToUpperInvariant(partes[0][0])}{char.ToUpperInvariant(partes[1][0])}";
        }
    }

    public static string RolVisible =>
        !EstaAutenticado ? "Educador/a" : (EsAdmin ? "Administrador/a" : "Educador/a");

    public static void Iniciar(DocenteDto docente) => Usuario = docente;

    public static void IniciarPorNombreUsuario(string usuarioLogin, DocenteDto? encontradoEnApi = null)
    {
        if (encontradoEnApi is not null)
        {
            Usuario = encontradoEnApi;
            return;
        }

        string limpio = usuarioLogin.Trim();
        var partes = limpio.Split(new[] { '.', '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < partes.Length; i++)
            partes[i] = char.ToUpperInvariant(partes[i][0]) + partes[i].Substring(1).ToLowerInvariant();

        string nombreVisible = partes.Length > 0 ? string.Join(" ", partes) : "Docent";
        bool esAdmin = string.Equals(limpio, "admin", StringComparison.OrdinalIgnoreCase);

        Usuario = new DocenteDto
        {
            Usuario = limpio,
            NombreVisible = esAdmin ? "AdministraciÃ³" : nombreVisible,
            Rol = esAdmin ? "admin" : "docente",
            Activo = true
        };
    }

    public static void Cerrar() => Usuario = null;
}