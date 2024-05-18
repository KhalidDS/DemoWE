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
            await HttpContext.Session.LoadAsync();
            string username = HttpContext.Session.GetString("Username") ?? string.Empty;
            ViewBag.name = username;

            // Retrieve email from session
            await HttpContext.Session.LoadAsync();
            string email = HttpContext.Session.GetString("Email") ?? string.Empty;
            ViewBag.email = email;


            // Retrieve department ID from session
            await HttpContext.Session.LoadAsync();
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

        public async Task<IActionResult> Admin(string? DepartmentID, Status? status)
        {
            
            // Load session variables
            await HttpContext.Session.LoadAsync();

            // Retrieve user ID from session
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            // Retrieve username from session
            string username = HttpContext.Session.GetString("Username") ?? string.Empty;
            ViewBag.name = username;

            // Retrieve email from session
            string email = HttpContext.Session.GetString("Email") ?? string.Empty;
            ViewBag.email = email;


            // Retrieve department ID from session
            string DeptID = HttpContext.Session.GetString("DepartmentID") ?? string.Empty;
            ViewBag.Department = DeptID;

            // Initialize DepartmentName variable
            string DepartmentName = string.Empty;

            // Retrieve DepartmentName from the database
            Department departmentName = await _context.Department.FirstOrDefaultAsync(t => t.DepartmentID.ToString() == DeptID);
            DepartmentName = departmentName?.DepartmentName ?? string.Empty;

            // Count the number of tasks with status "New" or "InProgress" assigned to the user
            int EscalatedRequestCount = await _context.Request
           .Where(x => x.Status == Status.Escalated)
           .CountAsync();

            ViewBag.EscalatedRequestCount = EscalatedRequestCount;


            // Assign DepartmentName to ViewBag
            ViewBag.DepartmentName = DepartmentName;

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
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) 
                .Select(x => x.TaskTitle)
                .ToList();
            Data.Add(labels);

            // Retrieve start dates
            List<DateTime> StartDate = _context.STask
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) 
                .Select(x => x.StartDate.Date)
                .ToList();
            Data.Add(StartDate);

            // Retrieve deadlines
            List<DateTime> Deadline = _context.STask
                .Where(x => x.AssignedTo == userIdInt && (x.Status == 0 || x.Status == Status.InProgress) && x.project_id == null) 
                .Select(x => x.Deadline.Date)
                .ToList();
            Data.Add(Deadline);

            return Data;
        }

        [HttpPost]
        public List<object> GetRData(int? AssignedTo, int? CreatedBy)
        {
            try
            {
                List<object> RData = new List<object>();

                // Retrieve task titles
                List<string> labels = _context.Request
                    .Where(x => x.Status == Status.Escalated)
                    .Select(x => x.RequestTitle)
                    .ToList();
                RData.Add(labels);

                // Retrieve start dates
                List<DateTime> startDates = _context.Request
                    .Where(x => x.Status == Status.Escalated)
                    .Select(x => x.StartDate.Date) // Assuming you want only the date part
                    .ToList();
                RData.Add(startDates);

                return RData;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log them, and return an error response
                // For example:
                // return StatusCode(500, "An error occurred while fetching data.");
                throw;
            }
        }
        public List<object> TData(int? AssignedTo, int? CreatedBy)
        {
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            List<object> SData = new List<object>();

            List<string> TaskTitle = _context.STask.Where(x => x.AssignedTo == userIdInt && x.project_id == null) 
                                                   .OrderByDescending(x => x.TaskID)
                                                   .Take(3)
                                                   .Select(x => x.TaskTitle)
                                                   .ToList();
            List<Status> status = _context.STask.Where(x => x.AssignedTo == userIdInt && x.project_id == null) 
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
                    case Status.Confirm:
                        statusText.Add("Confirm");
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
        public List<object> RData(Status? status)
        {
            try
            {
                string userId = HttpContext.Session.GetString("userid");
                if (string.IsNullOrEmpty(userId))
                {
                    // Handle missing user ID
                    return new List<object>(); // Or throw an exception
                }

                int userIdInt = Convert.ToInt32(userId);

                List<object> ReData = new List<object>();

                var escalatedRequests = _context.Request
                    .Where(x => x.Status == Status.Escalated)
                    .OrderByDescending(x => x.RequestID)
                    .Take(3)
                    .ToList();

                List<string> requestTitles = escalatedRequests.Select(x => x.RequestTitle).ToList();
                List<int> createdBy = escalatedRequests.Select(x => x.CreatedBy).ToList();

                ReData.Add(requestTitles);
                ReData.Add(createdBy);

                return ReData;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, log them, and return an empty list or error response
                // For example:
                // return new List<object>();
                throw;
            }
        }


    }
}