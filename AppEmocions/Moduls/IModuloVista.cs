namespace AppEmocions.Moduls;

public interface IModuloVista
{
    Task ActualizarFiltrosAsync(DateTime desde, DateTime hasta, string? grupo, CancellationToken ct = default);
}
