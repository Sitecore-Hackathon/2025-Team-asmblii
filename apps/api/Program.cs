using api.Models;
using api.Repositories.Solr;
using SolrNet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSolrNet<SolrSearchResult>("http://localhost:8983/solr/test");  //localhost to be replaced by?
builder.Services.AddScoped<ISolrRepository, SolrRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/solrsearch", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.Search(query, start, rows);
    return results;
}).WithName("SolrSearch");

app.MapGet("/solrping", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.Ping();
    return results;
}).WithName("solrping");

app.Run();

