# C# Cosmos DB SDK exercises
Contains an exercise in creating a product catalogue in Cosmos DB based on the Adventureworks database. Presently, (ie. as of 18 May 2023) the project is only focussed on the categories and sub-categories entities.

The files in the DataPrep folder is a Node JS script to read in the two CSV files in the Console project and outputs the JSON file also in this folder.

## Program.cs
### CosmosClient configuration
The DI container contains code to configure the ```CosmosClient``` in two ways (see comments in the code): 
* using a connection string
* using the ```DefaultAzureCredentials()``` method

### Bulk data upload
Lines 69 -> 95 contains code for bulk uploading documents to Cosmos DB. 

Essentially, we are creating a ```List<Task>()``` and adding each ```CreateItemAsync``` call as a ```Task<T>```. In line 95 we execute all tasks as a batch.

Note also that the ```CosmosClient``` needs to be configured with options to make bulk upload possible.
