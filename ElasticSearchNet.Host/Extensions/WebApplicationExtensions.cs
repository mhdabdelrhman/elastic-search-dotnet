using ElasticSearchNet.Host.Models;
using ElasticSearchNet.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchNet.Host.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {

        app.MapPost("users/create-index/{indexName}", async (string indexName, IElasticService elasticService) =>
        {
            await elasticService.CreateIndexIfNotExistsAsync(indexName);

            return Results.Ok($"Index {indexName} created or already exists.");
        });

        app.MapPost("users/add-user", async ([FromBody] User user, IElasticService elasticService) =>
        {
            var isAdded = await elasticService.AddOrUpdate(user);

            return isAdded ? Results.Ok($"User added or updated successfully.") : Results.Problem("Error adding or updating user.");
        });

        app.MapPut("users/update-user", async ([FromBody] User user, IElasticService elasticService) =>
        {
            var isAdded = await elasticService.AddOrUpdate(user);

            return isAdded ? Results.Ok($"User added or updated successfully.") : Results.Problem("Error adding or updating user.");
        });

        app.MapGet("users/get-user/{userId}", async (int userId, IElasticService elasticService) =>
        {
            var user = await elasticService.Get(userId);

            return user != null ? Results.Json(user) : Results.NotFound("User not found.");
        });

        app.MapGet("users/get-all-users", async (IElasticService elasticService) =>
        {
            var users = await elasticService.GetAll();

            return Results.Json(users);
        });

        app.MapDelete("users/delete-user/{userId}", async (int userId, IElasticService elasticService) =>
        {
            var removed = await elasticService.Remove(userId);

            return removed ? Results.Ok() : Results.NotFound("User not found.");
        });

        app.MapDelete("users/delete-all-users", async (IElasticService elasticService) =>
        {
            var removed = await elasticService.RemoveAll();

            return removed > 0 ? Results.Ok() : Results.NotFound("No users found.");
        });

        return app;
    }
}
