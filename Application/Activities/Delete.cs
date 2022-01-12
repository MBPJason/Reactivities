using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities
{
    // The delete method for our CRUD application
    public class Delete
    {
        // The expected format of the request we expect to receive. (return value: Result<Unit>)
        public class Command : IRequest<Result<Unit>>
        {
            // Format of expected value
            public Guid Id { get; set; }
        }

        // The handler expects to receive a command and return a Result with a Unit value
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            // The persistence layer(database to query)
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            // Main function to excute what happens when we receive a request
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Match the activity to an activity in the database with an activity guid id
                var activity = await _context.Activities.FindAsync(request.Id);
                // Remove it from the database
                _context.Remove(activity);
                // Save any changes
                var result = await _context.SaveChangesAsync() > 0;
                // If there is no changes to be save send a Failure response(This is also a check for null responses too)
                if (!result) return Result<Unit>.Failure("Failed to delete the activity");
                // If there are any changes that are saved then send a Success response
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}