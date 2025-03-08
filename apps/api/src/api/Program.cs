using api.Models;
using api.Repositories;
using api.Services;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Schema;

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
    var results = await solrRepository.Search(query, start, rows);
    return results;
}).WithName("SolrSearch");

app.MapGet("/solrping", async (ISolrRepository solrRepository) =>
{
    var results = await solrRepository.Ping();
    return results;
}).WithName("SolrPing");

app.MapGet("solrcorestatus", async (ISolrCoreAdmin solrCoreAdmin) =>
{
    IList<CoreResult> coreStatus = solrCoreAdmin.Status();
    return true;
}).WithName("SolrCoreStatus");

app.MapPost("solrpopulateindex/{count}", async (ISolrService solrService, int count) =>
{

    var results = solrService.PopulateIndex(count);
    return true;
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


// ready to run
app.Run();

