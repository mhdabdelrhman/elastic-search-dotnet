
using Newtonsoft.Json;

namespace ElasticSearchNet.Host.Models;

public class User
{
    [JsonProperty("Id")]
    public int Id { get; set; }

    [JsonProperty("FirstName")]
    public string FirstName { get; set; } = default!;

    [JsonProperty("LastName")]
    public string LastName { get; set; } = default!;
}
