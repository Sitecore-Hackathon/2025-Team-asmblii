using api.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace api.Repositories;

/// <summary>
/// Interface for the Solr repository. Supporting dependency injection.
/// </summary>
public interface ISolrRepository
{
    Task<ICollection<SolrSearchResultEntry>>Search(string query, int start, int rows);
    Task<ResponseHeader> Ping();
}

/// <summary>
/// Repository for Solr search operations.
/// </summary>
public class SolrRepository : ISolrRepository
{
    private readonly ISolrOperations<SolrSearchResultEntry> _solr;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolrRepository"/> class.
    /// </summary>
    /// <param name="solr">
    /// Solr operations to use.
    /// </param>
    public SolrRepository(ISolrOperations<SolrSearchResultEntry> solr)
    {
        _solr = solr;
    }

    /// <summary>
    /// Searches the Solr server for the specified query.
    /// </summary>
    /// <param name="query">
    /// query to search for.
    /// </param>
    /// <param name="start">
    /// start index of the search results.
    /// </param>
    /// <param name="rows">
    /// rows to return.
    /// </param>
    /// <returns></returns>
    public async Task<ICollection<SolrSearchResultEntry>> Search(string query, int start, int rows)
    {
        var results = await _solr.QueryAsync(new SolrQuery(query), options: new QueryOptions
        {
            StartOrCursor = new StartOrCursor.Start(start),
            Rows = rows
        });
        return results;
    }

    /// <summary>
    /// Pings the Solr server to check if it is up and running.
    /// </summary>
    /// <returns>
    /// ResponseHeader object with the status of the ping. Including properties like status, QTime, and params.
    /// </returns>
    public async Task<ResponseHeader> Ping()
    {
        var results = await _solr.PingAsync();
        return results;
    }
}
