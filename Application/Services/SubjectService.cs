using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Helper;
using Microsoft.IdentityModel.JsonWebTokens;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Application.Services
{
    public class SubjectService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ValidatorHelper validator) : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ValidatorHelper _validator = validator;

        public async Task<BaseResponse<SubjectDto>> Create(SubjectDto subjectDto)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _unitOfWork.Subject.ExistsAsync(s => s.Name == subjectDto.Name && s.IsDeleted == false);
            if (subject)
            {
                response.Message = "Subject already exists";
                return response;
            }

            var ValidateCategory = _validator.ValidateCategory(subjectDto.Category);
            if (!ValidateCategory)
            {
                response.Message = "Invalid Category";
                return response;
            }

            var newsubject = new Subject
            {
                Name = subjectDto.Name,
                Category = subjectDto.Category
            };
            await _unitOfWork.Subject.Register(newsubject);
            await _unitOfWork.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Delete(Guid subjectId)
        {
            var response = new BaseResponse<SubjectDto>();
            var subjectExist = await _unitOfWork.Subject.ExistsAsync(s => s.Id == subjectId && s.IsDeleted == false);
            if (!subjectExist)
            {
                response.Message = "Not found";
                return response;
            }

            var subject = await _unitOfWork.Subject.Get(s => s.Id == subjectId);

            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            if (subject.IsDeleted == true)
            {
                response.Message = "Subject already deleted";
                return response;
            }

            subject.IsDeleted = true;
            await _unitOfWork.Subject.Update(subject);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Get(Guid id)
        {
            var response = new BaseResponse<SubjectDto>();
            var subjectExist = await _unitOfWork.Subject.ExistsAsync(s => s.Id == id && s.IsDeleted == false);
            if (!subjectExist)
            {
                response.Message = "Not found";
                return response;
            }
            var subject = await _unitOfWork.Subject.Get(s => s.Id == id);

            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            var subjectDto = new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Category = subject.Category
            };

            response.Data = subjectDto;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<SubjectDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<SubjectDto>>();
            var subjects = await _unitOfWork.Subject.GetAll();

            if (subjects is null)
            {
                response.Message = "No subject registered";
                return response;
            }

            var subjectDtos = subjects
            .Where(s => s.IsDeleted == false)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Category = s.Category
            }).ToList();

            response.Data = subjectDtos;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Update(SubjectDto subjectDto, Guid subjectId)
        {
            var response = new BaseResponse<SubjectDto>();
            var subjectExist = await _unitOfWork.Subject.ExistsAsync(s => s.Id == subjectId && s.IsDeleted == false);
            if (!subjectExist)
            {
                response.Message = "Not found";
                return response;
            }

            var subject = await _unitOfWork.Subject.Get(a => a.Id == subjectId);
            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            var ValidateCategory = _validator.ValidateCategory(subjectDto.Category);
            if (!ValidateCategory)
            {
                response.Message = "Invalid Category";
                return response;
            }

            subject.Name = subjectDto.Name;
            subject.Category = subjectDto.Category;
            await _unitOfWork.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<SubjectDto>>> GetSubjectsByTeacher()
        {
            var response = new BaseResponse<IEnumerable<SubjectDto>>();

            // Get user ID from JWT
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.GetTeacher(t => t.UserId == userId);
            if (teacher == null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var subjects = teacher.TeacherSubjects.Select(ts => new SubjectDto
            {
                Id = ts.SubjectId,
                Name = ts.Subject.Name
            }).ToList();

            if (!subjects.Any())
            {
                response.Message = "No subjects assigned to this teacher";
                return response;
            }

            response.Data = subjects;
            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}