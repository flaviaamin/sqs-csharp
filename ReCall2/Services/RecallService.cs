using Newtonsoft.Json.Linq;
using ReCall2.Clients;
using System;
using System.Collections.Generic;
using ReCall2.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Amazon.SQS.Model;

namespace ReCall2.Services
{
    public class RecallService
    {
        public RecallService()
        {
            JToken jAppSettings = JToken.Parse(System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
            this.sqsAwsService = new SQSAwsClient(
                jAppSettings["awsId"].ToString(),
                jAppSettings["awsKey"].ToString(),
                jAppSettings["hostSqs"].ToString(),
                jAppSettings["sqsId"].ToString(),
                jAppSettings["sqsName"].ToString()
            );
        }

        private SQSAwsClient sqsAwsService;

        public async Task<List<Recall>> Recalls()
        {
            var lista = await sqsAwsService.ListAWSSQS();
            var recalls = new List<Recall>();

            if (lista != null)
            {
                foreach (Message message in lista)
                {
                    try
                    {
                        var recall = JsonSerializer.Deserialize<Recall>(message.Body);
                        recall.SQSId = message.ReceiptHandle;
                        recalls.Add(recall);
                    }
                    catch { }
                }
            }

            return recalls;
        }

        public async Task<bool> DeleteRecall(string sqsId)
        {
            return await sqsAwsService.DeleteMessage(sqsId);
        }
    }
}
