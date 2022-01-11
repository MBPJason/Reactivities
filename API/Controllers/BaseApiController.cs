using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Responses gets data in an API format from this controller
    [ApiController]
    // Naming convention for controllers.
    // Example: 'ActivitiesController' GET all route = '/api/activities'
    [Route("api/[controller]")]

    // Class for Controller extending from ControllerBase using AspNetCore.Mvc
    // This will be the base class model for all API controller build layouts
    public class BaseApiController : ControllerBase
    {
        // Sets up Mediator as private field
        private IMediator _medidator;

        // Sets this to either the private field above if available
        // Or if null grabs it from the Mediator Service
        protected IMediator Mediator => _medidator ??= HttpContext.RequestServices
            .GetService<IMediator>();

        // Error result handler
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result == null) return NotFound();
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            if (result.IsSuccess && result.Value == null)
                return NotFound();
            return BadRequest(result.Error);
        }
    }
}