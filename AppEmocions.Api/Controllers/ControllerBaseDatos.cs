
using AppEmocions.Api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AppEmocions.Api.Controllers;

public class ControllerBaseDatos : ControllerBase
{
    protected readonly Db _db;

    public ControllerBaseDatos(Db db)
    {
        _db = db;
    }

    protected List<Dictionary<string, object?>> Convertir(DataTable tabla)
    {
        var resultado = new List<Dictionary<string, object?>>();

        foreach (DataRow fila in tabla.Rows)
        {
            var elemento = new Dictionary<string, object?>();

            foreach (DataColumn columna in tabla.Columns)
            {
                elemento[columna.ColumnName] =
                    fila[columna] == DBNull.Value ? null : fila[columna];
            }

            resultado.Add(elemento);
        }

        return resultado;
    }
}
