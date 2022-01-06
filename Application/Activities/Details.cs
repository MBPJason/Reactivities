using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    // The Read part of 'CRUD' for our API. For a single selected Activity
    public class Details
    {
        // Receive a query request. Expecting it receive a Guid Id
        public class Query : IRequest<Activity> {
            public Guid Id { get; set; }
        }

        // A handler for our query requests.
        public class Handler : IRequestHandler<Query, Activity>
        {  
        // Essentially the call to persistence(db migration).
        private readonly DataContext _context;
            public Handler(DataContext context)
            {
            _context = context;
            }

            // Handler interface for our Query Request
            // Excutes a Task expecting a return value of an Activity
            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                // Gets request in the form of a query quid Id,
                // Queries persistence to match id to activity,
                // Return matched Activity
               return await _context.Activities.FindAsync(request.Id);
            }
        }
    }
}