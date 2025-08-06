using JCertPreApplication.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestQuestionRepository : IGenericRepository<TestQuestion>
    {
    }
}