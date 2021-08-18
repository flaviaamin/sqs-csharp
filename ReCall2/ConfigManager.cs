﻿using System;
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
using Newtonsoft.Json.Linq;

namespace ReCall2
{
    public class ConfigManager
    {
        public ConfigManager(Environment env, string queue)
        {
            this.setup(env, queue);
        }

        public string AwsId { get; set; }
        public string AwsKey { get; set; }
        public string SqsHost { get; set; }
        public string SqsId { get; set; }
        public string SqsName { get; set; }

        private void setup(Environment env, string queue)
        {
            var pathFile = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            if (File.Exists(pathFile))
            {
                JToken jAppSettings = JToken.Parse(System.IO.File.ReadAllText(pathFile));
                this.AwsId = jAppSettings["awsId"].ToString();
                this.AwsKey = jAppSettings["awsKey"].ToString();
                this.SqsHost = jAppSettings["queue"][env][queue]["sqsHost"].ToString();
                this.SqsId = jAppSettings["queue"][env][queue]["sqsId"].ToString();
                this.SqsName = jAppSettings["queue"][env][queue]["sqsName"].ToString();
            }
            else
            {
                this.AwsId = Environment.GetEnvironmentVariable("AWS_ID");
                this.AwsKey = Environment.GetEnvironmentVariable("AWS_KEY");
                this.SqsHost = Environment.GetEnvironmentVariable($"{env}_{queue}_SQS_HOST");
                this.SqsId = Environment.GetEnvironmentVariable($"{env}_{queue}_SQS_ID");
                this.SqsName = Environment.GetEnvironmentVariable($"{env}_{queue}_SQS_NAME");
            }
        }
    }
}
