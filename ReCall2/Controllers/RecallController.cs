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
        public IActionResult New()
        {
            return View();
        }

        private RecallService recallService = new RecallService();

        public async Task<IActionResult> List()
        {
            ViewBag.Lista = await recallService.Recalls();
            return View();
        }

        public async Task<IActionResult> Queues(bool? apagou)
        {
            if (apagou != null) ViewBag.Apagou = apagou;
            ViewBag.TotalQueue = await recallService.TotalQueue();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SQSDel([FromForm] Recall recall)
        {
            try
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
                //return View();

                return Redirect("/?apagou=true");
            }
            catch { return Redirect("/?apagou=false"); }
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
    }
}
