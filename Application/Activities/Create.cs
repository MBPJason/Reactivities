using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    // The Create part of 'CRUD' for our API.
    public class Create
    {
        // Created a command. (return value: Result<Unit>)
        public class Command : IRequest<Result<Unit>>
        {
            // Expects an Activity to be the value format.
            public Activity Activity { get; set; }
        }

        // Validation for our request commands
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        // The Handler for our requests. It expects to receive a command as the request.
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            // Essentially the call to persistence(db).
            // So we can persist/set our changes in our database/Entity.
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            // Interface for our command request.
            // It receives a command request to add an Activity
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Matches the user making the request from the jwt through IUserAccessor
                var user = await _context.Users.FirstOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetUsername());
                // Assigns the user as Host, and constructs the full object of info
                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true
                };
                // Adds the object of info to the ActivityAttendee table
                request.Activity.Attendees.Add(attendee);
                // Then sends that request to the persistence layer to add.
                _context.Activities.Add(request.Activity);
                // Saves changes the database and sets true or false of the variable.
                var result = await _context.SaveChangesAsync() > 0;
                // If no changes are saved(variable is false) send Failure response
                if (!result) return Result<Unit>.Failure("Failed to create activity");
                // The returns a 'void' unit when done to show completion.
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}