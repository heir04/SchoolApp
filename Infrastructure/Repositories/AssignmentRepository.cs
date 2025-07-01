using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IList<Assignment>> GetAllAssignments(Expression<Func<Assignment, bool>> expression)
        {
            var assignments = await _context.Assignments
                .Where(expression)
                .Include(a => a.Level)
                .Include(a => a.Session)
                .Include(a => a.Term)
                .Include(a => a.Subject)
                .Include(a => a.Teacher)
                .ToListAsync();
            
            return assignments;
        }

        public async Task<IList<Assignment>> GetAllAssignments()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Level)
                .Include(a => a.Session)
                .Include(a => a.Term)
                .Include(a => a.Subject)
                .Include(a => a.Teacher)
                .ToListAsync();
            
            return assignments;
        }

        public async Task<Assignment> GetAssignment(Expression<Func<Assignment, bool>> expression)
        {
            var assignment = await _context.Assignments
                .Where(expression)
                .Include(a => a.Level)
                .Include(a => a.Session)
                .Include(a => a.Term)
                .Include(a => a.Subject)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync();
            
            return assignment;
        }
    }
}