using api.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace api.Repositories.Solr;

public interface ISolrRepository
{
    Task<ICollection<SolrSearchResult>> Search(string query, int start, int rows);
    Task<ResponseHeader> Ping();
}

public class SolrRepository : ISolrRepository
{
    private readonly ISolrOperations<SolrSearchResult> _solr;
    public SolrRepository(ISolrOperations<SolrSearchResult> solr)
    {
        _solr = solr;
    }
    public async Task<ICollection<SolrSearchResult>> Search(string query, int start, int rows)
    {
        var results = await _solr.QueryAsync(new SolrQuery(query), options: new QueryOptions
        {
            StartOrCursor = new StartOrCursor.Start(start),
            Rows = rows
        });
        return results;
    }

    public async Task<ResponseHeader> Ping()
    {
        var results = await _solr.PingAsync();
        return results;
    }
}
