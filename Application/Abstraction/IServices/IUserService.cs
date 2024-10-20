using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IUserService
    {
        Task<BaseResponse<UserDto>> Register(UserDto userDto);
        Task<BaseResponse<UserDto>> UpdatePassword(UserDto userDto, Guid id);
        Task<BaseResponse<UserDto>> Delete(Guid id);
        Task<BaseResponse<UserDto>> GetUser(Guid id);
        Task<BaseResponse<IEnumerable<UserDto>>> GetAll();
    }
}