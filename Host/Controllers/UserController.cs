using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Application.Services;
using SchoolApp.Core.Helper;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserDto userDto)
        {
            var response = await _userService.Login(userDto);

            if (response.Data != null)
            {
                    var token = JwtHelper.GenerateToken(response.Data.Email, response.Data.RoleName, response.Data.Id);
                    return Ok(new{
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