using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Services
{
    public class SubjectService(IUnitOfWork unitOfWork) : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<BaseResponse<SubjectDto>> Create(SubjectDto subjectDto)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _unitOfWork.Subject.Get(s => s.Name == subjectDto.Name);
            if (subject != null)
            {
                response.Message = "Subject already exists";
                return response;
            }

            var newsubject = new Subject
            {
                Name = subjectDto.Name,
                CreatedOn = DateTime.Today
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
            var subject = await _unitOfWork.Subject.Get(s => s.Id == id);

            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            var subjectDto = new SubjectDto
            {
                Name = subject.Name
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

            var subjectDtos = subjects.Select(s => new SubjectDto{
                Id = s.Id,
                Name = s.Name
            }).ToList();

            response.Data = subjectDtos;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Update(SubjectDto subjectDto, Guid subjectId)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _unitOfWork.Subject.Get(a => a.Id == subjectId);
            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            subject.Name = subjectDto.Name;
            await _unitOfWork.Subject.Update(subject);
            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}