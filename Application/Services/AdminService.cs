using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationContext _context;
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public AdminService(ApplicationContext context, IAdminRepository adminRepository, IUserRepository userRepository, IRoleRepository roleRepository) 
        {
            _context = context; 
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<BaseResponse<AdminDto>> Delete(Guid id)
        {
            var admin = await _adminRepository.Get(a => a.Id == id);
            if (admin == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "Admin not found",
                    Status = false
                };

            }
            if (admin.IsDeleted == true)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "Admin already Deleted",
                    Status = false
                };
            }
            admin.IsDeleted = true;
            return new BaseResponse<AdminDto>
            {
                Message = "Admin Deleted ",
                Status = true
            };
        }

        public async Task<BaseResponse<AdminDto>> Get(Guid id)
        {
            var admin = await _adminRepository.Get(a => a.Id == id);
            if(admin == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "admin not found",
                    Status = false
                };
            }
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName
            };
            return new BaseResponse<AdminDto>
            {
                Message = "admin gotten",
                Status = true,
                Data = adminDto
            };
        }

        public async Task<BaseResponse<AdminDto>> Get(string email)
        {
            var admin = await _adminRepository.Get(a => a.Email == email);
            if (admin == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "admin not found",
                    Status = false
                };
            }
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName
            };
            return new BaseResponse<AdminDto>
            {
                Message = "admin gotten",
                Status = true,
                Data = adminDto
            };
        }

        public async Task<BaseResponse<IEnumerable<AdminDto>>> GetAll()
        {
            var admins = await _adminRepository.GetAll();
            if (admins == null)
            {
                return new BaseResponse<IEnumerable<AdminDto>>
                {
                    Message = "Not found",
                    Status = false
                };
            }

            var adminDtos = admins.Select(a => new AdminDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email
            }).ToList();
            return new BaseResponse<IEnumerable<AdminDto>>
            {
                Data = adminDtos,
                Message = "list of admins",
                Status = true
            };
        }

        public async Task<BaseResponse<AdminDto>> Register(AdminDto adminDto)
        {
            var getadmin = await _adminRepository.Get(a => a.Email == adminDto.Email);
            if (getadmin != null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "email already exists",
                    Status = false
                };
            }
            var user = new User
            {
               UserName = $"{adminDto.FirstName}{adminDto.LastName}",
                Password = BCrypt.Net.BCrypt.HashPassword(adminDto.Password),
                Email = adminDto.Email
            };
            await _userRepository.Register(user);
            var role = await _roleRepository.Get(r =>r.Name == "Admin");
            if (role == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "Role not found",
                    Status = false
                };
            }
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            _context.UserRoles.Add(userRole);
            var admin = new Admin
            {
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,           
                Email = adminDto.Email?.ToLower(),
                UserId = user.Id
            };
            var addadmin = await _adminRepository.Register(admin);
            admin.CreatedBy = addadmin.Id;
            admin.LastModifiedBy = addadmin.Id;
            admin.IsDeleted = false;

            var adminDTO = new AdminDto
            {
                Id = admin.Id,
                Email = adminDto.Email,
                Password = adminDto.Password,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
            };

            return new BaseResponse<AdminDto>
            { 
                Data = adminDTO, 
                Message = "Admin registered succesfuly",
                Status = true
            };
        }

        public async Task<BaseResponse<AdminDto>> Update(Guid id, AdminDto adminDto)
        {
            var admin = await _adminRepository.Get(a => a.Id == id);
            if (admin == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "admin found",
                    Status = false
                };
            }
            var getUser = await _userRepository.Get(u => u.Email == adminDto.Email);
            if (getUser == null)
            {
                return new BaseResponse<AdminDto>
                {
                    Message = "User not found",
                    Status = false
                };
            }
            getUser.Email = adminDto.Email;
            getUser.Password = adminDto.Password;
            await _userRepository.Update(getUser);

            admin.User.Email = adminDto.Email ?? admin.User.Email;
            admin.User.Password = adminDto.Password ?? admin.User.Password;
            admin.FirstName = adminDto.FirstName ?? admin.FirstName;
            admin.LastName = adminDto.LastName ?? admin.LastName;
            await _adminRepository.Update(admin);

            return new BaseResponse<AdminDto>
            {
                Message = "Admin updated succesfully",
                Status = true
            };
        }
    }
}
