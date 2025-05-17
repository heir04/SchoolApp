using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationContext context) 
        {
            _context = context;
        }

        public async Task<IList<Student>> GetAllStudents(Expression<Func<Student, bool>> expression)
        {
            var students = await _context.Students
            .Where(expression)
            .Include(s => s.Level)
            .ToListAsync();

            return students;
        }

        public async Task<IList<Student>> GetAllStudents()
        {
            var students = await _context.Students
            .Include(s => s.Level)
            .ToListAsync();

            return students;
        }

        public async Task<Student> GetStudent(Expression<Func<Student, bool>> expression)
        {
            var student = await _context.Students
            .Include(s => s.Level)
            .FirstOrDefaultAsync(expression);
            

            return student;
        }
    }
}