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

namespace ReCall2
{
    public class ConfigManager
    {
        public static string AwsId { get; set; }
        public static string AwsKey { get; set; }
        public static string SqsHost { get; set; }
        public static string SqsId { get; set; }
        public static string SqsName { get; set; }
    }
}
