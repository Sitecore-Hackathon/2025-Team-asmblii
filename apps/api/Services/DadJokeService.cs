using api.Models;

namespace api.Services;

/// <summary>
/// Interface for the Dad Joke service. Supporting dependency injection.
/// </summary>
public interface IDadJokeService
{
    Task<DadJoke> GetRandomJokeAsync();
    Task<DadJoke> GetJokeAsync(string id);
}

/// <summary>
/// Service for retrieving Dad Jokes, leveraging a typed HttpClient.
/// </summary>
/// <param name="httpClient">
/// httpClient to use for requests. can be tied to any source - for instance the ICanHazDadJoke service.
/// </param>
public class DadJokeService(HttpClient httpClient) : IDadJokeService
{
    private const string RandomJokeUrl = "/";
    private const string JokeUrl = "/j/{0}";

    /// <summary>
    /// Gets a random joke from specified client
    /// </summary>
    /// <returns>A random joke deserialized</returns>
    public async Task<DadJoke> GetRandomJokeAsync()
    {
        return await httpClient.GetFromJsonAsync<DadJoke>(RandomJokeUrl) ?? throw new Exception("Random joke not available from the desired service!");
    }

    /// <summary>
    /// Gets a Joke from specified client
    /// </summary>
    /// <returns>The specified joke deserialized</returns>
    /// <param name="id">The system id of the Joke at the Joke service for instance ICanHazDadJoke</param>
    public async Task<DadJoke> GetJokeAsync(string id)
    {
        var uri = string.Format(JokeUrl, id);
        return await httpClient.GetFromJsonAsync<DadJoke>(uri) ?? throw new Exception($"Joke with id {id} not available from the desired service!");
    }
}
