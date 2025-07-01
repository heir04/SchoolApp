using System.IdentityModel.Tokens.Jwt;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Services
{
    public class AssignmentService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public async Task<BaseResponse<AssignmentDto>> Create(AssignmentDto assignmentDto)
        {
            var response = new BaseResponse<AssignmentDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId && !t.IsDeleted);
            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var session = await _unitOfWork.Session.GetCurrentSession();
            if (session is null)
            {
                response.Message = "No session set";
                return response;
            }
            
            var term = session.Terms?.FirstOrDefault(t => t.CurrentTerm == true);
            if (term is null)
            {
                response.Message = "No current term set";
                return response;
            }

            if (assignmentDto.SubjectId == Guid.Empty)
            {
                response.Message = "subject is empty";
                return response;
            }
            var subject = await _unitOfWork.Subject.Get(s => s.Id == assignmentDto.SubjectId);
            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            if (assignmentDto.LevelId == Guid.Empty)
            {
                response.Message = "level is empty";
                return response;
            }
            var level = await _unitOfWork.Level.Get(s => s.Id == assignmentDto.LevelId);
            if (level is null)
            {
                response.Message = "Level not found";
                return response;
            }

            var assignment = new Assignment
            {
                Content = assignmentDto.Content,
                Level = level,
                LevelId = level.Id,
                Session = session,
                SessionId = session.Id,
                Subject = subject,
                SubjectId = subject.Id,
                TeacherId = teacher.Id,
                Teacher = teacher,
                Term = term,
                TermId = term.Id,
                Instructions = assignmentDto.Instructions,
                DueDate = assignmentDto.DueDate
            };

            await _unitOfWork.Assignment.Register(assignment);
            await _unitOfWork.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AssignmentDto>> Delete(Guid assignmentId)
        {
            var response = new BaseResponse<AssignmentDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId);
            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == assignmentId && !a.IsDeleted);

            if (assignment is null)
            {
                response.Message = "Not found";
                return response;
            }

             if (assignment.TeacherId != teacher.Id)
            {
                response.Message = "You are not authorized to update this assignment";
                return response;
            }

            assignment.IsDeleted = true;
            assignment.IsDeleteBy = teacher.Id;
            assignment.IsDeleteOn = DateTime.UtcNow;

            await _unitOfWork.Assignment.SaveChangesAsync();
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;

        }

        public async Task<BaseResponse<AssignmentDto>> Get(Guid id)
        {
            var response = new BaseResponse<AssignmentDto>();

            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == id && !a.IsDeleted);
            if (assignment is null)
            {
                response.Message = "assignment not found";
                return response;
            }

            var assignmentDto = new AssignmentDto
            {
                Id = assignment.Id,
                Instructions = assignment.Instructions,
                Level = assignment.Level?.LevelName,
                Teacher = $"{assignment.Teacher?.FirstName} {assignment.Teacher?.LastName}",
                Subject = assignment.Subject?.Name,
                Session = assignment.Session?.SessionName,
                Term = assignment.Term?.Name,
                DueDate = assignment.DueDate
            };

            response.Data = assignmentDto;
            response.Message = "assignment gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<AssignmentDto>>();

            var assignments = await _unitOfWork.Assignment.GetAllAssignments(a => !a.IsDeleted);
            if (assignments is null || !assignments.Any())
            {
                response.Message = "No assignments found for this student in the current term";
                return response;
            }
            
            var assignmentDto = assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                Content = a.Content,
                Instructions = a.Instructions,
                Level = a.Level?.LevelName,
                Teacher = $"{a.Teacher?.FirstName} {a.Teacher?.LastName}",
                Subject = a.Subject?.Name,
                Session = a.Session?.SessionName,
                Term = a.Term?.Name,
                DueDate = a.DueDate
            }).ToList();

            response.Data = assignmentDto;
            response.Message = "Assignments retrieved successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAllStudentTermAssignment()
        {
            var response = new BaseResponse<IEnumerable<AssignmentDto>>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var student = await _unitOfWork.Student.Get(s => s.UserId == userId);
            if (student is null)
            {
                response.Message = "Student not found";
                return response;
            }

            var session = await _unitOfWork.Session.GetCurrentSession();
            if (session is null)
            {
                response.Message = "No session set";
                return response;
            }
            
            var term = session.Terms?.FirstOrDefault(t => t.CurrentTerm == true);
            if (term is null)
            {
                response.Message = "No current term set";
                return response;
            }
            
            var assignments = await _unitOfWork.Assignment.GetAllAssignments(a => a.LevelId == student.LevelId && a.TermId == term.Id && !a.IsDeleted);
            if (assignments is null || !assignments.Any())
            {
                response.Message = "No assignments found for this student in the current term";
                return response;
            }

            var assignmentDto = assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                Content = a.Content,
                Instructions = a.Instructions,
                Level = a.Level?.LevelName,
                Teacher = $"{a.Teacher?.FirstName} {a.Teacher?.LastName}",
                Subject = a.Subject?.Name,
                Session = a.Session?.SessionName,
                Term = a.Term?.Name,
                DueDate = a.DueDate
            }).ToList();

            response.Data = assignmentDto;
            response.Message = "Assignments retrieved successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAllTeacherAssignment()
        {
            var response = new BaseResponse<IEnumerable<AssignmentDto>>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId);
            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var assignments = await _unitOfWork.Assignment.GetAllAssignments(a => a.TeacherId == teacher.Id && !a.IsDeleted);
            if (assignments is null || !assignments.Any())
            {
                response.Message = "No assignments found for this teacher in the current term";
                return response;
            }

            var assignmentDto = assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                Content = a.Content,
                Instructions = a.Instructions,
                Level = a.Level?.LevelName,
                Teacher = $"{a.Teacher?.FirstName} {a.Teacher?.LastName}",
                Subject = a.Subject?.Name,
                Session = a.Session?.SessionName,
                Term = a.Term?.Name,
                DueDate = a.DueDate
            }).ToList();

            response.Data = assignmentDto;
            response.Message = "Assignments retrieved successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AssignmentDto>> Update(AssignmentDto assignmentDto, Guid assignmentId)
        {
            var response = new BaseResponse<AssignmentDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId);
            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == assignmentId && !a.IsDeleted);
            if (assignment is null)
            {
                response.Message = "Not found";
                return response;
            }

            if (assignment.TeacherId != teacher.Id)
            {
                response.Message = "You are not authorized to update this assignment";
                return response;
            }

            if (assignment.DueDate < DateOnly.FromDateTime(DateTime.Now))
            {
                response.Message = "Can't update after due date";
                return response;
            }

            assignment.Content = assignmentDto.Content;
            assignment.Instructions = assignmentDto.Instructions;
            assignment.DueDate = assignmentDto.DueDate;
            assignment.LastModifiedBy = teacher.Id;
            assignment.LastModifiedOn = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}