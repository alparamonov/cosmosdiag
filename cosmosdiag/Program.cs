using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Azure.Documents;
using System.Linq;

namespace cosmosdiag
{
    class Program
    {
        private readonly string endpoint = "https://<ENDPOINT>.documents.azure.com:443/";
        private readonly string key1 = "<SECRET_KEY>";
        private readonly string databaseId = "<DATABASE_NAME>";
        private readonly string containerId = "<CONTAINER_NAME>";

        private CosmosClient cosmosClient;
        private Microsoft.Azure.Cosmos.Database database;
        private Container container;

        static async Task Main()
        {
            Program p = new Program();
            p.PrepareConnection();
            await p.RunDiag(10);
        }

        private void PrepareConnection()
        {
            this.cosmosClient = new CosmosClient(
              endpoint,
              key1,
              new CosmosClientOptions()
              {
                  ApplicationName = "CosmosDiag",
                  ApplicationRegion = LocationNames.WestEurope
              });
            
            GetDatabase();
            GetContainer();
        }

        private async Task RunDiag(int repetitions)
        {
            for (int i = 0; i < repetitions; i++)
            {
                await QueryItemsAsync();
            }
        }

        private void GetDatabase()
        {
            this.database = this.cosmosClient.GetDatabase(databaseId);
        }

        private void GetContainer()
        {
            this.container = this.database.GetContainer(containerId);
        }

        private async Task QueryItemsAsync()
        {
            var query = "SELECT * FROM c WHERE c.id = '123'";

            QueryDefinition queryDefinition = new QueryDefinition(query);
            FeedIterator<dynamic> queryResultSetIterator =
                this.container.GetItemQueryIterator<dynamic>(queryDefinition, requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey("123")
                });

            List<dynamic> items = new List<dynamic>();

            while (queryResultSetIterator.HasMoreResults)
            {
                Microsoft.Azure.Cosmos.FeedResponse<dynamic> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                string diagDataStr = currentResultSet.Diagnostics.ToString();

                DiagData data = JsonConvert.DeserializeObject<DiagData>(diagDataStr);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Total Elapsed time:\t{data.Summary.TotalElapsedTimeInMs / 1000} s");
                sb.AppendLine($"Total Request count:\t{data.Summary.TotalRequestCount}");
                sb.AppendLine($"Create Query Time:\t{data.Context[0].ElapsedTimeInMs / 1000} s");
                sb.AppendLine($"Service Interop Time:\t{data.Context[1].ElapsedTimeInMs / 1000} s");
                sb.AppendLine("Query Metrics:");
                var queryMetrics = data.Context[2].QueryMetric.Split(';');
                foreach (string metric in queryMetrics)
                {
                    sb.AppendLine($"\t{metric}");
                }
                string physicalStore = data
                    .Context[2]
                    .ContextDetailed
                    .Last(a => a.StoreResult != null)
                    .StoreResult
                    .Split(' ')[1];
                sb.AppendLine($"\t{physicalStore}");
                Console.WriteLine(sb.ToString());
                foreach (dynamic item in currentResultSet)
                {
                    items.Add(item);
                    Console.WriteLine("\tRead {0}\n", item);
                }
            }
        }
    }
}
