using Benday.DemoApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Benday.DemoApp.WebApi.Controllers;

[Route("api/secured")]
[ApiController]
public class SecuredController : ControllerBase
{
    [Authorize]
    [HttpGet("protected")]
    public ActionResult<GetMessageResponse> GetProtectedData()
    {
        var response = new GetMessageResponse();

        response.Message = $"You have accessed a protected endpoint! {DateTime.Now.Ticks}";

        return Ok(response);
    }

    [Authorize(Roles = "administrator")]
    [HttpGet("protected-admin")]
    public ActionResult<GetMessageResponse> GetAdminOnlyProtectedData()
    {
        var response = new GetMessageResponse();

        response.Message = $"You have accessed a protected endpoint and get admin only data! {DateTime.Now.Ticks}";

        return Ok(response);
    }
}