using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(JCertPreDatabaseContext context) : base(context)
        {
        }
    }
}
