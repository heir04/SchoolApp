using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Core.Dto;
using SchoolApp.Core.IServices;

namespace SchoolApp.Core.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        public SubjectService(ISubjectRepository subjectRepository)
        {
           _subjectRepository = subjectRepository;
        }
        public async Task<BaseResponse<SubjectDto>> Create(SubjectDto subjectDto)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _subjectRepository.Get(s => s.Name == subjectDto.Name);
            if (subject != null)
            {
                response.Message = "Subject already exists";
                return response;
            }

            var newsubject = new Subject
            {
                Name = subjectDto.Name,
            };
            await _subjectRepository.Register(newsubject);
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Delete(Guid subjectId)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _subjectRepository.Get(s => s.Id == subjectId);

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
            await _subjectRepository.Update(subject);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<SubjectDto>> Get(Guid id)
        {
            var response = new BaseResponse<SubjectDto>();
            var subject = await _subjectRepository.Get(s => s.Id == id);

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
            var subjects = await _subjectRepository.GetAll();

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
            var subject = await _subjectRepository.Get(a => a.Id == subjectId);
            if (subject is null)
            {
                response.Message = "Subject not found";
                return response;
            }

            subject.Name = subjectDto.Name;
            await _subjectRepository.Update(subject);
            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}