using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Cache
{
    public interface ICacheService
    {
        Task<string> GetDataAsync(string id);
    }
}
