using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoWE.Data;
using DemoWE.Models;
using Microsoft.Data.SqlClient;

namespace DemoWE.Controllers
{
    public class UsersController : Controller
    {
        private readonly DemoWEContext _context;

        public UsersController(DemoWEContext context)
        {
            _context = context;
        }


        public IActionResult login()
        {
            return View();
        }


        [HttpPost, ActionName("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(string na, string pa)
        {
            SqlConnection conn1 = new SqlConnection("Data Source=SQL9001.site4now.net;Initial Catalog=db_aa6a48_we;User Id=db_aa6a48_we_admin;Password=WorkEvo2024");
            string sql;
            sql = "SELECT * FROM [User] WHERE Username = @Username AND Password = @Password";
            SqlCommand comm = new SqlCommand(sql, conn1);
            comm.Parameters.AddWithValue("@Username", na);
            comm.Parameters.AddWithValue("@Password", pa);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                int role = (int)reader["Role"];
                string id = Convert.ToString((int)reader["EmployeeNumber"]);
                HttpContext.Session.SetString("Username", na);
                HttpContext.Session.SetString("Role", role.ToString());
                HttpContext.Session.SetString("userid", id);
                reader.Close();
                conn1.Close();
                if (role == 1)
                    return RedirectToAction("Employee","Home");
                else if (role == 2)
                    return RedirectToAction("Index", "STasks");
                else
                    return RedirectToAction("Admin", "Home");
            }
            else
            {
                ViewData["Message"] = "Wrong username or password";
                return View();
            }
        }
        










        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeNumber,Username,Password,FirstName,LastName,Email,PhoneNumber,DepartmentID")] User user)
        {
            
                 user.Role = 1;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(login));
            }
        

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeNumber,Username,Password,Role,FirstName,LastName,Email,PhoneNumber,DepartmentID")] User user)
        {
            if (id != user.EmployeeNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.EmployeeNumber))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.EmployeeNumber == id);
        }
    }
}
