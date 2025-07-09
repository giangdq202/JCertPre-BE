using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Enums
{
    public enum ContentName
    {
        
        [Description("chữ hán")]
        ChuHan,
        [Description("từ vựng")]
        TuVung,
        [Description("ngữ pháp")]
        NguPhap,
        [Description("đọc hiểu")]
        DocHieu,
        [Description("nghe hiểu")]
        NgheHieu
    }
}
