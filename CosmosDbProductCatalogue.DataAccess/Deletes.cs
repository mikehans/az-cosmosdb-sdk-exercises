using CosmosDbProductCatalogue.DTOs;
using Microsoft.Azure.Cosmos;
using Serilog;
using System;
using System.Linq;

namespace CosmosDbProductCatalogue.DataAccess;

public class Deletes
{
    readonly ILogger logger;
    readonly CosmosClient client;

    public Deletes(CosmosClient client, ILogger logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task DeleteOneItem()
    {
        logger.Information("\nDELETE");
        logger.Information("\tWill delete Road Bikes sub-category (ID: cat92)");

        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");

        var query = new QueryDefinition(
                query: "SELECT * FROM categories c WHERE c.id = @identifier"
            ).WithParameter("@identifier", "cat92");

        using FeedIterator<CategoryDTO> feed =
            container.GetItemQueryIterator<CategoryDTO>(
                queryDefinition: query
            );

        while (feed.HasMoreResults)
        {
            FeedResponse<CategoryDTO> results = await feed.ReadNextAsync();

            foreach (var result in results)
            {
                logger.Information($"Deleting result: \t {result.id} \t{result.name}");

                ItemResponse<CategoryDTO> response = await container.DeleteItemAsync<CategoryDTO>(result.id, new PartitionKey(result.id));
                logger.Information("\tDeleted item cat92");
            }
        }
    }
}
