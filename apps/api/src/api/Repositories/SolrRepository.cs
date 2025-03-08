using api.Models;
using SolrNet;

namespace api.Repositories;

/// <summary>
/// Interface for the Solr repository. Supporting dependency injection.
/// </summary>
public interface ISolrService
{
    Task<ResponseHeader> PopulateIndex(int numberofDocs);

}

public class SolrService : ISolrService
{
    private readonly ISolrOperations<SolrSearchResultEntry> _solr;
    private static Random random = new Random();

    public SolrService(ISolrOperations<SolrSearchResultEntry> solr)
    {
        _solr = solr;
    }

    public async Task<ResponseHeader> PopulateIndex(int numberOfDocs)
    {
        for(int count = 0; count < numberOfDocs; count++)
        {
            var doc = new SolrSearchResultEntry
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Title " + RandomString(random.Next(10, 50)),
                Content = "Content " + RandomString(random.Next(50,2000)),
                Url = "https://asmblii.com",
                Author = "Author " + RandomString(random.Next(5, 15)),
                Date = DateTime.Now,
                Tags = new List<string> { "tag1", "tag2" },
                Categories = new List<string> { "category1", "category2" }
            };
            await _solr.AddAsync(doc);
        }
        await _solr.CommitAsync();
        await _solr.OptimizeAsync();
        return new ResponseHeader
        {
            Status = 200,
            QTime = 0,
            Params = new Dictionary<string, string> { 
                { "number_of_docs added", numberOfDocs.ToString() } 
            }
        };  
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ?!-";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
