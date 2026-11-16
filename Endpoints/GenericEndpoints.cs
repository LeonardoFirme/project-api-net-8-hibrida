using Microsoft.EntityFrameworkCore;
using api_net_8_hibrida.Data;
using api_net_8_hibrida.Repositories;
using api_net_8_hibrida.Models;

namespace api_net_8_hibrida.Endpoints;

public static class GenericEndpoints
{
    public static void MapGenericEndpoints(this WebApplication app)
    {
        app.MapGet("/api/{entityName}", async (
            string entityName,
            string? db,
            int page,
            int pageSize,
            IServiceProvider serviceProvider) =>
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 50;
            if (pageSize > 100) pageSize = 100;

            DbContext context = db?.ToLower() switch
            {
                "sqlserver" => serviceProvider.GetRequiredService<SqlServerDbContext>(),
                _ => serviceProvider.GetRequiredService<MySqlDbContext>()
            };

            var entityType = context.Model.GetEntityTypes()
                .FirstOrDefault(e => e.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))?.ClrType;

            if (entityType == null)
                return Results.NotFound($"Entidade '{entityName}' não encontrada no contexto.");

            var repositoryType = typeof(GenericRepository<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, context);

            var method = repositoryType.GetMethod("GetAllAsync");
            var task = (Task)method!.Invoke(repository, new object[] { page, pageSize })!;

            await task;

            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty!.GetValue(task);

            return Results.Ok(result);
        });
    }
}