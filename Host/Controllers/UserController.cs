using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Helper;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, JwtHelper jwtHelper) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly JwtHelper _jwtHelper = jwtHelper;
        
        // public UserController(IUserService userService)
        // {
        //     _userService = userService;
        // }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {
            var response = await _userService.Login(userDto);
            // return response.Status ? Ok(response) : BadRequest(response);

            if (response.Data != null)
            {
                    var token = _jwtHelper.GenerateToken(response.Data.Email, response.Data.RoleName, response.Data.Id);
                    return  Ok(new{
                    Token = token
                    });
            }
            return Unauthorized(response.Message);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // For client-side logout, you can simply return a success response
            // The client should handle the removal of the token from storage
            return Ok(new { Message = "Logged out successfully" });
        }

    }
}