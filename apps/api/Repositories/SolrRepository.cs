using api.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace api.Repositories.Solr;

public interface ISolrRepository
{
    Task<ICollection<SolrSearchResult>> Search(string query, int start, int rows);
}

public class SolrService : ISolrRepository
{
    private readonly ISolrOperations<SolrSearchResult> _solr;
    public SolrService(ISolrOperations<SolrSearchResult> solr)
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
}
