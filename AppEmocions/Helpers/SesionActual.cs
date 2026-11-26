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

    public static void Cerrar() => Usuario = null;
}