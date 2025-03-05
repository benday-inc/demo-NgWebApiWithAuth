using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Benday.DemoApp.WebApi.Controllers;

[Route("api/secured")]
[ApiController]
public class SecuredController : ControllerBase
{
    [Authorize]
    [HttpGet("protected")]
    public IActionResult GetProtectedData()
    {
        return Ok(new { 
            message = $"You have accessed a protected endpoint! {DateTime.Now.Ticks}" 
        });
    }
}