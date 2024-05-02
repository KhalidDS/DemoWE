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

        public async Task<IActionResult> Employee(string? DepartmentID)
        {
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);
            await HttpContext.Session.LoadAsync();
            string username = HttpContext.Session.GetString("Username") ?? string.Empty;
            ViewBag.name = username;
            await HttpContext.Session.LoadAsync();
            string DeptID = HttpContext.Session.GetString("DepartmentID") ?? string.Empty;
            ViewBag.Department = DeptID;
            string DepartmentName = string.Empty; // Initialize DepartmentName variable

            Department departmentName = await _context.Department.FirstOrDefaultAsync(t => t.DepartmentID.ToString() == DeptID);
            DepartmentName = departmentName?.DepartmentName ?? string.Empty;

            ViewBag.DepartmentName = DepartmentName; // Assign DepartmentName to ViewBag

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

            List<string> labels = _context.STask.Where(x => x.AssignedTo == userIdInt).Select(x => x.TaskTitle).ToList();
            Data.Add(labels);
            List<DateTime> StartDate = _context.STask.Where(x => x.AssignedTo == userIdInt).Select(x => x.StartDate).ToList();
            Data.Add(StartDate);
            List<DateTime> Deadline = _context.STask.Where(x => x.AssignedTo == userIdInt).Select(x => x.Deadline).ToList();
            Data.Add(Deadline);

            return Data;
        }

        public List<object> TData(int? AssignedTo, int? CreatedBy)
        {
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            List<object> SData = new List<object>();

            List<string> TaskTitle = _context.STask.Where(x => x.AssignedTo == userIdInt)
                                                   .OrderByDescending(x => x.TaskID)
                                                   .Take(3)
                                                   .Select(x => x.TaskTitle)
                                                   .ToList();
            List<Status> status = _context.STask.Where(x => x.AssignedTo == userIdInt)
                                                .OrderByDescending(x => x.TaskID)
                                                .Take(3)
                                                .Select(x => x.Status)
                                                .ToList();

            List<string> statusText = new List<string>();
            foreach (var s in status)
            {
                switch (s)
                {
                    case Status.New:
                        statusText.Add("New");
                        break;
                    case Status.InProgress:
                        statusText.Add("InProgress");
                        break;
                    case Status.Declined:
                        statusText.Add("Declined");
                        break;
                    case Status.Completed:
                        statusText.Add("Completed");
                        break;
                    default:
                        statusText.Add("Unknown");
                        break;
                }
            }

            SData.Add(TaskTitle);
            SData.Add(statusText);

            return SData;
        }
    }
}