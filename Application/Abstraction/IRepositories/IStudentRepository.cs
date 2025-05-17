using SchoolApp.Core.Domain.Entities;
using System.Linq.Expressions;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<IList<Student>> GetAllStudents(Expression<Func<Student, bool>> expression);
        Task<IList<Student>> GetAllStudents();
        Task<Student> GetStudent(Expression<Func<Student, bool>> expression);
    }
}
