using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoWE.Data;
using DemoWE.Models;
using System.IO;
using Microsoft.AspNetCore.Http;


namespace DemoWE.Controllers
{
    public class STasksController : Controller
    {
        private readonly DemoWEContext _context;

        public STasksController(DemoWEContext context)
        {
            _context = context;
        }

        // GET: STasks


        public async Task<IActionResult> Index(int? AssignedTo, int? CreatedBy)
        {
            await HttpContext.Session.LoadAsync();
            HttpContext.Session.Remove("ProjectID");
            var li = await _context.User.ToListAsync();
            ViewBag.At = li;
            // Get the user ID from the session
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");
            await HttpContext.Session.LoadAsync();
            string username = HttpContext.Session.GetString("Username");
            await HttpContext.Session.LoadAsync();
            string deptId = HttpContext.Session.GetString("DepartmentID");
            await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");
            ViewBag.name = username;
            
            // Convert userId to int
            int userIdInt = Convert.ToInt32(userId);
            int deptIdInt = Convert.ToInt32(deptId);
            int roleInt = Convert.ToInt32(role);
           
            ViewBag.userid = userIdInt;
            // Retrieve the tasks that match the AssignedTo ID and the user ID
            var tasks = await _context.STask
            .Where(t => (t.AssignedTo == userIdInt || t.CreatedBy == userIdInt) && t.project_id == null)
            .ToListAsync();
            var di = await _context.User
              .Where(t => t.DepartmentID == deptIdInt)
              .ToListAsync();
            ViewBag.dt = di;
            return View(tasks);
        }
      
        // GET: STasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var li = await _context.User.ToListAsync();
            ViewBag.At = li;
            if (id == null)
            {
                return NotFound();
            }

            var sTask = await _context.STask
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (sTask == null)
            {
                return NotFound();
            }

            return View(sTask);
        }

        //GET: STasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: STasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("TaskID,TaskTitle,Priority,TaskDescription,AssignedTo,project_id,StartDate,Deadline")] STask sTask)
        {
            //file is not working!!
            if (file != null)
            {
                string filename = file.FileName;
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/files"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                {
                    await file.CopyToAsync(filestream);
                }

                sTask.sfile = filename;
            }

            // Get the user ID from the session
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");

            // Get the project ID from the session
            await HttpContext.Session.LoadAsync();
            int? projectId = HttpContext.Session.GetInt32("ProjectID");

            // Use the projectIDValue as needed
            if (projectId.HasValue)
            {
                sTask.project_id = projectId.Value;
            }

            // Convert userId to int
            int userIdInt = Convert.ToInt32(userId);

            sTask.CreatedBy = userIdInt;
            sTask.Status = 0;

            _context.Add(sTask);

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: STasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            await HttpContext.Session.LoadAsync();
            string deptId = HttpContext.Session.GetString("DepartmentID");
            int deptIdInt = Convert.ToInt32(deptId);
            if (id == null)
            {
                return NotFound();
            }

            var sTask = await _context.STask.FindAsync(id);
            if (sTask == null)
            {
                return NotFound();
            }
            var li = await _context.User.ToListAsync();
            ViewBag.At = li;
            var di = await _context.User
             .Where(t => t.DepartmentID == deptIdInt)
             .ToListAsync();
            ViewBag.dt = di;
            return View(sTask);
        }

        // POST: STasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskID,TaskTitle,TaskDescription,sfile,Status,StartDate,Deadline,CreatedBy,AssignedTo,Priority")] STask sTask)
        {
            if (id != sTask.TaskID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!STaskExists(sTask.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sTask);
        }

        // GET: STasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sTask = await _context.STask
                .FirstOrDefaultAsync(m => m.TaskID == id);
            if (sTask == null)
            {
                return NotFound();
            }

            return View(sTask);
        }

        // POST: STasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sTask = await _context.STask.FindAsync(id);
            if (sTask != null)
            {
                _context.STask.Remove(sTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool STaskExists(int id)
        {
            return _context.STask.Any(e => e.TaskID == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var task = await _context.STask.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Assuming Status is an enum, if not, adjust this part accordingly
            if (Enum.TryParse<Status>(status, out Status parsedStatus))
            {
                task.Status = parsedStatus;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Task status updated successfully." });
            }
            else
            {
                return BadRequest("Invalid status value.");
            }
        }




    }
}
