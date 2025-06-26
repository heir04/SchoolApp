using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController(ITeacherService teacherService) : ControllerBase
    {
        private readonly ITeacherService _teacherService = teacherService;
        
        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] TeacherDto teacherDto)
        {
            var result = await _teacherService.Register(teacherDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }
     
        [Authorize(Roles = "Admin")]   
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute]Guid id)
        {
            var result = await _teacherService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get")]
        public async Task<IActionResult> Get(string email)
        {
            var result = await _teacherService.Get(email);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetByCurrentUserId()
        {
            var result = await _teacherService.GetByCurrentUserId();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _teacherService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Count")]
        public async Task<IActionResult> Count()
        {
            var result = await _teacherService.Count();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, TeacherDto teacherDto)
        {
            var result = await _teacherService.Update(teacherDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _teacherService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}