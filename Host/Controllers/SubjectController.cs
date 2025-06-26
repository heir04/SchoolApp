using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Register([FromForm] SubjectDto subjectDto)
        {
            var result = await _subjectService.Create(subjectDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _subjectService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles ="Admin,Teacher")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subjectService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles ="Teacher")]
        [HttpGet("GetSubjectsByTeacher")]
        public async Task<IActionResult> GetSubjectsByTeacher()
        {
            var result = await _subjectService.GetSubjectsByTeacher();
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles ="Admin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, SubjectDto subjectDto)
        {
            var result = await _subjectService.Update(subjectDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _subjectService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}