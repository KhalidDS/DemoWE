using DemoWE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DemoWE.Data;
using Microsoft.EntityFrameworkCore;



namespace DemoWE.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoWEContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DemoWEContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Employee()
        {
            string username = HttpContext.Session.GetString("Username");
            ViewBag.name = username;

            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public List<object> GetData(int? AssignedTo, int? CreatedBy)
        {
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);
            List<object> Data = new List<object>();

            List<string> labels = _context.STask.Select(x => x.TaskTitle).ToList();
            Data.Add(labels);
            List<DateTime> StartDate = _context.STask.Select(x => x.StartDate).ToList();
            Data.Add(StartDate);
            List<DateTime> Deadline = _context.STask.Select(x => x.Deadline).ToList();
            Data.Add(Deadline);

            return Data;
        }
    }
}