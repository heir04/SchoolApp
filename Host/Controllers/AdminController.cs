using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        private readonly IAdminService _adminService = adminService;

        [HttpPost("Register")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Register([FromForm] AdminDto adminDto)
        {
            var result = await _adminService.Register(adminDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _adminService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetProfile")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetByCurrentUserId()
        {
            var result = await _adminService.GetByCurrentUserId();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Get(string email)
        {
            var result = await _adminService.Get(email);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, AdminDto adminDto)
        {
            var result = await _adminService.Update(id, adminDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _adminService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
