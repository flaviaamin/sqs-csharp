using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ReCall2.Models;
using ReCall2.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReCall2.Controllers
{
    public class RecallController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public RecallController(ILogger<HomeController> logger)
        {
            _logger = logger;

            JToken jAppSettings = JToken.Parse(System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
            this.sqsAwsService = new SQSAws(
                jAppSettings["awsId"].ToString(),
                jAppSettings["awsKey"].ToString(),
                jAppSettings["hostSqs"].ToString(),
                jAppSettings["sqsId"].ToString(),
                jAppSettings["sqsName"].ToString());
        }

        public IActionResult Index()
        {
            return View();
        }

        private SQSAws sqsAwsService;

        public async Task<IActionResult> List()
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

            ViewBag.Lista = recalls;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SQSDel([FromForm] Recall recall)
        {
            var success = await sqsAwsService.DeleteMessage(recall.SQSId);
            if (success)
            {
                Console.WriteLine("successfully deleted");
            }
            else
            {
                Console.WriteLine("error when deleting from the queue");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Recall recall)
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

            var success = await sqsAwsService.SendAWSSQS(recall);

            if (success)
            {
                Console.WriteLine("successfully registered");
            }
            else
            {
                Console.WriteLine("Error when registering in the queue");
            }

            //return StatusCode(200, recall);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
