using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.IO;
using System.Text.Json;
using ReCall2.Models;

namespace ReCall2.Clients
{
    public class SQSAwsClient
    {
        public SQSAwsClient(ConfigManager configManager)
        {
            this.configManager = configManager;
        }

        private ConfigManager configManager;

        public async Task<List<Message>> ListAWSSQS()
        {
             AmazonSQSClient sq = new AmazonSQSClient(configManager.AwsId, configManager.AwsKey);
           // AmazonSQSClient sq = new AmazonSQSClient(new BasicAWSCredentials(awsId, awsKey), RegionEndpoint.GetBySystemName("eu-central-1"));
            ReceiveMessageRequest rmr = new ReceiveMessageRequest();
            rmr.QueueUrl = $"{configManager.SqsHost}/{configManager.SqsId}/{configManager.SqsName}";
            rmr.MaxNumberOfMessages = 10;
            ReceiveMessageResponse response = await sq.ReceiveMessageAsync(rmr);
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK) ? response.Messages : null;
        }

        public async Task<int> TotalQueue()
        {
            return (await this.ListAWSSQS()).Count;
        }

        public async Task<bool> DeleteMessage(string receiptHandle)
        {
            AmazonSQSClient sq = new AmazonSQSClient(configManager.AwsId, configManager.AwsKey);
            DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest();

            deleteMessageRequest.QueueUrl = $"{configManager.SqsHost}/{configManager.SqsId}/{configManager.SqsName}";
            deleteMessageRequest.ReceiptHandle = receiptHandle;

            var response = await sq.DeleteMessageAsync(deleteMessageRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<bool> SendAWSSQS(Recall recall)
        {
            //  AmazonSQSClient sqsClient = new AmazonSQSClient(new BasicAWSCredentials(awsId, awsKey), RegionEndpoint.GetBySystemName("eu-central-1"));
            
            AmazonSQSClient sqsClient = new AmazonSQSClient(configManager.AwsId, configManager.AwsKey);
            var sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.QueueUrl = $"{configManager.SqsHost}/{configManager.SqsId}/{configManager.SqsName}";
            sendMessageRequest.MessageBody = JsonSerializer.Serialize(recall);

            var sendMessageResponse = await sqsClient.SendMessageAsync(sendMessageRequest);
            return sendMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<int> GetSqsCount()
        {
            AmazonSQSClient sqsClient = new AmazonSQSClient(configManager.AwsId, configManager.AwsKey);
            var request = new GetQueueAttributesRequest { QueueUrl = $"{configManager.SqsHost}/{configManager.SqsId}/{configManager.SqsName}" };
            request.AttributeNames.Add("ApproximateNumberOfMessages");

            var response = await sqsClient.GetQueueAttributesAsync(request);
            return response.ApproximateNumberOfMessages;
        }

        public async Task<int> GetSqsNotVisibleCount()
        {
            AmazonSQSClient sqsClient = new AmazonSQSClient(configManager.AwsId, configManager.AwsKey);
            var request = new GetQueueAttributesRequest { QueueUrl = $"{configManager.SqsHost}/{configManager.SqsId}/{configManager.SqsName}" };
            request.AttributeNames.Add("ApproximateNumberOfMessagesNotVisible");

            var response = await sqsClient.GetQueueAttributesAsync(request);
            return response.ApproximateNumberOfMessagesNotVisible;
        }
    }
}
