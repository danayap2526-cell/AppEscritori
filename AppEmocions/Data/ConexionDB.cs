using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AppEmocions.Data
{
    public static class Conexion
    {
        // Se carga la primera vez que se usa, no al iniciar el tipo
        private static readonly Lazy<string> _cadena = new(Cargar);

        private static string Cargar()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Local.json", optional: true)
                .Build();

            string? cadena = config.GetConnectionString("SqlServer");

            if (string.IsNullOrWhiteSpace(cadena) || cadena.Contains("CAMBIAR"))
                throw new InvalidOperationException(
                    "Falta la cadena de conexión en appsettings.json (ConnectionStrings:SqlServer).");

            return cadena;
        }

        // PRIVADO: nadie fuera de esta clase abre conexiones
        private static SqlConnection Crear() => new SqlConnection(_cadena.Value);

        /// <summary>Abre la conexión y comprueba que responde. Llamar una vez al arrancar.</summary>
        public static bool Probar(out string mensaje)
        {
            try
            {
                using var cn = Crear();
                cn.Open();

                using var cmd = new SqlCommand("SELECT 1", cn);
                cmd.ExecuteScalar();

                mensaje = $"Conectado a {cn.Database} en {cn.DataSource} (SQL Server {cn.ServerVersion})";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
                return false;
            }
        }

        public static DataTable Consultar(string sql, params SqlParameter[] parametros)
        {
            using var cn = Crear();
            using var cmd = new SqlCommand(sql, cn);
            if (parametros.Length > 0) cmd.Parameters.AddRange(parametros);

            var tabla = new DataTable();
            using var da = new SqlDataAdapter(cmd);
            da.Fill(tabla); // abre y cierra la conexión por sí solo
            return tabla;
        }

        public static int Ejecutar(string sql, params SqlParameter[] parametros)
        {
            using var cn = Crear();
            using var cmd = new SqlCommand(sql, cn);
            if (parametros.Length > 0) cmd.Parameters.AddRange(parametros);

            cn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object? EjecutarEscalar(string sql, params SqlParameter[] parametros)
        {
            using var cn = Crear();
            using var cmd = new SqlCommand(sql, cn);
            if (parametros.Length > 0) cmd.Parameters.AddRange(parametros);

            cn.Open();
            return cmd.ExecuteScalar();
        }

        /// <summary>
        /// Ejecuta varias operaciones en una sola transacción.
        /// Si algo falla, se hace rollback automáticamente.
        /// </summary>
        public static void EjecutarTransaccion(Action<SqlConnection, SqlTransaction> acciones)
        {
            using var cn = Crear();
            cn.Open();
            using var tx = cn.BeginTransaction();
            try
            {
                acciones(cn, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}