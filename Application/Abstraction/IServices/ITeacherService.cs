using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface ITeacherService
    {
        Task<BaseResponse<TeacherDto>> Register(TeacherDto teacherDto);
        Task<BaseResponse<TeacherDto>> Update(TeacherDto teacherDto, Guid id);
        Task<BaseResponse<TeacherDto>> Delete(Guid id);
        Task<BaseResponse<TeacherDto>> GetByCurrentUserId();
        Task<BaseResponse<TeacherDto>> Get(Guid id);
        Task<BaseResponse<TeacherDto>> Get(string email);
        Task<BaseResponse<IEnumerable<TeacherDto>>> GetAll();
        Task<BaseResponse<DataCountDto>> Count();
    }
}