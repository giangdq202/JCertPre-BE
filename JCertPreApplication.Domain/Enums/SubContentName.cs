using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Enums
{
    public enum SubContentName
    {
        // tat ca cac phan
        [Description("Tất cả phần")]
        All,

        //chu han
        [Description("Đọc chữ Hán")]
        DocChuHan,
        [Description("Nhớ chữ Hán")]
        NhoChuHan,

        //tu vung
        [Description("Chọn từ phù hợp với câu")]
        ChonTuPhuHopVoiCau,
        [Description("Tìm câu có cách diễn đạt giống")]
        TimCauCoCachDienDatGiong,

        //ngu phap
        [Description("Chọn ngữ pháp phù hợp với câu")]
        ChonNguPhapPhuHopVoiCau,
        [Description("Sắp xếp câu")]
        SapXepCau,
        [Description("Tìm đáp án đúng để hoàn thành đoạn văn")]
        TimDapAnDungDeHoanThanhDoanVan,

        // doc hieu
        [Description("Đoạn văn ngắn")]
        DoanVanNgan,
        [Description("Trung văn")]
        TrungVan,
        [Description("Tìm kiếm thông tin")]
        TimKiemThongTin,

        // nghe hieu
        [Description("Hiểu đề bài")]
        HieuDeBai,
        [Description("Hiểu điểm chính")]
        HieuDiemChinh,
        [Description("Diễn đạt bằng lời nói")]
        DienDatBangLoiNoi,
        [Description("Phản hồi tức thời")]
        PhanHoiTucThoi
    }
}
