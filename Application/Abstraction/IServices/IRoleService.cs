using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IRoleService
    {
        Task<BaseResponse<RoleDto>> CreateRole(RoleDto roleDto);
        Task<BaseResponse<IEnumerable<RoleDto>>> GetRolesByUserId(Guid userId);
    }
}
