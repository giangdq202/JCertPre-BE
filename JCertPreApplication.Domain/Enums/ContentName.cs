using System.ComponentModel;

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
