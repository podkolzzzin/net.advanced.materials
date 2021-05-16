using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetAdvancedLiveCoding.Chat.Models;

namespace NetAdvancedLiveCoding.Chat.Controllers
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
        private readonly ILogger<HomeController> _logger;

        private static ConcurrentBag<Message> messages = new ConcurrentBag<Message>();
        private static ConcurrentBag<TaskCompletionSource<bool>> pendingTasks = new ConcurrentBag<TaskCompletionSource<bool>>();

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
                pendingTasks.Add(tcs);
                await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(60)), tcs.Task);
            }
            return Json(messages.Where(m => m.Index > index).OrderBy(m => m.Index));
        }

        [HttpPost]
        public IActionResult Send([FromForm]Message message)
        {
            message.Time = DateTime.Now.ToString("HH:mm");
            message.Date = DateTime.Now.ToString("MMM dd");
            message.Index = messages.Count + 1;
            
            var r = pendingTasks;
            pendingTasks = new ConcurrentBag<TaskCompletionSource<bool>>();
            messages.Add(message);
            foreach (var pt in r)
                pt.SetResult(true);
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
