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
    public class STasksController : Controller
    {
        private readonly DemoWEContext _context;

        public STasksController(DemoWEContext context)
        {
            _context = context;
        }

        // GET: STasks
        public async Task<IActionResult> Index(int? AssignedTo)
        {
            // Get the user ID from the session


            string userId = HttpContext.Session.GetString("userid");

            // Convert userId to int
            int userIdInt = Convert.ToInt32(userId);

            // Retrieve the tasks that match the AssignedTo ID and the user ID
            var tasks = await _context.STask
                .Where(t => t.AssignedTo == userIdInt)
                .ToListAsync();

            return View(tasks);
        }

        // GET: STasks/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: STasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: STasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskID,TaskTitle,Priority,TaskDescription,sfile,AssignedTo,CreatedBy,StartDate,Deadline")] STask sTask)
        {
            sTask.Status = 0;

            _context.Add(sTask);
            await _context.SaveChangesAsync();

            // Return the created task as a partial view
            return PartialView("_CreatePartial", new STask());
        }

        // GET: STasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sTask = await _context.STask.FindAsync(id);
            if (sTask == null)
            {
                return NotFound();
            }
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
    }


}
