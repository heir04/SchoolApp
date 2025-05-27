using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;
        
        [HttpPost("Register")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromForm] ResultDto resultDto, Guid studentId)
        {
            var result = await _resultService.Create(resultDto, studentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("CreateResults")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateResults([FromForm] ResultDto resultDto)
        {
            var result = await _resultService.CreateBulkResults(resultDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        [Authorize(Roles = "Admin, Teacher, Student")]
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var result = await _resultService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [HttpGet("CheckResult")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CheckResult()
        {
            var result = await _resultService.CheckResult();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> GetAllResult([FromRoute]Guid subjectId)
        {
            var result = await _resultService.GetAllResult(subjectId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Update([FromRoute]Guid id, Guid subjectId, ResultDto resultDto)
        {
            var result = await _resultService.Update(resultDto, id, subjectId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _resultService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}