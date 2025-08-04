using System.IdentityModel.Tokens.Jwt;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;

namespace SchoolApp.Application.Services
{
    public class UserService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

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
            await _unitOfWork.SaveChangesAsync();
            response.Message = "User deleted successfully";
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
                user => new UserDto
                {
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
                UserName = user.UserName,
                Email = user.Email
            };
            response.Data = userDto;
            response.Message = "";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<UserDto>> Login(UserDto userDto)
        {
            var response = new BaseResponse<UserDto>();

            var user = await _unitOfWork.User.GetUser(x => x.Email == userDto.Email.ToLower());

            if (user is null)
            {
                var student = await _unitOfWork.Student.Get(s => s.StudentId == userDto.Email.ToLower() && !s.IsDeleted);
                if (student != null)
                {
                    user = await _unitOfWork.User.GetUser(x => x.Id == student.UserId && !x.IsDeleted);
                }
            }

            if (user is null)
            {
                response.Message = "Incorrect email/studentId or password!";
                return response;
            }

            string hashedPassword = HashingHelper.HashPassword(userDto.Password, user.HashSalt);
            if (user.PasswordHash == null || !user.PasswordHash.Equals(hashedPassword))
            {
                response.Message = $"Incorrect email or password!";
                return response;
            }

            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole == null)
            {
                response.Message = "User has no roles assigned";
                return response;
            }
            var role = await _unitOfWork.Role.Get(r => r.Id == userRole.RoleId);
            response.Data = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleName = role.Name
            };
            response.Message = "Welcome";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<UpdateUserPasswordDto>> UpdatePassword(UpdateUserPasswordDto userDto)
        {
            var response = new BaseResponse<UpdateUserPasswordDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var user = await _unitOfWork.User.Get(u => u.Id == userId && !u.IsDeleted);
            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }
            string hashedCurrentPassword = HashingHelper.HashPassword(userDto.CurrentPassword, user.HashSalt);
            if (user.PasswordHash == null || !user.PasswordHash.Equals(hashedCurrentPassword))
            {
                response.Message = "Current password is incorrect";
                return response;
            }

            string saltString = HashingHelper.GenerateSalt();
            string hashedPassword = HashingHelper.HashPassword(userDto.NewPassword, saltString);
            user.HashSalt = saltString;
            user.PasswordHash = hashedPassword;
            user.LastModifiedBy = user.Id;
            user.LastModifiedBy = user.Id;

            await _unitOfWork.SaveChangesAsync();
            response.Message = "Password Updated";
            response.Status = true;
            return response;
        }
    }
}