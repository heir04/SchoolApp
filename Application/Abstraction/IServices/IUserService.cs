using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IUserService
    {
        Task<BaseResponse<UserDto>> Login(UserDto userDto);
        Task<BaseResponse<UpdateUserPasswordDto>> UpdatePassword(UpdateUserPasswordDto userDto);
        Task<BaseResponse<UserDto>> Delete(Guid id);
        Task<BaseResponse<UserDto>> GetUser(Guid id);
        Task<BaseResponse<IEnumerable<UserDto>>> GetAll();
    }
}