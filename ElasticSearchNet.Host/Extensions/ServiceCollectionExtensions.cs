using ElasticSearchNet.Host.Models;
using ElasticSearchNet.Host.Services;

namespace ElasticSearchNet.Host.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<ElasticSettings>(configuration.GetSection("ElasticSettings"));
        services.AddSingleton<IElasticService, ElasticService>();

        return services;
    }
}
