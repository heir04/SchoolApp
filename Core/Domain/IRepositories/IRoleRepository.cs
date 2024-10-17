using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Core.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<IList<Role>> GetRolesByUserId(Guid id);
    }
}
