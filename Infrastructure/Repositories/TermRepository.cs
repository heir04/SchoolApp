using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class TermRepository : BaseRepository<Term>, ITermRepository
    {
        public TermRepository(ApplicationContext context) 
        {
        }
        
    }
}