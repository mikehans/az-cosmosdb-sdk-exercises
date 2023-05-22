using CosmosDbProductCatalogue.DTOs;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Abstractions;
using Serilog;
using Serilog.Core;
using System.ComponentModel;

namespace CosmosDbProductCatalogue.DataAccess;

// These examples taken from: https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/how-to-dotnet-query-items

public class Queries
{
    private readonly CosmosClient client;
    private readonly ILogger logger;
    public Queries(CosmosClient client, ILogger logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task GetAllCategories()
    {
        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");

        // query with container.GetItemQueryIterator<T>
        logger.Information("\nQUERY USING GetItemQueryIterator<T> WITH A STRING");
        logger.Information("\tQuery is: SELECT * FROM categories");

        using FeedIterator<CategoryDTO> feed =
            container.GetItemQueryIterator<CategoryDTO>(
                queryText: "SELECT * FROM categories"
            );

        while (feed.HasMoreResults)
        {
            FeedResponse<CategoryDTO> results = await feed.ReadNextAsync();

            foreach (var result in results)
            {
                logger.Information($"Found result: \t {result.name}");
            }
        }

    }

    public async Task GetCategoriesNamedBrakes()
    {
        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");
        // query with container.GetItemQueryIterator<T>
        logger.Information("\n\nQUERY USING GetItemQueryIterator<T> WITH A QueryDefinition OBJECT FOR USING PARAMS");
        logger.Information("\tQuery gets categories where the category name is \"Brakes\" ");

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
                logger.Information($"Found: \t{result.name}");
            }
        }
    }

    public async Task GetCategoriesByIQueryable()
    {
        var database = client.GetDatabase("product-catalogue");
        var container = database.GetContainer("categories");
        // query with container.GetItemLinqQueryable<T>
        logger.Information("\n\nQUERY USING GetItemLinqQueryable<T> WITH A LINQ QUERY");
        logger.Information("\tQuery finds all categories with parent ID \"cat2\" ");

        IOrderedQueryable<CategoryDTO> queryable = container.GetItemLinqQueryable<CategoryDTO>();

        var matches = queryable.Where(c => c.parent.Id == "cat2");

        using FeedIterator<CategoryDTO> linqFeed = matches.ToFeedIterator();

        while (linqFeed.HasMoreResults)
        {
            FeedResponse<CategoryDTO> results = await linqFeed.ReadNextAsync();

            foreach (var result in results)
            {
                logger.Information($"Matched item: \t {result.name}");
            }
        }
    }
}
