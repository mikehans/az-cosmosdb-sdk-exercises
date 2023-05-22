using CosmosDbProductCatalogue.DTOs;
using Microsoft.Azure.Cosmos;
using Serilog;
using System;
using System.Linq;

namespace CosmosDbProductCatalogue.DataAccess;

public class Updates
{
    readonly CosmosClient client;
    readonly ILogger logger;

    public Updates(CosmosClient client, ILogger logger)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task UpsertADocument()
    {
        logger.Information("\nUPSERT");
        logger.Information("\tWill replace document name");

        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");

        var query = new QueryDefinition(
                query: "SELECT * FROM categories c WHERE c.name = @categoryName"
            ).WithParameter("@categoryName", "Brakes");

        using var filteredFeed = container.GetItemQueryIterator<CategoryDTO>(
            queryDefinition: query);

        while (filteredFeed.HasMoreResults)
        {
            FeedResponse<CategoryDTO> results = await filteredFeed.ReadNextAsync();

            foreach (var result in results)
            {
                logger.Information($"Found: \t{result.id} \t{result.name}");
                CategoryDTO updatedCategory = new CategoryDTO
                {
                    id = result.id,
                    name = $"Upserted {result.name}",
                    ancestors = result.ancestors,
                    parent = result.parent
                };
                ItemResponse<CategoryDTO> upsertItem = await container.UpsertItemAsync(updatedCategory, new PartitionKey(updatedCategory.id));
                logger.Information($"upsert status code: {upsertItem.StatusCode}");
            }
        }
    }

    public async Task ReplaceADocument()
    {
        logger.Information("\nREPLACE");
        logger.Information("\tWill replace document name");

        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");

        var query = new QueryDefinition(
                query: "SELECT * FROM categories c WHERE c.name = @categoryName"
            ).WithParameter("@categoryName", "Upserted Brakes");

        using var filteredFeed = container.GetItemQueryIterator<CategoryDTO>(
            queryDefinition: query);

        while (filteredFeed.HasMoreResults)
        {
            FeedResponse<CategoryDTO> results = await filteredFeed.ReadNextAsync();

            foreach (var result in results)
            {
                logger.Information($"Found: \t{result.id} \t{result.name}");
                CategoryDTO updatedCategory = new CategoryDTO
                {
                    id = result.id,
                    name = $"{result.name} and replaced",
                    ancestors = result.ancestors,
                    parent = result.parent
                };

                var response = await container.ReplaceItemAsync<CategoryDTO>(updatedCategory, result.id, new PartitionKey(result.id));

                logger.Information($"Replace result status code: {response.StatusCode}");
            }


        }
    }
}
