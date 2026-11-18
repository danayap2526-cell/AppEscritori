using Microsoft.Data.SqlClient;
using System.Data;

namespace AppEmocions.Api.Data;

public class Db
{
    private readonly string _cadena;

    public Db(IConfiguration config)
    {
        _cadena = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:SqlServer");
    }

    private SqlConnection Crear() => new(_cadena);

    public bool Probar(out string mensaje)
    {
        try
        {
            using var cn = Crear();
            cn.Open();
            mensaje = $"Conectado a {cn.Database}";
            return true;
        }
        catch (Exception ex)
        {
            mensaje = ex.Message;
            return false;
        }
    }

    public DataTable Consultar(string sql, params SqlParameter[] p)
    {
        using var cn = Crear();
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddRange(p);
        var tabla = new DataTable();
        new SqlDataAdapter(cmd).Fill(tabla);
        return tabla;
    }

    public int Ejecutar(string sql, params SqlParameter[] p)
    {
        using var cn = Crear();
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddRange(p);
        cn.Open();
        return cmd.ExecuteNonQuery();
    }
}