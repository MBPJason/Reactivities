using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    // The Read part of 'CRUD' for our API. For a single selected Activity
    public class Details
    {
        // Receive a query request. Expecting it receive a Guid Id. Return Value is a Result<ActivityDto>
        public class Query : IRequest<Result<ActivityDto>>
        {
            public Guid Id { get; set; }
        }

        // A handler for our query requests.
        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {  
        // Essentially the call to persistence(db migration).
        private readonly DataContext _context;
        private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
            _mapper = mapper;
            _context = context;
            }

            // Handler interface for our Query Request
            // Excutes a Task expecting a return value of an Activity
            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Gets request in the form of a query quid Id,
                // Queries persistence to match id to activity,
                // Return matched Activity
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}