using Elastic.Clients.Elasticsearch;
using ElasticSearchNet.Host.Models;
using Microsoft.Extensions.Options;

namespace ElasticSearchNet.Host.Services;

public class ElasticService : IElasticService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSettings _elasticSettings;

    public ElasticService(IOptions<ElasticSettings> options)
    {
        _elasticSettings = options.Value;
        var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
            // .Authentication(new BasicAuthentication(_elasticSettings.Username, _elasticSettings.Password))
            .DefaultIndex(_elasticSettings.DefaultIndex);
        _client = new ElasticsearchClient(settings);
    }

    public async Task CreateIndexIfNotExistsAsync(string indexName)
    {
        var response = await _client.Indices.ExistsAsync(indexName);
        if (!response.Exists)
        {
            await _client.Indices.CreateAsync(indexName);
        }
    }

    public async Task<bool> AddOrUpdate(User user)
    {
        var response = await _client.IndexAsync(user, idx => idx
            .Index(_elasticSettings.DefaultIndex)
            .Id(user.Id)
            .Refresh(Refresh.WaitFor)); // TaskIndexResponse

        return response.IsValidResponse;
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName)
    {
        var response = await _client.BulkAsync(b => b
            .Index(_elasticSettings.DefaultIndex)
            .UpdateMany(users, (ud, u) => ud.Doc(u).DocAsUpsert(true))); // TaskBulkResponse
        return response.IsValidResponse;
    }

    public async Task<User> Get(int id)
    {
        var response = await _client.GetAsync<User>(id.ToString(),
            g => g.Index(_elasticSettings.DefaultIndex)); // TaskGetResponse<User>

        return response.Source ?? default!;
    }

    public async Task<List<User>> GetAll()
    {
        var response = await _client.SearchAsync<User>(s => s
            .Index(_elasticSettings.DefaultIndex)); // TaskSearchResponse<User>

        return response.IsValidResponse ? response.Documents.ToList() : [];
    }

    public async Task<bool> Remove(int id)
    {
        var response = await _client.DeleteAsync<User>(id.ToString(),
            d => d.Index(_elasticSettings.DefaultIndex)); // TaskDeleteResponse

        return response.IsValidResponse;
    }
    public async Task<long> RemoveAll()
    {
        var response = await _client.DeleteByQueryAsync<User>(d => d
            .Indices(Indices.Index(_elasticSettings.DefaultIndex))
            .Query(q => q.MatchAll(new Elastic.Clients.Elasticsearch.QueryDsl.MatchAllQuery())));

        return response.IsValidResponse ? response.Deleted ?? default : default;
    }
}
