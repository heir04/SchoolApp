using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Infrastructure.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly ILevelService _levelService;
        public LevelController(ILevelService levelService) 
        {
            _levelService = levelService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm]LevelDto levelDto)
        {
            var result = await _levelService.Create(levelDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _levelService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, LevelDto levelDto)
        {
            var result = await _levelService.Update(levelDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _levelService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}
