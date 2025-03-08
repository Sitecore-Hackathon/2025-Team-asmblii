using api.Models;
using api.Repositories;
using api.Services;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SolrNet;
using SolrNet.Impl;

var builder = WebApplication.CreateBuilder(args);

// add healthchecks
builder.Services.AddHealthChecks();

// add opentelemetry
var serviceName = builder.Configuration.GetValue<string>("OtelServiceName") ?? "api-app";
var otlpExporterSection = builder.Configuration.GetSection("OtlpExporter");

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
    options.AddOtlpExporter(options => otlpExporterSection.Bind(options));
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing =>
      {
          if (builder.Environment.IsDevelopment())
          {
              tracing.SetSampler<AlwaysOnSampler>();
          }

          tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.SetVerboseDatabaseStatements = true;
                    options.FlushInterval = TimeSpan.FromSeconds(1);
                }
            })
            .SetErrorStatusOnException()
            .AddOtlpExporter(options => otlpExporterSection.Bind(options));
      })
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddRuntimeInstrumentation()
          .AddOtlpExporter(options => otlpExporterSection.Bind(options)));

// add OpenAPI
builder.Services.AddOpenApi();

// add API services
var solrUri = builder.Configuration["Solr:Uri"];
var apiUri = builder.Configuration["Uri"];

builder.Services.AddSolrNet<SolrSearchResultEntry>(solrUri); 
builder.Services.AddScoped<ISolrRepository, SolrRepository>();
builder.Services.AddHttpClient<IDadJokeService, DadJokeService>(client =>
{
    var icanhazdadjokesAddress = builder.Configuration["ICanHazDadJokes:Uri"] ?? throw new Exception("Baseaddress for the joke service is not set!");
    var requestingParty = builder.Configuration["ICanHazDadJokes:RequestingParty"] ?? throw new Exception("If using ICanHazDadJoke they kindly request that contact details in form of a website or email is supplied to requests to their free API. See https://icanhazdadjoke.com/api#custom-user-agent for more info.");

    client.BaseAddress = new Uri(icanhazdadjokesAddress);
    client.DefaultRequestHeaders.UserAgent.ParseAdd(requestingParty);
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

//To enable Solr Core Admin features
builder.Services.AddScoped<ISolrStatusResponseParser, SolrStatusResponseParser>();
builder.Services.AddScoped<ISolrCoreAdmin, SolrCoreAdmin>();

builder.Services.AddScoped<ISolrService, SolrService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // add OpenAPI endpoints
    app.MapOpenApi();
}

// add healthcheck endpoint
app.MapHealthChecks("/healthz");

// add test endpoint
app.MapGet("/test", () =>
{
    return "solr:uri=" + solrUri;
}).WithName("Test");

// add Solr endpoint
app.MapGet("/solrsearch", async (ISolrRepository solrRepository, string query, int start, int rows) =>
{
    var results = await solrRepository.SearchAsync(query, start, rows);
    return results;
}).WithName("SolrSearch");

app.MapGet("/solrping", async (ISolrService solrService) =>
{
    var results = await solrService.PingAsync();
    return results;
}).WithName("SolrPing");

app.MapGet("solrcorestatus", (ISolrCoreAdmin solrCoreAdmin) =>
{
    IList<CoreResult> coreStatus = solrCoreAdmin.Status();
    return coreStatus;
}).WithName("SolrCoreStatus");

app.MapPost("solrpopulateindex/{count}", async (ISolrService solrService, int count) =>
{
    var results = await solrService.PopulateIndexAsync(count);
    
    return results;
}).WithName("SolrPopulateIndex");


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

/******** Slow methods.... *******/
app.MapGet("/slowhi", async () => 
{
    var value = new Random().Next(400, 20000);
    await Task.Delay(value);
    return new { message = "Hi" };
}).WithName("SlowHi");

app.MapGet("/randomrequests", async (HttpClient client) =>
{
    var baseAddress = builder.Configuration["Uri"] ?? throw new Exception("Baseaddress for the api is not set!");
    client.BaseAddress = new Uri(apiUri);
    var random = new Random();
    var numberOfRequests = random.Next(1, 20); // Random number of requests between 1 and 10
    var jokeIds = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
    var endpoints = new List<string>
    {
        "/test",
        $"/solrsearch?query=test&start=0&rows={random.Next(2,20)}",
        "/solrping",
        "/solrcorestatus",
        $"/solrpopulateindex/{random.Next(2,20)}",
        $"/dadjoke/{jokeIds[random.Next(0,9)]}",
        "/randomdadjoke",
        "/slowhi"
    };

    var tasks = new List<Task<HttpResponseMessage>>();

    for (int i = 0; i < numberOfRequests; i++)
    {
        var endpoint = endpoints[random.Next(endpoints.Count)];
        tasks.Add(client.GetAsync(endpoint));
    }

    var responses = await Task.WhenAll(tasks);

    var results = new List<string>();
    foreach (var response in responses)
    {
        results.Add(await response.Content.ReadAsStringAsync());
    }
    return results;
}).WithName("RandomRequests");

// ready to run
app.Run();

