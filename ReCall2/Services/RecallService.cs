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
        public RecallService(string fila)
        {
            string awsId, awsKey, hostSqs, sqsId, sqsName;

            var pathFile = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            if (File.Exists(pathFile))
            {
                JToken jAppSettings = JToken.Parse(System.IO.File.ReadAllText(pathFile));
                awsId = jAppSettings["awsId"].ToString();
                awsKey = jAppSettings["awsKey"].ToString();
                hostSqs = jAppSettings["filas"][fila]["hostSqs"].ToString();
                sqsId = jAppSettings["filas"][fila]["sqsId"].ToString();
                sqsName = jAppSettings["filas"][fila]["sqsName"].ToString();
            }
            else
            {
                awsId = Environment.GetEnvironmentVariable("AWS_ID");
                awsKey = Environment.GetEnvironmentVariable("AWS_KEY");
                hostSqs = Environment.GetEnvironmentVariable($"{fila}_HOST_SQS");
                sqsId = Environment.GetEnvironmentVariable($"{fila}_SQS_ID");
                sqsName = Environment.GetEnvironmentVariable($"{fila}_SQS_NAME");
            }

            this.sqsAwsService = new SQSAwsClient(
                awsId,
                awsKey,
                hostSqs,
                sqsId,
                sqsName
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
