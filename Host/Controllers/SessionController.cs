using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(ISessionService sessionService) : ControllerBase
    {
        private readonly ISessionService _sessionService = sessionService;

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] SessionDto sessionDto)
        {
            var result = await _sessionService.Create(sessionDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var result = await _sessionService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetCurrentSessionAndTermName")]
        public async Task<IActionResult> GetCurrentSessionAndTermName()
        {
            var result = await _sessionService.GetCurrentSessionAndTermName();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAllSessionTerm")]
        public async Task<IActionResult> GetAllSessionTerm()
        {
            var result = await _sessionService.GetAllSessionTerm();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sessionService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("EndSession")]
        public async Task<IActionResult> EndSession()
        {
            var result = await _sessionService.EndSession();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, SessionDto sessionDto)
        {
            var result = await _sessionService.Update(sessionDto, id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _sessionService.Delete(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("UpdateCurrentTerm/{termId}")]
        public async Task<IActionResult> UpdateCurrentTerm([FromRoute]Guid termId)
        {
            var result = await _sessionService.UpdateCurrentTerm(termId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
    
}