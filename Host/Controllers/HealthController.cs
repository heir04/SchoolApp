using Microsoft.AspNetCore.Mvc;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Message = "SchoolApp is running successfully on Railway!" 
            });
        }
        
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(new { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Message = "SchoolApp is running successfully on Railway!" 
            });
        }

        [HttpGet("database")]
        public IActionResult DatabaseHealth([FromServices] SchoolApp.Infrastructure.Context.ApplicationContext context)
        {
            try
            {
                // Simple database connectivity test
                var canConnect = context.Database.CanConnect();
                return Ok(new { 
                    DatabaseStatus = canConnect ? "Connected" : "Disconnected",
                    Timestamp = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    DatabaseStatus = "Error", 
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow 
                });
            }
        }
    }
}
