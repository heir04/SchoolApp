using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface ISessionService
    {
        Task<BaseResponse<SessionDto>> Create(SessionDto sessionDto);
        Task<BaseResponse<SessionDto>> Update(SessionDto sessionDto, Guid sessionId);
        Task<BaseResponse<SessionDto>> Delete(Guid sessionId);
        Task<BaseResponse<SessionDto>> Get(Guid id);
        Task<BaseResponse<IEnumerable<SessionDto>>> GetAll();
    }
}