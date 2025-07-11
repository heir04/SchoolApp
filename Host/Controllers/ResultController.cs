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
        public async Task<IActionResult> CreateResults([FromBody] BulkResultDto bulkResultDto)
        {
            var result = await _resultService.CreateBulkResults(bulkResultDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Teacher,Student")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
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

        [HttpGet("GetAllResult/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllResultByStudentId(string studentId)
        {
            var result = await _resultService.GetAllResultsByStudentId(studentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll/{subjectId}")]
        [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
        public async Task<IActionResult> GetAllResult([FromRoute] Guid subjectId)
        {
            var result = await _resultService.GetAllResult(subjectId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAllByLevel/{levelId}")]
        [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
        public async Task<IActionResult> GetAllResultByLevel([FromRoute] Guid levelId)
        {
            var result = await _resultService.GetAllResultByLevel(levelId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetStudentsByLevel/{levelId}/{subjectId}")]
        [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
        public async Task<IActionResult> GetStudentByLevel([FromRoute] Guid levelId, [FromRoute] Guid subjectId)
        {
            var result = await _resultService.GetStudentsByLevel(levelId, subjectId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Update([FromRoute] Guid id, Guid subjectId, ResultDto resultDto)
        {
            var result = await _resultService.Update(resultDto, id, subjectId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("GiveRemark/{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GiveRemark([FromRoute] Guid id, GiveResultRemarkDto remarkDto)
        {
            var result = await _resultService.GiveRemark(id, remarkDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _resultService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetResultsRemarkCounts/{levelId?}")]
        [Authorize(Roles = "Admin,Teacher,SuperAdmin")]
        public async Task<IActionResult> GetRemarkCounts([FromRoute] Guid? levelId = null)
        {
            var result = await _resultService.GetResultsRemarkCounts(levelId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAllStudentResults")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllResultsByCurrentUserId()
        {
            var result = await _resultService.GetAllResultByCurrentUserId();
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}