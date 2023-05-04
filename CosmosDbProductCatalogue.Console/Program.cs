using Azure.Identity;
using CosmosDbProductCatalogue.DataAccess;
using CosmosDbProductCatalogue.DTOs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Text.Json;

Logger logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .MinimumLevel
    .Debug()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (ctx, services) =>
        {
            // CosmosClient options:
            //   1. initialise with a connection string

            services.AddSingleton(s =>
                new CosmosClient(
                    ctx.Configuration["CosmosDbConnectionString"],
                    new CosmosClientOptions() { AllowBulkExecution = true })
            );

            //   2. initialise with DefaultAzureCredential - recommended in docs
            //services.AddSingleton(
            //    s => new CosmosClient(
            //            "https://product-catalogue-db.documents.azure.com:443/",
            //            tokenCredential: new DefaultAzureCredential(),
            //            clientOptions: new CosmosClientOptions() { AllowBulkExecution = true }));
        })
    .UseSerilog()
    .Build();

CosmosClient cosmosClient = host.Services.GetRequiredService<CosmosClient>();
DbManagement dbService = new DbManagement(cosmosClient);

logger.Information("Cosmos Client: {@Client}", cosmosClient.Endpoint);
try
{
    await dbService.EnsureDbAndContainersCreated();

    logger.Information("DB and containers created");

    // set up JSON Serializer options for camelCase
    JsonSerializerOptions options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    // read data in from categories.json 
    var text = await File.ReadAllTextAsync("Data/categories.json");

    // parse to JSON
    var categoryJson = JsonSerializer.Deserialize<ICollection<CategoryDTO>>(text, options);

    // get the container
    var database = cosmosClient.GetDatabase("product-catalogue");
    var container = database.GetContainer("categories");

    // create a List<Task> for the items
    List<Task> tasks = new List<Task>();
    foreach (var item in categoryJson)
    {
        tasks.Add(
            container.CreateItemAsync(item, new PartitionKey(item.id))
                .ContinueWith(response =>
                {
                    if (!response.IsCompletedSuccessfully)
                    {
                        AggregateException innerExceptions = response.Exception.Flatten();

                        if (innerExceptions.InnerExceptions.FirstOrDefault(inner => inner is CosmosException) is CosmosException cosmosException)
                        {
                            logger.Error("Received {@statusCode} :: {@message}", cosmosException.StatusCode, cosmosException.Message);
                        }
                        else
                        {
                            logger.Error("Exception: {@ex}", innerExceptions.InnerExceptions.FirstOrDefault());
                        }
                    }
                }
            )
        );
    }

    // await write to CosmosDB categories container
    await Task.WhenAll(tasks);
} catch(Exception ex)
{
    logger.Error(ex.Message);
}

logger.Information("DONE");

await host.RunAsync();
