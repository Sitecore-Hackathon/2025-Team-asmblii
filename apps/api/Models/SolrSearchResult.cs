using SolrNet.Attributes;

namespace  api.Models;

public class SolrSearchResultEntry
{
    //Selected index fields to expose - with empty object values as fallback

    /// <summary>
    /// Gets or sets the ID of a search result entry.
    /// </summary>
    [SolrUniqueKey("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of a search result entry.
    /// </summary>
    [SolrField("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of a search result entry.
    /// </summary>
    [SolrField("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of a search result entry.
    /// </summary>
    [SolrField("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the author of a search result entry.
    /// </summary>
    [SolrField("author")]
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of a search result entry.
    /// </summary>
    [SolrField("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the tags of a search result entry.
    /// </summary>
    [SolrField("tags")]
    public ICollection<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the categories of a search result entry.
    /// </summary>
    [SolrField("categories")]
    public ICollection<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the type of a search result entry.
    /// </summary>
    [SolrField("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score of a search result entry.
    /// </summary>
    [SolrField("score")]
    public float Score { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolrSearchResultEntry"/> class.
    /// </summary>
    public SolrSearchResultEntry()
    {
    }
}