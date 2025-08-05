using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestRepository : IGenericRepository<Test>
    {
        
    }
}