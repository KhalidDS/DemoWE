using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoWE.Data;
using DemoWE.Models;

namespace DemoWE.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly DemoWEContext _context;
        private readonly DemoWEContext _staskContext;

        public ProjectsController(DemoWEContext context, DemoWEContext staskContext)
        {
            _context = context;
            _staskContext = staskContext;
        }

        // GET: Projects
        public async Task<IActionResult> Index(int? AssignedDepartmentID)
        {
            

            var li = await _context.Department.ToListAsync();
            ViewBag.dep = li;
            // Get the user ID from the session
            await HttpContext.Session.LoadAsync();
            string DeptID = HttpContext.Session.GetString("DepartmentID");
             await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");
            // Convert userId to int
            int userIdInt = Convert.ToInt32(DeptID);
            int roleInt = Convert.ToInt32(role);
            ViewBag.Role = roleInt;
            // Retrieve the tasks that match the AssignedTo ID and the user ID
            var pt = await _context.Project_1
                .Where(t => t.AssignedDepartmentID == userIdInt )
                .ToListAsync();

            return View(pt);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
             await HttpContext.Session.LoadAsync();
            string deptId = HttpContext.Session.GetString("DepartmentID");
            await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");

            int deptIdInt = Convert.ToInt32(deptId);
            int roleInt = Convert.ToInt32(role);
            ViewBag.Role = roleInt;
            var li = await _context.User.ToListAsync();
            ViewBag.At = li;
            var di = await _context.User
            .Where(t => t.DepartmentID == deptIdInt)
            .ToListAsync();
            ViewBag.dt = di;
            if (id == null)
            {
                return NotFound();
            }

            // Store the ID in session
            HttpContext.Session.SetInt32("ProjectID", id.Value);
             var st = await _staskContext.STask
                .Where(t => t.project_id == id)
                .ToListAsync();
            ViewBag.st = st;
            var project = await _context.Project_1
                .FirstOrDefaultAsync(m => m.ProjectID == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }



        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectID,ProjectTitle,ProjectDescription,StartDate,Deadline,Priority,AssignedDepartmentID")] Project project)
        {
            await HttpContext.Session.LoadAsync();
            string deptId = HttpContext.Session.GetString("DepartmentID");
            ViewBag.asa = deptId;
            int deptIdInt = Convert.ToInt32(deptId);
            project.AssignedDepartmentID = deptIdInt;
            project.Status = 0;
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
          
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project_1.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProjectID,ProjectTitle,ProjectDescription,StartDate,Deadline,Priority,AssignedDepartmentID,Status")] Project project)
        {
            if (id != project.ProjectID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectID))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project_1
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project_1.FindAsync(id);
            if (project != null)
            {
                _context.Project_1.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project_1.Any(e => e.ProjectID == id);
        }

        //public IActionResult CreateRedirect()
        //{
           

        //    return RedirectToAction("Create", "STasks");
        //}

    }
}
