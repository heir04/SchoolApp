using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IResultService
    {
        Task<BaseResponse<ResultDto>> Create(ResultDto resultDto, Guid studentId);
        Task<BaseResponse<IEnumerable<StudentResultStatusDto>>> CreateResultsForLevel(ResultDto resultDto, Guid levelId);
        Task<BaseResponse<ResultDto>> Update(ResultDto resultDto, Guid resultId, Guid subjectId);
        Task<BaseResponse<ResultDto>> Get(Guid id);
        Task<BaseResponse<ResultDto>> Delete(Guid resultId);
        Task<BaseResponse<ResultDto>> CheckResult();
        Task<BaseResponse<IEnumerable<ResultDto>>> GetAllResult(Guid subjectId);
    }
}