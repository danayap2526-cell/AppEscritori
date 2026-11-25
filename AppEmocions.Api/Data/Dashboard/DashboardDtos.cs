namespace AppEmocions.Api.Data.Dashboard;

public sealed record FiltroDashboard(DateTime Desde, DateTime HastaExclusivo, string? Grupo);

public sealed class ResumenDashboardDto
{
    public int ParticipantesActivos { get; set; }
    public int Sesiones { get; set; }
    public int SesionesCompletadas { get; set; }
    public int SesionesAbandonadas { get; set; }
    public double PuntuacionMedia { get; set; }
    public double RespMediaMs { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
}

public sealed class SemanaEvolucionDto
{
    public int Semana { get; set; }
    public string? Grupo { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
    public double TasaAcierto { get; set; }
}

public sealed class NivelEstadisticaDto
{
    public byte Nivel { get; set; }
    public int Aciertos { get; set; }
    public int Errores { get; set; }
    public int Registros { get; set; }
    public int Completados { get; set; }
}

public sealed class MetricaAtencionRawDto
{
    public string Codigo { get; set; } = string.Empty;
    public int Interrumpidas { get; set; }
    public double? RespMs { get; set; }
    public int AcRec { get; set; }
    public int TotRec { get; set; }
    public int AcAnt { get; set; }
    public int TotAnt { get; set; }
}

public sealed record NinoAtencionDto(string Codigo, string Motivo, string Estado);

public sealed class ComparativaGrupoDto
{
    public string Grupo { get; set; } = string.Empty;
    public int TotalInfants { get; set; }
    public int Sesiones { get; set; }
    public double TasaAcierto { get; set; }
    public double RespMediaMs { get; set; }
    public double TasaAbandono { get; set; }
    public double TasaFinalizacion { get; set; }
}

public sealed class ComparativaNivelGrupoDto
{
    public string Grupo { get; set; } = string.Empty;
    public byte Nivel { get; set; }
    public double TasaAcierto { get; set; }
}

public sealed class AbandonoNivelGrupoDto
{
    public byte Nivel { get; set; }
    public string Grupo { get; set; } = string.Empty;
    public int Iniciadas { get; set; }
    public int Completadas { get; set; }
    public int Descansar { get; set; }
    public int Sortida { get; set; }
    public int SegonPla { get; set; }
}