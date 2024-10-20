using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Infrastructure.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService) 
        {
            _roleService = roleService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            var result = await _roleService.CreateRole(roleDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
