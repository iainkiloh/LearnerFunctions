using LearnerFunctions.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LearnerFunctions
{
    public static class GenerateLicenceFile
    {
        [FunctionName("GenerateLicenceFile")]
        public static async Task Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            //[Blob("licenses/{rand-guid}.lic")] TextWriter outputBlob,
            IBinder blobBinder,
            ILogger log)
        {

            var outputBlob = await blobBinder.BindAsync<TextWriter>(
                new BlobAttribute(blobPath: $"licenses/{order.OrderId}.lic")
                {
                    Connection = "AzureWebJobsStorage"
                });

            outputBlob.WriteLine($"OrderId:{order.OrderId}");
            outputBlob.WriteLine($"Email:{order.Email}");
            outputBlob.WriteLine($"ProductId:{order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate:{DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBlob.WriteLine($"secret: {BitConverter.ToString(hash).Replace("-", "")}");

            log.LogInformation($"C# Queue trigger function processed: {order}");

        }
    }
}
