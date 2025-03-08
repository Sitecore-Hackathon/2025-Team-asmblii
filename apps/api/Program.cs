using api.Models;
using api.Repositories;
using api.Services;
using SolrNet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/******** OpenApi ********/
builder.Services.AddOpenApi();


/******** Solr ********/
builder.Services.AddSolrNet<SolrSearchResultEntry>("http://localhost:8983/solr/test");  //localhost to be replaced by?
builder.Services.AddScoped<ISolrRepository, SolrRepository>();


/******** DadJokes ********/
builder.Services.AddHttpClient<IDadJokeService, DadJokeService>(client =>
{
    client.BaseAddress = new Uri("https://icanhazdadjoke.com/");
    //ICanHazDadJoke Kindly request that contact details in form of a website or email is supplied to requests to their free API. See https://icanhazdadjoke.com/api#custom-user-agent for more info.
    client.DefaultRequestHeaders.UserAgent.ParseAdd($"asmblii: https://github.com/Sitecore-Hackathon/2025-Team-asmblii - https://asmblii.com");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/******** SOLR methods ********/
app.MapGet("/solrsearch", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.Search(query, start, rows);
    return results;
}).WithName("SolrSearch");

app.MapGet("/solrping", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.Ping();
    return results;
}).WithName("SolrPing");


/******** IHazDadJokes methods ********/
app.MapGet("/dadjoke", async (IDadJokeService dadJokeService, string id) =>
{
    var results = await dadJokeService.GetJokeAsync(id);
    return results;
}).WithName("DadJoke");

app.MapGet("/randomdadjoke", async (IDadJokeService dadJokeService) =>
{
    var results = await dadJokeService.GetRandomJokeAsync();
    return results;
}).WithName("RandomDadJoke");


//initiate the app
app.Run();


