using System.IdentityModel.Tokens.Jwt;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Helper;

namespace SchoolApp.Application.Services
{
    public class AssignmentService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ValidatorHelper validatorHelper) : IAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ValidatorHelper _validatorHelper = validatorHelper;

        public async Task<BaseResponse<AssignmentDto>> Create(AssignmentDto assignmentDto)
        {
            var response = new BaseResponse<AssignmentDto>();

            // Validate user
            var userValidation = _validatorHelper.ValidateUser();
            if (!userValidation.IsValid)
            {
                response.Message = userValidation.ErrorMessage;
                return response;
            }

            // Validate teacher
            var teacherValidation = await _validatorHelper.ValidateTeacherAsync(userValidation.UserId);
            if (!teacherValidation.IsValid)
            {
                response.Message = teacherValidation.ErrorMessage;
                return response;
            }

            // Validate session and term
            var sessionTermValidation = await _validatorHelper.ValidateCurrentSessionAndTermAsync();
            if (!sessionTermValidation.IsValid)
            {
                response.Message = sessionTermValidation.ErrorMessage;
                return response;
            }

            // Validate subject
            var subjectValidation = await _validatorHelper.ValidateSubjectAsync(assignmentDto.SubjectId);
            if (!subjectValidation.IsValid)
            {
                response.Message = subjectValidation.ErrorMessage;
                return response;
            }

            // Validate level
            var levelValidation = await _validatorHelper.ValidateLevelAsync(assignmentDto.LevelId);
            if (!levelValidation.IsValid)
            {
                response.Message = levelValidation.ErrorMessage;
                return response;
            }

            var assignment = new Assignment
            {
                Content = assignmentDto.Content,
                Level = levelValidation.Level,
                LevelId = levelValidation.Level!.Id,
                Session = sessionTermValidation.Session,
                SessionId = sessionTermValidation.Session!.Id,
                Subject = subjectValidation.Subject,
                SubjectId = subjectValidation.Subject!.Id,
                TeacherId = teacherValidation.Teacher!.Id,
                Teacher = teacherValidation.Teacher,
                Term = sessionTermValidation.Term,
                TermId = sessionTermValidation.Term!.Id,
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

            // Validate user
            var userValidation = _validatorHelper.ValidateUser();
            if (!userValidation.IsValid)
            {
                response.Message = userValidation.ErrorMessage;
                return response;
            }

            // Validate teacher
            var teacherValidation = await _validatorHelper.ValidateTeacherAsync(userValidation.UserId);
            if (!teacherValidation.IsValid)
            {
                response.Message = teacherValidation.ErrorMessage;
                return response;
            }

            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == assignmentId && !a.IsDeleted);
            if (assignment is null)
            {
                response.Message = "Not found";
                return response;
            }

            // Validate ownership
            var ownershipValidation = _validatorHelper.ValidateAssignmentOwnership(assignment, teacherValidation.Teacher!);
            if (!ownershipValidation.IsValid)
            {
                response.Message = ownershipValidation.ErrorMessage;
                return response;
            }

            assignment.IsDeleted = true;
            assignment.IsDeleteBy = teacherValidation.Teacher!.Id;
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
                Content = assignment.Content,
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
            
            // Validate user
            var userValidation = _validatorHelper.ValidateUser();
            if (!userValidation.IsValid)
            {
                response.Message = userValidation.ErrorMessage;
                return response;
            }

            // Validate student
            var studentValidation = await _validatorHelper.ValidateStudentAsync(userValidation.UserId);
            if (!studentValidation.IsValid)
            {
                response.Message = studentValidation.ErrorMessage;
                return response;
            }

            // Validate session and term
            var sessionTermValidation = await _validatorHelper.ValidateCurrentSessionAndTermAsync();
            if (!sessionTermValidation.IsValid)
            {
                response.Message = sessionTermValidation.ErrorMessage;
                return response;
            }
            
            var assignments = await _unitOfWork.Assignment.GetAllAssignments(a => a.LevelId == studentValidation.Student!.LevelId && a.TermId == sessionTermValidation.Term!.Id && !a.IsDeleted);
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
            
            // Validate user
            var userValidation = _validatorHelper.ValidateUser();
            if (!userValidation.IsValid)
            {
                response.Message = userValidation.ErrorMessage;
                return response;
            }

            // Validate teacher
            var teacherValidation = await _validatorHelper.ValidateTeacherAsync(userValidation.UserId);
            if (!teacherValidation.IsValid)
            {
                response.Message = teacherValidation.ErrorMessage;
                return response;
            }

            var assignments = await _unitOfWork.Assignment.GetAllAssignments(a => a.TeacherId == teacherValidation.Teacher!.Id && !a.IsDeleted);
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

            // Validate user
            var userValidation = _validatorHelper.ValidateUser();
            if (!userValidation.IsValid)
            {
                response.Message = userValidation.ErrorMessage;
                return response;
            }

            // Validate teacher
            var teacherValidation = await _validatorHelper.ValidateTeacherAsync(userValidation.UserId);
            if (!teacherValidation.IsValid)
            {
                response.Message = teacherValidation.ErrorMessage;
                return response;
            }

            var assignment = await _unitOfWork.Assignment.Get(a => a.Id == assignmentId && !a.IsDeleted);
            if (assignment is null)
            {
                response.Message = "Not found";
                return response;
            }

            // Validate ownership
            var ownershipValidation = _validatorHelper.ValidateAssignmentOwnership(assignment, teacherValidation.Teacher!);
            if (!ownershipValidation.IsValid)
            {
                response.Message = ownershipValidation.ErrorMessage;
                return response;
            }

            // Validate due date
            var dueDateValidation = _validatorHelper.ValidateAssignmentDueDate(assignment);
            if (!dueDateValidation.IsValid)
            {
                response.Message = dueDateValidation.ErrorMessage;
                return response;
            }

            assignment.Content = assignmentDto.Content;
            assignment.Instructions = assignmentDto.Instructions;
            assignment.DueDate = assignmentDto.DueDate;
            assignment.LastModifiedBy = teacherValidation.Teacher!.Id;
            assignment.LastModifiedOn = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}