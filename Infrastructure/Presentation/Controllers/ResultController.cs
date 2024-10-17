using Microsoft.AspNetCore.Mvc;
using SchoolApp.Core.Dto;
using SchoolApp.Core.IServices;

namespace SchoolApp.Infrastructure.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        public ResultController(IResultService resultService)
        {
           _resultService = resultService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] ResultDto resultDto, Guid studentId, Guid subjectId, Guid levelId)
        {
            var result = await _resultService.Create(resultDto, studentId, subjectId, levelId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")] 
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var result = await _resultService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllResult([FromRoute]Guid id)
        {
            var result = await _resultService.GetAllResult(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, ResultDto resultDto)
        {
            var result = await _resultService.Update(resultDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _resultService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}