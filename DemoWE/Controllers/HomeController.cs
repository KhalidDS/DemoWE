using DemoWE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DemoWE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;



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

        public async Task<IActionResult> Employee(string? DepartmentID, Status? status)
        {
            // Load session variables
            await HttpContext.Session.LoadAsync();

            // Retrieve user ID from session
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            // Retrieve username from session
            string username = HttpContext.Session.GetString("Username") ?? string.Empty;
            ViewBag.name = username;

            // Retrieve department ID from session
            string DeptID = HttpContext.Session.GetString("DepartmentID") ?? string.Empty;
            ViewBag.Department = DeptID;

            // Initialize DepartmentName variable
            string DepartmentName = string.Empty;

            // Retrieve DepartmentName from the database
            Department departmentName = await _context.Department.FirstOrDefaultAsync(t => t.DepartmentID.ToString() == DeptID);
            DepartmentName = departmentName?.DepartmentName ?? string.Empty;

            // Count the number of tasks with status "New" or "InProgress" assigned to the user
            int newOrInProgressCount = await _context.STask
            .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null)
            .CountAsync();

            // Pass the count to the view
            ViewBag.NewOrInProgressCount = newOrInProgressCount;

            // Assign DepartmentName to ViewBag
            ViewBag.DepartmentName = DepartmentName;

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

            // Retrieve task titles
            List<string> labels = _context.STask
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) // Add condition for project_id == null
                .Select(x => x.TaskTitle)
                .ToList();
            Data.Add(labels);

            // Retrieve start dates
            List<DateTime> StartDate = _context.STask
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) // Add condition for project_id == null
                .Select(x => x.StartDate.Date)
                .ToList();
            Data.Add(StartDate);

            // Retrieve deadlines
            List<DateTime> Deadline = _context.STask
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) // Add condition for project_id == null
                .Select(x => x.Deadline.Date)
                .ToList();
            Data.Add(Deadline);

            return Data;
        }

        public List<object> TData(int? AssignedTo, int? CreatedBy)
        {
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            List<object> SData = new List<object>();

            List<string> TaskTitle = _context.STask.Where(x => x.AssignedTo == userIdInt && x.project_id == null) // Add condition for project_id == null
                                                   .OrderByDescending(x => x.TaskID)
                                                   .Take(3)
                                                   .Select(x => x.TaskTitle)
                                                   .ToList();
            List<Status> status = _context.STask.Where(x => x.AssignedTo == userIdInt && x.project_id == null) // Add condition for project_id == null
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