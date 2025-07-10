using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Infrastructure.Repositories
{
    public class TermRepository : BaseRepository<Term>, ITermRepository
    {
        public TermRepository(ApplicationContext context) 
        {
        }
        
    }
}