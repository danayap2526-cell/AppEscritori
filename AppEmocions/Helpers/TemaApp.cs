using System.Drawing;

namespace AppEmocions.Helpers;

public static class TemaApp
{
    public static readonly Color AzulMenu = Color.FromArgb(32, 45, 73);
    public static readonly Color AzulSeleccionado = Color.FromArgb(53, 69, 101);
    public static readonly Color Amarillo = Color.FromArgb(255, 198, 83);
    public static readonly Color Fondo = Color.FromArgb(248, 247, 244);
    public static readonly Color Turquesa = Color.FromArgb(54, 184, 177);
    public static readonly Color Texto = Color.FromArgb(35, 52, 79);
    public static readonly Color Gris = Color.FromArgb(130, 145, 165);
    public static readonly Color BordeSuave = Color.FromArgb(225, 229, 235);
    public static readonly Color VerdePositivo = Color.FromArgb(35, 145, 133);
    public static readonly Color VerdePositivoFondo = Color.FromArgb(235, 248, 245);
    public static readonly Color NaranjaNegativo = Color.FromArgb(190, 95, 45);
    public static readonly Color NaranjaNegativoFondo = Color.FromArgb(252, 238, 230);
    public static readonly Color RojoError = Color.FromArgb(200, 70, 60);

    private static readonly string[] MesesCortos = { "gen.", "febr.", "març", "abr.", "maig", "juny", "jul.", "ag.", "set.", "oct.", "nov.", "des." };

    public static string FormatearRangoFechas(DateTime ini, DateTime fin)
    {
        string mesIni = MesesCortos[ini.Month - 1];
        string mesFin = MesesCortos[fin.Month - 1];
        if (ini.Date == fin.Date) return $"{ini.Day} {mesIni} {ini.Year}";
        if (ini.Month == fin.Month && ini.Year == fin.Year) return $"{ini.Day}-{fin.Day} {mesIni} {ini.Year}";
        return $"{ini.Day} {mesIni} - {fin.Day} {mesFin} {ini.Year}";
    }
}
