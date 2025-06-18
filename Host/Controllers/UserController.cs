using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, JwtHelper jwtHelper) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly JwtHelper _jwtHelper = jwtHelper;

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {
            var response = await _userService.Login(userDto);
            if (response.Status == false || response.Data == null)
            {
                return BadRequest(response);
            }

            if (response.Data != null)
            {
                    var token = _jwtHelper.GenerateToken(response.Data.Email, response.Data.RoleName, response.Data.Id);
                    return  Ok(new{
                    Token = token
                    });
            }
            return Unauthorized(response.Message);
        }

        [HttpPut("UpdatePassword")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(UpdateUserPasswordDto userDto)
        {
            var response = await _userService.UpdatePassword(userDto);
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            return Ok(new { Message = "Logged out successfully" });
        }

    }
}