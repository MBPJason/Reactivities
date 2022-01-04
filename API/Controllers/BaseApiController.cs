using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
    }
}