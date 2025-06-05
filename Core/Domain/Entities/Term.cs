using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Term : AuditableEntity
    {
        public string? Name { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        public bool CurrentTerm { get; set; }
    }
}