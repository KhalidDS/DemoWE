using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoWE.Data;
using DemoWE.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace DemoWE.Controllers
{
    public class RequestsController : Controller
    {
        private readonly DemoWEContext _context;

        public RequestsController(DemoWEContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the list of requests based on the specified criteria.
        /// </summary>
        /// int CreatedBy The ID of the user who created the requests.</param>
        /// <returns>The list of requests.</returns>
        public async Task<IActionResult> Index(int? CreatedBy)
        {
            // Get the user ID from the session
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);
        
            // Retrieve the user's information from the database
            var user = await _context.User.FindAsync(userIdInt);
            await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");
            int roleInt = Convert.ToInt32(role);
            

            IQueryable<Request> requests;

            // Check if the user is a manager (Role == 2)
            if (user.Role == 2)
            {
                // If the user is a manager, filter requests by their department ID
                requests = _context.Request.Where(t => t.AssignedDepartmentID == user.DepartmentID);
            }
            else if (user.Role == 1)
            {
                // If the user is not a manager but has role 1, filter requests by the user who created them
                requests = _context.Request.Where(t => t.CreatedBy == userIdInt);
            }
            else if (user.Role == 0)
            {
                // If the user has role 0, filter requests by the escalated status
                requests = _context.Request.Where(t => t.Status == Status.Escalated);
            }
            else
            {
                // If the user has an unknown role, return an empty list of requests
                requests = Enumerable.Empty<Request>().AsQueryable();
            }

            // Convert the filtered requests to a list and pass it to the view
            return View(await requests.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id ,Request r)
        {
           
            var li = await _context.User.ToListAsync();
            ViewBag.At = li;
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FirstOrDefaultAsync(m => m.RequestID == id);
            if (request == null)
            {
                return NotFound();
            }

            // Get the user ID from the session
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);

            // Retrieve the user's information from the database
            var user = await _context.User.FindAsync(userIdInt);
           
            // Pass the user's role to the view
            ViewData["UserRole"] = user.Role;
            ViewData["Userid"] = user.EmployeeNumber;
            ViewData["Createdby"] = r.CreatedBy;
            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestID,RequestTitle,RequestDescription,rfile,Status,StartDate,CreatedBy,Deadline,AssignedDepartmentID")] Request request)
        {
            await HttpContext.Session.LoadAsync();
            string userId = HttpContext.Session.GetString("userid");
            int userIdInt = Convert.ToInt32(userId);
            await HttpContext.Session.LoadAsync();
            string role = HttpContext.Session.GetString("Role");
            int roleInt = Convert.ToInt32(role);
          

            // Retrieve the user's information from the database
            var user = await _context.User.FindAsync(userIdInt);
            request.AssignedDepartmentID = user.DepartmentID;

            // Set CreatedBy to the user's ID
            request.CreatedBy = userIdInt;
            request.StartDate = DateTime.Now;
            if (roleInt == 2)
            {
                request.Status = Status.Escalated;
            }


            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }
           
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestID,RequestTitle,RequestDescription,rfile,Status,StartDate,Deadline")] Request request)
        {
            if (id != request.RequestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.RequestID))
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
            return View(request);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Request
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Request.FindAsync(id);
            if (request != null)
            {
                _context.Request.Remove(request);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Request.Any(e => e.RequestID == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var request = await _context.Request.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            // Parse the string status to Status enum
            if (Enum.TryParse<Status>(status, out Status parsedStatus))
            {
                // Update the status
                request.Status = parsedStatus;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Request status updated successfully." });
            }
            else
            {
                return BadRequest("Invalid status value.");
            }
        }


    }
}