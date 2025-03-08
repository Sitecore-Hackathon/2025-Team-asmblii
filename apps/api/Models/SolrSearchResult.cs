using SolrNet.Attributes;

namespace  api.Models;

public class SolrSearchResult
{
    [SolrUniqueKey("id")]
    public string Id { get; set; } = string.Empty;

    [SolrField("title")]
    public string Title { get; set; } = string.Empty;

    [SolrField("content")]
    public string Content { get; set; } = string.Empty;

    [SolrField("url")]
    public string Url { get; set; } = string.Empty;

    [SolrField("author")]
    public string Author { get; set; } = string.Empty;

    [SolrField("date")]
    public DateTime Date { get; set; }

    [SolrField("tags")]
    public ICollection<string> Tags { get; set; } = new List<string>();

    [SolrField("categories")]
    public ICollection<string> Categories { get; set; } = new List<string>();

    [SolrField("type")]
    public string Type { get; set; } = string.Empty;

    [SolrField("score")]
    public float Score { get; set; }

    public SolrSearchResult()
    {
    }
}