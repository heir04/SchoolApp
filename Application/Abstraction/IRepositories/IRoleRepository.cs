using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<IList<Role>> GetRolesByUserId(Guid id);
    }
}
