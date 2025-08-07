using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.Test
{
    public class CreateAutoTestInput
    {
        public TestType TestType { get; set; }
        public CourseLevel CourseLevel { get; set; }
    }
}
