﻿using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Application.Services
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        public async Task<BaseResponse<RoleDto>> CreateRole(RoleDto roleDto)
        {
            var response = new BaseResponse<RoleDto>();
            var role = await _roleRepository.ExistsAsync(r => r.Name == roleDto.Name);
            if (role)
            {
                response.Message = "Role already exists";
                return response;
            }

            var newRole = new Role
            {
                Name = roleDto.Name
            };

            await _roleRepository.Register(newRole);
            await _roleRepository.SaveChangesAsync();

            response.Message = "Role created succesfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<RoleDto>>> GetRolesByUserId(Guid userId)
        {
            var response = new BaseResponse<IEnumerable<RoleDto>>();
            var roles = await _roleRepository.GetRolesByUserId(userId);
            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
            }).ToList();

            response.Message = "List gotten";
            response.Status = true;
            response.Data = roleDtos;
            return response;
        }
    }
}
