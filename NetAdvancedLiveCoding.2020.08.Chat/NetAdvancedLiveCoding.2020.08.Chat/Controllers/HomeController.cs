using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetAdvancedLiveCoding._2020._08.Chat.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetAdvancedLiveCoding._2020._08.Chat.Controllers
{
    public class Message
    {
        public int UserId { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }

        public int Index { get; set; }
    }

    public class HomeController : Controller
    {
        private static ConcurrentBag<Message> messages = new ConcurrentBag<Message>();
        private static ConcurrentBag<TaskCompletionSource<bool>> taskCompletionSources = new ConcurrentBag<TaskCompletionSource<bool>>();


        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Listen(int index)
        {
            if (index == messages.Count)
            {
                var tcs = new TaskCompletionSource<bool>();
                taskCompletionSources.Add(tcs);
                await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(60)));
            }
            return Json(messages.Where(m => m.Index > index).OrderBy(m => m.Index));
        }

        [HttpPost]
        public IActionResult Send([FromForm] Message message)
        {
            message.Time = DateTime.Now.ToString("HH:mm");
            message.Date = DateTime.Now.ToString("MMM dd");
            message.Index = messages.Count + 1;
            messages.Add(message);

            var oldTasks = taskCompletionSources;
            taskCompletionSources = new ConcurrentBag<TaskCompletionSource<bool>>();

            foreach (var tcs in oldTasks)
                tcs.SetResult(true);

            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
