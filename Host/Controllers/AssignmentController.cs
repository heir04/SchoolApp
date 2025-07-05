using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController(IAssignmentService assignmentService) : ControllerBase
    {
        private readonly IAssignmentService _assignmentService = assignmentService;

        [HttpPost("Create")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([FromForm] AssignmentDto assignmentDto)
        {
            var result = await _assignmentService.Create(assignmentDto);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Delete/{assignmentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(Guid assignmentId)
        {
            var result = await _assignmentService.Delete(assignmentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Get/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _assignmentService.Get(id);
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _assignmentService.GetAll();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAllStudentTermAssignment")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAllStudentTermAssignment()
        {
            var result = await _assignmentService.GetAllStudentTermAssignment();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAllTeacherAssignment")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetAllTeacherAssignment()
        {
            var result = await _assignmentService.GetAllTeacherAssignment();
            return result.Status ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Update/{assignmentId}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Update(AssignmentDto assignmentDto, Guid assignmentId)
        {
            var result = await _assignmentService.Update(assignmentDto, assignmentId);
            return result.Status ? Ok(result) : BadRequest(result);
        }
    }
}