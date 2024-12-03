using System.Linq.Expressions;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface IResultRepository : IBaseRepository<Result>
    {
        Task<List<Result>> GetAllResult(Expression<Func<Result, bool>> expression);
        Task<Result> GetResult(Expression<Func<Result, bool>> expression);
    }
}
