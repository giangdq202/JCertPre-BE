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
        Kanji,
        [Description("từ vựng")]
        Vocabulary,
        [Description("ngữ pháp")]
        Grammar,
        [Description("đọc hiểu")]
        Reading,
        [Description("nghe hiểu")]
        Listening
    }
}
