using SolrNet.Attributes;

namespace  api.Models;

public class SolrSearchResultEntry
{
    //Selected index fields to expose - with empty object values as fallback - attr_ is used as prefix to hit a dynamic field in Solr schema definition

    /// <summary>
    /// Gets or sets the ID of a search result entry.
    /// </summary>
    [SolrUniqueKey("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of a search result entry.
    /// </summary>
    [SolrField("attr_title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of a search result entry.
    /// </summary>
    [SolrField("attr_content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of a search result entry.
    /// </summary>
    [SolrField("attr_url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of a search result entry.
    /// </summary>
    [SolrField("attr_author")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of a search result entry.
    /// </summary>
    [SolrField("attr_date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the tags of a search result entry.
    /// </summary>
    [SolrField("attr_tags")]
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the categories of a search result entry.
    /// </summary>
    [SolrField("attr_categories")]
    public ICollection<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the type of a search result entry.
    /// </summary>
    [SolrField("attr_type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score of a search result entry.
    /// </summary>
    [SolrField("attr_score")]
    public float Score { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolrSearchResultEntry"/> class.
    /// </summary>
    public SolrSearchResultEntry()
    {
    }
}