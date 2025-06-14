using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IResultService
    {
        Task<BaseResponse<ResultDto>> Create(ResultDto resultDto, Guid studentId);
        Task<BaseResponse<IEnumerable<StudentResultStatusDto>>> CreateBulkResults(BulkResultDto resultDto);
        Task<BaseResponse<ResultDto>> Update(ResultDto resultDto, Guid resultId, Guid subjectId);
        Task<BaseResponse<ResultDto>> Get(Guid id);
        Task<BaseResponse<IEnumerable<StudentDto>>> GetStudentsByLevel(Guid levelId, Guid subjectId);
        Task<BaseResponse<ResultDto>> Delete(Guid resultId);
        Task<BaseResponse<ResultDto>> CheckResult();
        Task<BaseResponse<GiveResultRemarkDto>> GiveRemark(Guid resultId, GiveResultRemarkDto remarkDto);
        Task<BaseResponse<IEnumerable<ResultDto>>> GetAllResult(Guid subjectId);
        Task<BaseResponse<IEnumerable<ResultDto>>> GetAllResultByLevel(Guid levelId);
        Task<BaseResponse<ResultRemarkCountDto>> GetResultsRemarkCounts(Guid? levelId = null);
        Task<BaseResponse<IEnumerable<ResultDto>>> GetAllResultByCurrentUserId();
    }
}