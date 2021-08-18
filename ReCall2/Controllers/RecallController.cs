using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ReCall2.Clients;
using ReCall2.Enuns;
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

        private RecallEnvironment getRecallEnv()
        {
            try
            {
                string envCookie = HttpContext.Request.Cookies["env"];
                return Enum.Parse<RecallEnvironment>(envCookie);
            }
            catch { return RecallEnvironment.DLY; }
        }

        public async Task<IActionResult> List()
        {
            ViewBag.Lista = await new RecallService(getRecallEnv()).Recalls();
            return View();
        }

        public async Task<IActionResult> Queues(bool? delete, string queue)
        {
            if (delete != null) ViewBag.Delete = delete;
            if (queue != null) ViewBag.Queue = queue;

            RecallEnvironment env = getRecallEnv();
            
            RecallService recallServiceCreate = new RecallService(env, "CREATE_MESSAGE");
            ViewBag.TotalQueueCreate = await recallServiceCreate.RecallCount();
            ViewBag.TotalQueueNotVisibleCreate = await recallServiceCreate.RecallCountNotVisible();

            RecallService recallServiceUpdate = new RecallService(env, "UPDATE_MESSAGE");
            ViewBag.TotalQueueUpdate = await recallServiceUpdate.RecallCount();
            ViewBag.TotalQueueNotVisibleUpdate = await recallServiceUpdate.RecallCountNotVisible();

            RecallService recallServiceSorry = new RecallService(env, "SORRY_MESSAGE");
            ViewBag.TotalQueueSorry = await recallServiceSorry.RecallCount();
            ViewBag.TotalQueueNotVisibleSorry = await recallServiceSorry.RecallCountNotVisible();

            RecallService recallServiceWithDraw = new RecallService(env, "WITHDRAW_MESSAGE");
            ViewBag.TotalQueueWithDraw = await recallServiceWithDraw.RecallCount();
            ViewBag.TotalQueueNotVisibleWithDraw = await recallServiceWithDraw.RecallCountNotVisible();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SQSDel(string queue, [FromForm] Recall recall)
        {
            try
            {
                RecallEnvironment env = getRecallEnv();
                var success = await new RecallService(env, queue).DeleteRecall(recall.SQSId);
                if (success)
                {
                    Console.WriteLine("successfully deleted");
                }
                else
                {
                    Console.WriteLine("error when deleting from the queue");
                }
                //return View();

                return Redirect("/?delete=true&queue=" + queue);
            }
            catch { return Redirect("/?delete=false&queue=" + queue); }
        }

        [HttpPost]
        public async Task<IActionResult> Register(Recall recall)
        {
            RecallEnvironment env = getRecallEnv();
            var success = await new RecallService(env).Save(recall);

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
