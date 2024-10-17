using SchoolApp.Core.Dto;

namespace SchoolApp.Core.IServices
{
    public interface IAdminService
    {
        Task<BaseResponse<AdminDto>> Register(AdminDto adminDto);
        Task<BaseResponse<AdminDto>> Delete(Guid id);
        Task<BaseResponse<AdminDto>> Get(Guid id);
        Task<BaseResponse<AdminDto>> Get(string email);
        Task<BaseResponse<IEnumerable<AdminDto>>> GetAll();
        Task<BaseResponse<AdminDto>> Update(Guid id, AdminDto adminDto);
    }
}