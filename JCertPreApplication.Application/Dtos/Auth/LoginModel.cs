using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Auth
{
    public class LoginModel
    {
        public string EmailorPhone { get; set; }
        public string Password { get; set; }
    }
}
