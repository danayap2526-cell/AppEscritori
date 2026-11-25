using System.Data;
using Microsoft.Data.SqlClient;

namespace AppEmocions.Api.Data;

public sealed class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:SqlServer en appsettings.json.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}