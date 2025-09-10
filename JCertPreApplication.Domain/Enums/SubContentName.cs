using System.ComponentModel;


namespace JCertPreApplication.Domain.Enums
{
    public enum SubContentName
    {
        //chu han
        [Description("Đọc chữ Hán")]
        Mondai1,
        [Description("Nhớ chữ Hán")]
        Mondai2,

        //tu vung
        [Description("Chọn từ phù hợp với câu")]
        Mondai3,
        [Description("Tìm câu có cách diễn đạt giống")]
        Mondai4,

        //ngu phap
        [Description("Chọn ngữ pháp phù hợp với câu")]
        Mondai5,
        [Description("Sắp xếp câu")]
        Mondai6,
        [Description("Tìm đáp án đúng để hoàn thành đoạn văn")]
        Mondai7,

        // doc hieu
        [Description("Đoạn văn ngắn")]
        Mondai8,
        [Description("Trung văn")]
        Mondai9,
        [Description("Tìm kiếm thông tin")]
        Mondai10,

        // nghe hieu
        [Description("Hiểu đề bài")]
        Mondai11,
        [Description("Hiểu điểm chính")]
        Mondai12,
        [Description("Diễn đạt bằng lời nói")]
        Mondai13,
        [Description("Phản hồi tức thời")]
        Mondai14,
        [Description("Viết đoạn văn ngắn")]
        Mondai15,

    }
}
