using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface ILevelService
    {
        Task<BaseResponse<LevelDto>> Create(LevelDto levelDto);
        Task<BaseResponse<LevelDto>> Update(LevelDto levelDto, Guid levelId);
        Task<BaseResponse<LevelDto>> Delete(Guid levelId);
        Task<BaseResponse<IEnumerable<LevelDto>>> GetAll();
    }
}