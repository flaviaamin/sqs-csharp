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
        public RecallService(string queue = "CREATE_MESSAGE")
        {
            this.sqsAwsService = new SQSAwsClient(new ConfigManager(queue));
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

        public async Task<int> RecallCount()
        {
            return await sqsAwsService.GetSqsCount();
        }

        public async Task<int> RecallCountNotVisible()
        {
            return await sqsAwsService.GetSqsNotVisibleCount();
        }

        public async Task<bool> Save(Recall recall)
        {
            if (recall.Vins != null && recall.Vins.Count == 1 && recall.Vins[0] != null && recall.Vins[0].Contains(","))
            {
                string[] vins = recall.Vins[0].Split(',');
                recall.Vins.Clear();
                foreach (string vin in vins)
                {
                    recall.Vins.Add(vin.Trim());
                }
            }

            recall.Translations = new Translation()
            {
                EnUs = new Country()
                {
                    Title = recall.Title,
                    Text = recall.Text,
                    ThankYouMessage = recall.ThankYouMessage
                }
            };

            recall.EventDate = DateTime.Parse(recall.EventDate).AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Millisecond).AddSeconds(DateTime.Now.Second).ToString();

            return await sqsAwsService.SendAWSSQS(recall);

        }
    }
}
