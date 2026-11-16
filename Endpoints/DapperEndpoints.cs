using Dapper;
using MySqlConnector;

namespace api_net_8_hibrida.Endpoints;

public static class DapperEndpoints
{
    public static void MapDapperEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dapper/{dbName}/{tableName}", async (
            string dbName,
            string tableName,
            HttpContext httpContext,
            IConfiguration config,
            IWebHostEnvironment env,
            int page = 1,
            int pageSize = 50) =>
        {
            var apiKey = httpContext.Request.Headers["X-Api-Key"].ToString();

            var expectedApiKey = env.IsDevelopment()
                ? config["Security:ApiKeyTest"]
                : config["Security:ApiKeyLive"];

            if (string.IsNullOrEmpty(apiKey) || apiKey != expectedApiKey)
                return Results.Unauthorized();

            var allowedTables = new[] { "usuarios", "processos", "movimentos", "assuntos" };
            if (!allowedTables.Contains(tableName.ToLower()))
                return Results.StatusCode(403);

            var connectionString = config.GetConnectionString(dbName);
            if (string.IsNullOrEmpty(connectionString))
                return Results.NotFound($"Banco de dados '{dbName}' não configurado.");

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            var safeTableName = tableName.Replace("`", "").Replace(";", "");
            var sql = $"SELECT * FROM `{safeTableName}` LIMIT @PageSize OFFSET @Offset";

            using var connection = new MySqlConnection(connectionString);

            var parameters = new { PageSize = pageSize, Offset = (page - 1) * pageSize };
            var result = await connection.QueryAsync(sql, parameters);

            return Results.Ok(result);
        });
    }
}