using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GreetingController : ControllerBase
{
    /// <summary>
    /// Returns a greeting message from the API.
    /// </summary>
    [HttpGet]
    public Greeting Get()
    {
        return new Greeting
        {
            Message = "Hello from the Backend API",
            ServerTimeUtc = DateTime.UtcNow,
        };
    }
}
