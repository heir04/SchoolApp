using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface IAssignmentRepository : IBaseRepository<Assignment>
    {
        Task<IList<Assignment>> GetAllAssignments(Expression<Func<Assignment, bool>> expression);
        Task<IList<Assignment>> GetAllAssignments();
        Task<Assignment> GetAssignment(Expression<Func<Assignment, bool>> expression);
    }
}