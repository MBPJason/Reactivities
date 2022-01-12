using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Activity Controller using the BaseApiController as the class model
    public class ActivitiesController : BaseApiController
    {

        // Endpoints for ActivitiesController
        // A function for Error handlering was put in and the expect return values of it
        // is a Task<IActionResult>

        // Gets all activities endpoint. Returns the data in a list format
        // Example: "/api/activities"
        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            return HandleResult(await Mediator.Send(new List.Query()));
        }

        // Gets a specific activity by provided id. Adds the additional parameter at the end of the route. 
        // Example: "/api/activities/{id}"
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        // A post request to add an activity.
        // Automatically looks for the new activity in the body of the response 
        // and matches it values against the Activity table.
        [HttpPost]
        public async Task<IActionResult> CreateActivity(Activity activity)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        // Update/Edit Endpoint
        // Takes an "id" parameter then set's it as the activity id 
        // Then sends down the activity down the Edit command
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, Activity activity)
        {
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        // Delete Endpoint
        // Takes an "id" paramater and sends that down to the Delete command 
        [HttpDelete("{id}")]
        public async Task<IActionResult> EditActivity(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}