# Setup
You will need to create and configure the ```appsettings.json``` file in the root of the Console project. The basic format needs to look like the below example. This one is set to use the local Cosmos DB emulator.

```json
{
  "CosmosDbConnectionString": "AccountEndpoint=https://localhost:8081/;AccountKey=[insert key here];"
  "DatabaseUri": "https://localhost:8081"
}
```

The project is hard-coded to look for a database called ```product-catalogue``` and a container called ```categories```. You'll need to create these ahead of running the code.

