﻿using SchoolApp.Core.Domain.Entities;
using System.Linq.Expressions;

namespace SchoolApp.Application.Abstraction.IRepositories
{
    public interface ITeacherRepository : IBaseRepository<Teacher>
    {
        Task<IList<Teacher>> GetAllTeachers();
        Task<Teacher> GetTeacher(Expression<Func<Teacher, bool>> expression);
    }
}
