using System.Linq.Expressions;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Core.Domain.IRepositories
{
    public interface IResultRepository : IBaseRepository<Result>
    {
        Task<List<Result>> GetAllResult(Expression<Func<Result, bool>> expression);
    }
}
