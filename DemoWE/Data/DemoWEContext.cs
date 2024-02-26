using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemoWE.Models;

namespace DemoWE.Data
{
    public class DemoWEContext : DbContext
    {
        public DemoWEContext (DbContextOptions<DemoWEContext> options)
            : base(options)
        {
        }

        public DbSet<DemoWE.Models.STask> STask { get; set; } = default!;
        public DbSet<DemoWE.Models.User> User { get; set; } = default!;
    }
}
