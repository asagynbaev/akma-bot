using Microsoft.AspNetCore.Mvc;

namespace MindMate.Controllers;

[ApiController]
[Route("[controller]")]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> _logger;

    public MainController(ILogger<MainController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "ping")]
    public string Get()
    {
        return "Hello from MindMate";
    }
}

