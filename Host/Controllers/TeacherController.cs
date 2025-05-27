using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController(ITeacherService teacherService) : ControllerBase
    {
        private readonly ITeacherService _teacherService = teacherService;
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] TeacherDto teacherDto)
        {
            var result = await _teacherService.Register(teacherDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var result = await _teacherService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(string email)
        {
            var result = await _teacherService.Get(email);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teacherService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, TeacherDto teacherDto)
        {
            var result = await _teacherService.Update(teacherDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _teacherService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}