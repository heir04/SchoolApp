using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace SchoolApp.Host.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(IStudentService studentService) : ControllerBase
    {
        private readonly IStudentService _studentService = studentService;

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] StudentDto studentDto)
        {
            var result = await _studentService.Register(studentDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _studentService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetByStudentId")]
        public async Task<IActionResult> GetByStudentId(string studentId)
        {
            var result = await _studentService.GetByStudentId(studentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get")]
        public async Task<IActionResult> Get(string email)
        {
            var result = await _studentService.Get(email);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _studentService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }        

        [HttpGet("GetAll/{levelId}")]
        public async Task<IActionResult> GetAll(Guid levelId)
        {
            var result = await _studentService.GetAll(levelId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, StudentDto studentDto)
        {
            var result = await _studentService.Update(studentDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var result = await _studentService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}