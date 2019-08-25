using LearnerFunctions.TableStorageClasses;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;

namespace LearnerFunctions
{
    public static class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        public static void Run(
            //[BlobTrigger("licenses/{name}", Connection = "AzureWebJobsStorage")]string licenseFileContents,
            //[SendGrid(ApiKey = "SendGridApiKey")] out SendGridMessage message,
            [BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContents,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> mailSender,
            [Table("orders", "orders", "{orderId}")] OrderStorage orderStorage, //fetches data from table storage
            string orderId, 
            ILogger log)
        {

            log.LogInformation($"Got license file for: {orderStorage.Email}\n License filename: {orderId}");

            var message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(orderStorage.Email);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);
            message.AddAttachment(orderId, base64, "text/plain");
            message.Subject = "Your License File";
            message.HtmlContent = "Thank you for you order";

            if(!orderStorage.Email.EndsWith("@test.com"))
            {
                mailSender.Add(message);
            }


        }



}
}
