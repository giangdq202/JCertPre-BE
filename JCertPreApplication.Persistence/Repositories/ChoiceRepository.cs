using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class ChoiceRepository : GenericRepository<Choice>, IChoiceRepository
    {
        public ChoiceRepository(JCertPreDatabaseContext context) : base(context)
        {
        }
    }
}