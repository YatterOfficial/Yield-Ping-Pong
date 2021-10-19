using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PingPongController : ControllerBase
{
    public static string Name { get; set;}
    private readonly ILogger<PingPongController> _logger;

    public PingPongController(ILogger<PingPongController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string Get(string name)
    {

        // All this does is send the Server's ball back to the other 
        // side so that he can send it over again.
        
        // A ping responds with a pong, a pong with a ping,
        // anything else is a stranger or imposter

        if(name.Equals("ping")&&Name.Equals("pong"))
        {
            if(PingPongTable.Server.Equals("pong"))
            {
                return "not a game";
            }
            return "pong";
        }
        else if(name.Equals("pong")&&Name.Equals("ping"))
        {
            if(PingPongTable.Server.Equals("ping"))
            {
                return "not a game";
            }
            return "ping";
        }

        return "stranger";
    }
}
