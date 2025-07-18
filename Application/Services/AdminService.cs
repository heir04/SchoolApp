﻿using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
using SchoolApp.Infrastructure.Context;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SchoolApp.Application.Services
{
    public class AdminService(ApplicationContext context, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IAdminService
    {
        private readonly ApplicationContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<BaseResponse<AdminDto>> Delete(Guid id)
        {
            var response = new BaseResponse<AdminDto>();
            var adminExist = await _unitOfWork.Admin.ExistsAsync(a => a.Id == id && a.IsDeleted == false);
            if (!adminExist)
            {
                response.Message = "Admin not found";
                return response;
            }
            var admin = await _unitOfWork.Admin.Get(a => a.Id == id);
            if (admin == null)
            {
                response.Message = "Admin not found";
                return response;
            }
            admin.IsDeleted = true;
            await _unitOfWork.Admin.SaveChangesAsync();
        
            response.Message = "Admin Deleted";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Get(Guid id)
        {
            var response = new BaseResponse<AdminDto>();
            var adminExist = await _unitOfWork.Admin.ExistsAsync(a => a.Id == id && a.IsDeleted == false);
            if (!adminExist)
            {
                response.Message = "admin not found";
                return response;
            }

            var admin = await _unitOfWork.Admin.Get(a => a.Id == id);
            if(admin == null )
            {
                response.Message = "admin not found";
                return response;
            }
            
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                DateOfBirth = admin.DateOfBirth,
                PhoneNumber = admin.PhoneNumber,
                Gender = admin.Gender,
                Address = admin.Address
            };
        
            response.Message = "admin gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Get(string email)
        {
            var response = new BaseResponse<AdminDto>();
            var adminExist = await _unitOfWork.Admin.ExistsAsync(a => a.Email == email && a.IsDeleted == false);
            if (!adminExist)
            {
                response.Message = "admin not found";
                return response;
            }
            var admin = await _unitOfWork.Admin.Get(a => a.Email == email);
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
        
        public async Task<BaseResponse<AdminDto>> GetByCurrentUserId()
        {
            var response = new BaseResponse<AdminDto>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }
            var adminExist = await _unitOfWork.Admin.ExistsAsync(a => a.UserId == userId && a.IsDeleted == false);
            if (!adminExist)
            {
                response.Message = "admin not found";
                return response;
            }
            var admin = await _unitOfWork.Admin.Get(a => a.UserId == userId);
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
                LastName = admin.LastName,
                DateOfBirth = admin.DateOfBirth,
                PhoneNumber = admin.PhoneNumber,
                Gender = admin.Gender,
                Address = admin.Address,
                CreatedOn = admin.CreatedOn
            };
            
            response.Message = "admin gotten";
            response.Status = true;
            response.Data = adminDto;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<AdminDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<AdminDto>>();
            var admins = await _unitOfWork.Admin.GetAll();
            if (admins == null)
            {   
                response.Message = "admins not found";
                return response;
            }

            var adminDtos = admins
            .Where(a => a.IsDeleted == false)
            .Select(a => new AdminDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                DateOfBirth = a.DateOfBirth,
                PhoneNumber = a.PhoneNumber,
                Gender = a.Gender,
                Address = a.Address,
                CreatedOn = a.CreatedOn
            }).ToList();
        
            response.Data = adminDtos;
            response.Message = "List of admins";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Register(AdminDto adminDto)
        {
            var response = new BaseResponse<AdminDto>();
            var getadmin = await _unitOfWork.Admin.ExistsAsync(a => a.Email == adminDto.Email.ToLower());
            var defaultPassword = $"{adminDto.FirstName}";
            string saltString = HashingHelper.GenerateSalt();
            string hashedPassword = HashingHelper.HashPassword(defaultPassword, saltString);

            if (getadmin)
            {
                response.Message = "Email already exist";
                return response;
            }
            var user = new User
            {
                UserName = $"{adminDto.FirstName}{adminDto.LastName}",
                HashSalt = saltString,
                PasswordHash = hashedPassword,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Email = adminDto.Email?.ToLower()
            };
            
            var role = await _unitOfWork.Role.Get(r => r.Name == "Admin");
            if (role == null)
            {
                response.Message = "Role not found";
                return response;
            }
            var userRole = new UserRole
            {
                UserId = user.Id,
                User = user,
                RoleId = role.Id,
                Role = role,
                CreatedOn = DateTime.Today
            };
            _context.UserRoles.Add(userRole);
            user.UserRoles.Add(userRole);
            
            var admin = new Admin
            {
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,           
                Email = adminDto.Email?.ToLower(),
                UserId = user.Id,
                User = user,
                DateOfBirth = adminDto.DateOfBirth,
                PhoneNumber = adminDto.PhoneNumber,
                Gender = adminDto.Gender,
                Address = adminDto.Address,
                CreatedOn = DateTime.Today
            };

            await _unitOfWork.Admin.Register(admin);
            await _unitOfWork.User.Register(user);
            await _unitOfWork.SaveChangesAsync();
            // admin.CreatedBy = addadmin.Id;
            // admin.LastModifiedBy = addadmin.Id;

            response.Message = "Admin registered succesfuly";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<AdminDto>> Update(Guid id, AdminDto adminDto)
        {
            var response = new BaseResponse<AdminDto>();
            
            var adminExist = await _unitOfWork.Admin.ExistsAsync(a => a.Id == id && a.IsDeleted == false);
            if (!adminExist)
            {
               response.Message = "admin not found";
               return response;
            }
            var admin = await _unitOfWork.Admin.Get(a => a.Id == id);
            
            var getUser = await _unitOfWork.User.Get(u => u.Id == admin.UserId);
            if (getUser == null)
            {
                response.Message = "User not found";
                return response;
            }
            getUser.Email = adminDto.Email;

            admin.FirstName = adminDto.FirstName ?? admin.FirstName;
            admin.LastName = adminDto.LastName ?? admin.LastName;
            admin.DateOfBirth = adminDto.DateOfBirth;
            admin.PhoneNumber = adminDto.PhoneNumber ?? admin.PhoneNumber;
            admin.Gender = adminDto.Gender ?? admin.Gender;
            admin.Address = adminDto.Address ?? admin.Address;
            admin.LastModifiedBy = admin.Id;
            admin.LastModifiedOn = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            response.Message = "Admin updated succesfully";
            response.Status = true;
            return response;
        }
    }
}
