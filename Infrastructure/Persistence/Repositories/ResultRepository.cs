using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class ResultRepository : BaseRepository<Result>, IResultRepository
    {
        public ResultRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Result>> GetAllResult(Expression<Func<Result, bool>> expression)
        {
            var result = _context.Results
                .Where(expression)
                .Include(r => r.Student)
                .Include(r => r.Session)
                .Include(r => r.Subject)
                .ToListAsync();

            return await result;
        }
    }
}