using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponse<UserDto>> Delete(Guid id)
        {
            var response = new BaseResponse<UserDto>();
            var user = await _unitOfWork.User.Get(u => u.Id == id);

            if (user is null)
            {
                response.Message = "User not found";
                return response;
            }

            user.IsDeleted = true;
            user.IsDeleteBy = user.Id;
            user.IsDeleteOn = DateTime.Now;
            await _unitOfWork.User.Update(user);
            response.Message = "";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<UserDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<UserDto>>();

            var users = await _unitOfWork.User.GetAll();
            if (users is null)
            {
                response.Message = "No users found";
                return response;
            }

            response.Data = users.Select(
                user => new UserDto{
                   // UserName = user.UserName,
                   Email = user.Email,         
                }).ToList();
            response.Status = true;
            response.Message = "";
            return response;
        }

        public async Task<BaseResponse<UserDto>> GetUser(Guid id)
        {
            var response = new BaseResponse<UserDto>();
            var user = await _unitOfWork.User.Get(u => u.Id == id);
            if (user is null)
            {
                response.Message = "No user found";
                return response;
            }

            var userDto = new UserDto
            {
                // UserName = user.UserName,
                Email = user.Email
            };
            response.Data = userDto;
            response.Message = "";
            response.Status = true;
            return response;
        }

        public Task<BaseResponse<UserDto>> Register(UserDto userDto)
        {
            var response = new BaseResponse<UserDto>();
            var getUser =  _unitOfWork.User.ExistsAsync(x => x.Email == userDto.Email);
            var user = new User
            {
               // UserName = userDto.Email,
               Email = userDto.Email
            };
            throw new NotImplementedException();
        }
        
        // Password to be hashed later
        public async Task<BaseResponse<UserDto>> UpdatePassword(UserDto userDto, Guid id)
        {
            var response = new BaseResponse<UserDto>();

            var user = await _unitOfWork.User.Get(u => u.Id == id);
            if (user is null)
            {
                response.Message = "User not found";
                return response;
            }

            user.Password = userDto.Password;
            user.LastModifiedBy = user.Id;
            await _unitOfWork.User.Update(user);
            response.Message = "";
            response.Status = true;
            return response;
        }
    }
}