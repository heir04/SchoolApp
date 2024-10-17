using SchoolApp.Core.Dto;

namespace SchoolApp.Core.IServices
{
    public interface IRoleService
    {
        Task<BaseResponse<RoleDto>> CreateRole(RoleDto roleDto);
        Task<BaseResponse<IEnumerable<RoleDto>>> GetRolesByUserId(Guid userId);
    }
}
