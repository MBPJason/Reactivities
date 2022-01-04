using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    // Activity Controller using the BaseApiController as the class model
    public class ActivitiesController : BaseApiController
    {
        // Set up as private read only endpoints
        private readonly DataContext _context;
        public ActivitiesController(DataContext context)
        {
            _context = context;
        }

        // Endpoints for ActivitiesController

        // Gets all activities endpoint. Returns the data in a list format
        // Example: "/api/activities"
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities() {
            return await _context.Activities.ToListAsync();
        }

        // Gets a specific activity by provided id. Adds the additional parameter at the end of the route. 
        // Example: "/api/activities/{id}"
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id) {
            return await _context.Activities.FindAsync(id);
        }
    }
}