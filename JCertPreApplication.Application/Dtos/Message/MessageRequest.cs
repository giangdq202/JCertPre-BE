using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Message
{
    public class MessageRequest
    {
        public string Content { get; set; }
        public Guid senderId { get; set; }
    }
}
