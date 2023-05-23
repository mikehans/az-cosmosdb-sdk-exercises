using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbProductCatalogue.DataAccess; 

public class DbManagement : IDbManagement
{
    CosmosClient _client;

    public DbManagement(CosmosClient client)
    {
        _client = client;
    }

    public async Task<bool> EnsureDbAndContainersCreated()
    {
        bool dbCreated = await EnsureDbCreated();
        if (dbCreated)
        {
            bool containersCreated = await EnsureContainersCreated();

            if (containersCreated) {
                return true;
            } else
            {
                throw new Exception("Failed to create the Containers");
            }
        } else
        {
            throw new Exception("Failed to create the database");
        }
    }

    public async Task<bool> EnsureDbCreated()
    {
        await _client.CreateDatabaseIfNotExistsAsync("product-catalogue");

        Database database = _client.GetDatabase(id: "product-catalogue");

        return database != null;
    }

    public async Task<bool> EnsureContainersCreated()
    {
        Database database = _client.GetDatabase(id: "product-catalogue");

        ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync("categories", "/id");

        return true;
    }
}
