using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Enums
{
    public enum ContentName
    {
        [System.ComponentModel.Description("chữ hán")]
        ChuHan,
        [System.ComponentModel.Description("từ vựng")]
        TuVung,
        [System.ComponentModel.Description("ngữ pháp")]
        NguPhap,
        [System.ComponentModel.Description("đọc hiểu")]
        DocHieu,
        [System.ComponentModel.Description("nghe hiểu")]
        NgheHieu
    }
}
