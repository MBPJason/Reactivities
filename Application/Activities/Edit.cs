using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        // Gets a command request client to edit/update an already generated activity
        public class Command : IRequest<Result<Unit>>
        {
            // The expect value of what client would send in the request body
            public Activity Activity { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        // The Handler for the client's command request.
        public class Handler : IRequestHandler<Command, Result<Unit>>
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
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Query the database for a specific activity to edit
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                // Check for null
                if (activity == null) return null;
                // Pullied in mapper to map request changed values to the activity the id matches
                _mapper.Map(request.Activity, activity);
                // Set value of any changes
                var result = await _context.SaveChangesAsync() > 0;
                // If no changes send Failure response
                if (!result) return Result<Unit>.Failure("Failed to update activity");
                // If there are any changes send Success response
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}