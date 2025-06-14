using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
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
                .Include(r => r.SubjectScores)
                .ThenInclude(ss => ss.Subject)
                .ToListAsync();

            return await result;
        }

        public async Task<Result> GetResult(Expression<Func<Result, bool>> expression)
        {
            var result = _context.Results
                .Where(expression)
                .Include(r => r.Student)
                .Include(r => r.Level)
                .Include(r => r.Session)
                .Include(r => r.Term)
                .Include(r => r.SubjectScores)
                .ThenInclude(ss => ss.Subject)
                .FirstOrDefaultAsync();

            return await result;
        }
        
        public async Task<Result> GetResultSubjectScore(Expression<Func<Result, bool>> expression)
        {
            var result = _context.Results
                .Where(expression)
                .Include(r => r.SubjectScores)
                .FirstOrDefaultAsync(expression);

            return await result;
        }
    }
}