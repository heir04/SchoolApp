using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Core.Dto;
using SchoolApp.Core.IServices;

namespace SchoolApp.Core.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository) 
        {
            _roleRepository = roleRepository;
        }

        public async Task<BaseResponse<RoleDto>> CreateRole(RoleDto roleDto)
        {
            var role = await _roleRepository.Get(r => r.Name == roleDto.Name);
            if (role != null)
            {
                return new BaseResponse<RoleDto>
                {
                    Message = "Role already exists",
                    Status = false
                };
            }

            var newRole = new Role
            {
                Name = roleDto.Name,
            };

            await _roleRepository.Register(newRole);

            return new BaseResponse<RoleDto>
            {
                Message = "Role created succesfully",
                Status = true
            };
        }

        public async Task<BaseResponse<IEnumerable<RoleDto>>> GetRolesByUserId(Guid userId)
        {
            var roles = await _roleRepository.GetRolesByUserId(userId);
            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
            }).ToList();

            return new BaseResponse<IEnumerable<RoleDto>>
            {
                Message = "List gotten",
                Status = true,
                Data = roleDtos
            };
           
        }
    }
}
