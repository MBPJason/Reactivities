using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    // Our setup using EntityFrameWork to establish a session with our db.
    // Essentially making this our virtual representation of db to our API for CRUD applications.
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // This is called when we do any CRUD request with the Activity table.
        public DbSet<Activity> Activities { get; set; }
    }
}