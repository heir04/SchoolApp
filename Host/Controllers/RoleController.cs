using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
         private readonly IRoleService _roleService = roleService;

        [HttpPost("Create")]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            var result = await _roleService.CreateRole(roleDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
