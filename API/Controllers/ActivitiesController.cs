using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers;

// Primary constructor : AppDbContext is injected by the DI container
// Route and [ApiController] attribute are inherited from BaseApiController -> api/activities
public class ActivitiesController(AppDbContext context) : BaseApiController
{
    // GET api/activities
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> GetActivities()
    {
        // Query all activities asynchronously and return them as JSON (200 OK)
        return await context.Activities.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivityDetail(string id)
    {
        var activity = await context.Activities.FindAsync(id);

        if (activity == null) return NotFound();

        return activity;
    }
}