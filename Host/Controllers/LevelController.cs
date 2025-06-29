using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController(ILevelService levelService) : ControllerBase
    {
        private readonly ILevelService _levelService = levelService;

        [HttpPost("Create")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Create([FromForm]LevelDto levelDto)
        {
            var result = await _levelService.Create(levelDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _levelService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Update(Guid id, LevelDto levelDto)
        {
            var result = await _levelService.Update(levelDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _levelService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
