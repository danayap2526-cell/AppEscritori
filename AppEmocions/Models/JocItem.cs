namespace AppEmocions.Models
{
    public enum TipoIcono
    {
        Corazon,
        Sol,
        Estrella
    }

    public class JocItem
    {
        public string Nom { get; set; } = "";
        public string Identificador { get; set; } = "";
        public string Descripcio { get; set; } = "";
        public bool Actiu { get; set; }
        public TipoIcono Icono { get; set; } = TipoIcono.Estrella;
    }
}