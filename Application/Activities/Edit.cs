using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        // Gets a command request client to edit/update an already generated activity
        public class Command : IRequest
        {
            // The expect value of what client would send in the request body
            public Activity Activity { get; set; }
        }

        // The Handler for the client's command request.
        public class Handler : IRequestHandler<Command>
        {
            // The call to persistence(database)
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            // The Interface for Handler
            // No value will be returned to the client
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                // Query the database for a specific activity to edit
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                // Pullied in mapper to map request changed values to the activity the id matches
                _mapper.Map(request.Activity, activity);
                // Save changes
                await _context.SaveChangesAsync();
                // Return void unit
                return Unit.Value;
            }
        }
    }
}