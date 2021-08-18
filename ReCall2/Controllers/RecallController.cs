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

        public async Task<IActionResult> List()
        {
            ViewBag.Lista = await new RecallService().Recalls();
            return View();
        }

        public async Task<IActionResult> Queues(bool? delete)
        {
            if (delete != null) ViewBag.Delete = delete;

            RecallService recallServiceCreate = new RecallService("CREATE_MESSAGE");
            ViewBag.TotalQueueCreate = await recallServiceCreate.RecallCount();
            ViewBag.TotalQueueNotVisibleCreate = await recallServiceCreate.RecallCountNotVisible();

            RecallService recallServiceUpdate = new RecallService("UPDATE_MESSAGE");
            ViewBag.TotalQueueUpdate = await recallServiceUpdate.RecallCount();
            ViewBag.TotalQueueNotVisibleUpdate = await recallServiceUpdate.RecallCountNotVisible();

            RecallService recallServiceSorry = new RecallService("SORRY_MESSAGE");
            ViewBag.TotalQueueSorry = await recallServiceSorry.RecallCount();
            ViewBag.TotalQueueNotVisibleSorry = await recallServiceSorry.RecallCountNotVisible();

            RecallService recallServiceWithDraw = new RecallService("WITHDRAW_MESSAGE");
            ViewBag.TotalQueueWithDraw = await recallServiceWithDraw.RecallCount();
            ViewBag.TotalQueueNotVisibleWithDraw = await recallServiceWithDraw.RecallCountNotVisible();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SQSDel(string environment, [FromForm] Recall recall)
        {
            try
            {
                var success = await new RecallService(environment).DeleteRecall(recall.SQSId);
                if (success)
                {
                    Console.WriteLine("successfully deleted");
                }
                else
                {
                    Console.WriteLine("error when deleting from the queue");
                }
                //return View();

                return Redirect("/?delete=true");
            }
            catch { return Redirect("/?delete=false"); }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string environment, Recall recall)
        {
            var success = await new RecallService(environment).Save(recall);

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
