using LearnerFunctions.Contracts;
using LearnerFunctions.TableStorageClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace LearnerFunctionApp
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            [Table("orders")] IAsyncCollector<OrderStorage> orderTable,
            ILogger log)
        {
            log.LogInformation("Recevied a payment");

            //add to queue - queue is created if it doesn't already exist, message is serialized for us too - nice!
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order data = JsonConvert.DeserializeObject<Order>(requestBody);
            await orderQueue.AddAsync(data);

            //we also store the order in table storage in the same storage location
            //we could also add this to CosmosDB, or our own Db if we wanted to
            OrderStorage orderTableStorageItem = new OrderStorage
            {
                PartitionKey = "orders",
                RowKey = data.OrderId,
                Email = data.Email,
                OrderId = data.OrderId,
                Price = data.Price,
                ProductId = data.ProductId
            };

            await orderTable.AddAsync(orderTableStorageItem);

            log.LogInformation($"Order {data.OrderId} received from {data.Email} for productId {data.ProductId}");

            return new OkObjectResult($"Thank you for your payment");
        }
    }
}
