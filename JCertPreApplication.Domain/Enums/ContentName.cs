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
        
        [Description("Chữ Hán")]
        Kanji,
        [Description("Từ Vựng")]
        Vocabulary,
        [Description("Ngữ Pháp")]
        Grammar,
        [Description("Đọc Hiểu")]
        Reading,
        [Description("Nghe Hiểu")]
        Listening
    }
}
