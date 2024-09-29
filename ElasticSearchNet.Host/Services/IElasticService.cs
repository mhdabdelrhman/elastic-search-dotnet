using ElasticSearchNet.Host.Models;

namespace ElasticSearchNet.Host.Services;

public interface IElasticService
{
    /// create index
    Task CreateIndexIfNotExistsAsync(string indexName);

    /// add or update user
    Task<bool> AddOrUpdate(User user);

    /// add or update user bulk
    Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName);

    /// get user by id
    Task<User> Get(int id);

    /// get all users
    Task<List<User>> GetAll();

    /// remove 
    Task<bool> Remove(int id);

    /// remove all
    Task<long> RemoveAll();
}
