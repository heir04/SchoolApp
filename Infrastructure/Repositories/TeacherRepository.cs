using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SchoolApp.Infrastructure.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Teacher>> GetAllTeachers()
        {
            var teachers = _context.Teachers
            .Include(t => t.TeacherSubjects)
            .ThenInclude(ts => ts.Subject)
            .ToListAsync();

            return await teachers;
        }
        public async Task<Teacher> GetTeacher(Expression<Func<Teacher, bool>> expression)
        {
            var teacher = _context.Teachers
            .Include(t => t.TeacherSubjects)
            .FirstOrDefaultAsync(expression);

            return await teacher;
        }
    }
}