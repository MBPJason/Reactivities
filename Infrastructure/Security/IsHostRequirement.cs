using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    // A class buildout for authorizing certain end points only for the host/creator of the activity
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }
    // The Handler of the class
    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        // Constructor of the handler. Getting access to the database and JWT claim values from auth bearer token pass through http requests
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(DataContext dbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        // The task we are asking the handler to perfrom
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // Find the user identifier from the jwt auth token in memory
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            // If no userID found from db search exit with return
            if (userId == null) return Task.CompletedTask;
            // If user is found grab(parse) the activity ID provided in the url
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());
            // Query database, but doesn't track this in entity(memory), for a match of the user ID and activity ID
            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
                .Result;
            // If no match exit with a return
            if (attendee == null) return Task.CompletedTask;
            // If there is a match, check if they are the host and if they are return with Success
            if (attendee.IsHost) context.Succeed(requirement);
            // If they aren't exit return 
            return Task.CompletedTask;
        }
    }
}