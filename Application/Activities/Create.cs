using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    // The Create part of 'CRUD' for our API.
    public class Create
    {
        // Created a command. Doesn't return a value.
        public class Command : IRequest
        {
            // Expects an Activity to be value being set.
            public Activity Activity { get; set; }
        }

        // The Handler for our requests. It expects to receive a command as the request.
        public class Handler : IRequestHandler<Command>
        {
            // Essentially the call to persistence(db migration).
            // So we can persist/set our changes in our database.
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            // Interface for our command request.
            // It receives a command request to add an Activity
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // Then sends that request to the persistence layer to add.
                _context.Activities.Add(request.Activity);

                // Saves the new addition to the database.
                await _context.SaveChangesAsync();

                // The returns a 'void' unit when done to show completion.
                return Unit.Value;
            }
        }
    }
}