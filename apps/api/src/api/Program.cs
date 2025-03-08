using api.Models;
using api.Repositories;
using api.Services;
using SolrNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

// Add services to the container.
/******** OpenApi ********/
builder.Services.AddOpenApi();


/******** Solr ********/

var solrUri = builder.Configuration["Solr:Uri"];

builder.Services.AddSolrNet<SolrSearchResultEntry>(solrUri); 
builder.Services.AddScoped<ISolrRepository, SolrRepository>();


/******** DadJokes ********/


builder.Services.AddHttpClient<IDadJokeService, DadJokeService>(client =>
{
    var icanhazdadjokesAddress = builder.Configuration["ICanHazDadJokes:Uri"] ?? throw new Exception("Baseaddress for the joke service is not set!");
    var requestingParty = builder.Configuration["ICanHazDadJokes:RequestingParty"] ?? throw new Exception("If using ICanHazDadJoke they kindly request that contact details in form of a website or email is supplied to requests to their free API. See https://icanhazdadjoke.com/api#custom-user-agent for more info.");

    client.BaseAddress = new Uri(icanhazdadjokesAddress);
    client.DefaultRequestHeaders.UserAgent.ParseAdd(requestingParty);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});


var app = builder.Build();

app.MapHealthChecks("/healthz");

app.MapGet("/test",  () =>
{
    return "solr:uri=" + solrUri;
}).WithName("Test");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

/******** SOLR methods ********/
app.MapPost("/solrsearch", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.Search(query, start, rows);
    return results;
}).WithName("SolrSearch");

app.MapGet("/solrping", async (ISolrRepository solrRepository) =>
{
    var results = await solrRepository.Ping();
    return results;
}).WithName("SolrPing");


/******** IHazDadJokes methods ********/
app.MapGet("/dadjoke/{id}", async (IDadJokeService dadJokeService, string id) =>
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


