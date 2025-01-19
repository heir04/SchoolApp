using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

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
        
        [Authorize(Roles = "Teacher")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] ResultDto resultDto, Guid studentId)
        {
            var result = await _resultService.Create(resultDto, studentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")] 
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var result = await _resultService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles = "Student")]
        [HttpGet("Get")] 
        public async Task<IActionResult> CheckResult()
        {
            var result = await _resultService.CheckResult();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllResult([FromRoute]Guid id)
        {
            var result = await _resultService.GetAllResult(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, Guid subjectId, ResultDto resultDto)
        {
            var result = await _resultService.Update(resultDto, id, subjectId);
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