﻿using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
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
            var response = new BaseResponse<AdminDto>();
            var admin = await _adminRepository.Get(a => a.Id == id);
            if (admin == null)
            {
                response.Message = "Admin not found";
                return response;
            }
            if (admin.IsDeleted == true)
            {
                response.Message = "Admin already Deleted";
                return response;
            }
            admin.IsDeleted = true;
        
            response.Message = "Admin Deleted";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Get(Guid id)
        {
            var response = new BaseResponse<AdminDto>();
            var admin = await _adminRepository.Get(a => a.Id == id);
            if(admin == null)
            {
                response.Message = "admin not found";
                return response;
            }
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName
            };
        
            response.Message = "admin gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Get(string email)
        {
            var response = new BaseResponse<AdminDto>();
            var admin = await _adminRepository.Get(a => a.Email == email);
            if (admin == null)
            {
                response.Message = "admin gotten";
                return response;
            }
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName
            };
            
            response.Message = "admin gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<AdminDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<AdminDto>>();
            var admins = await _adminRepository.GetAll();
            if (admins == null)
            {   
                response.Message = "admins not found";
                return response;
            }

            var adminDtos = admins.Select(a => new AdminDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email
            }).ToList();
        
            response.Data = adminDtos;
            response.Message = "List of admins";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Register(AdminDto adminDto)
        {
            var response = new BaseResponse<AdminDto>();
            var getadmin = await _adminRepository.Get(a => a.Email == adminDto.Email.ToLower());
            var defaultPassword = $"{adminDto.FirstName}";
            string saltString = HashingHelper.GenerateSalt();
            string hashedPassword = HashingHelper.HashPassword(defaultPassword, saltString);

            if (getadmin != null)
            {
                response.Message = "Email already exist";
                return response;
            }
            var user = new User
            {
                UserName = $"{adminDto.FirstName}{adminDto.LastName}",
                HashSalt = saltString,
                PasswordHash = hashedPassword,
                Email = adminDto.Email.ToLower()
            };
            
            var role = await _roleRepository.Get(r =>r.Name == "Admin");
            if (role == null)
            {
                response.Message = "Role not found";
                return response;
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
            await _userRepository.Register(user);
            var addadmin = await _adminRepository.Register(admin);
            await _adminRepository.SaveChangesAsync();
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

             
            response.Data = adminDTO;
            response.Message = "Admin registered succesfuly";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Update(Guid id, AdminDto adminDto)
        {
            var response = new BaseResponse<AdminDto>();
            var admin = await _adminRepository.Get(a => a.Id == id);
            var adminExist = await _adminRepository.ExistsAsync(a => a.Id == id);
            if (!adminExist)
            {
               response.Message = "admin found";
               return response;
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
            await _userRepository.Update(getUser);

            admin.User.Email = adminDto.Email ?? admin.User.Email;
            admin.FirstName = adminDto.FirstName ?? admin.FirstName;
            admin.LastName = adminDto.LastName ?? admin.LastName;
            await _adminRepository.Update(admin);

           
            response.Message = "Admin updated succesfully";
            response.Status = true;
            return response;
        }
    }
}
