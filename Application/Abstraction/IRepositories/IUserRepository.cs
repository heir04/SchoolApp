using System.Linq.Expressions;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetUser(Expression<Func<User, bool>> expression); 
    }
}
