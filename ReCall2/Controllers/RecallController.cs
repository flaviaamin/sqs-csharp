using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ReCall2.Clients;
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
            recallService = new RecallService();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private RecallService recallService;

        public async Task<IActionResult> List()
        {
            ViewBag.Lista = await recallService.Recalls();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SQSDel([FromForm] Recall recall)
        {
            var success = await recallService.DeleteRecall(recall.SQSId);
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
            var success = await recallService.Save(recall);

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
