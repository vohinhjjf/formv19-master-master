using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO;
using BUS;
using DAO;
using System.Globalization;
using System.Data.SQLite;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Windows.Forms.DataVisualization.Charting;
using DevExpress.XtraPrinting;

namespace RestaurantManagerment
{
    public partial class Tab5 : UserControl
    {
        SQLiteConnection conn = DataProvider.OpenConnection();
        public Tab5()
        {
            InitializeComponent();
        }

        DateTime now;
        private void Tab5_Load(object sender, EventArgs e)
        {
            now = DateTime.Now;
            lbNgay.Text = now.ToString("dd/MM/yyyy");
            lbDoanhThu.Text = DoanhThu();
        }

        private string DoanhThu()
        {
            List<QuanLiHoaDon_DTO> lstHoaDon = HoaDonOrder_BUS.LocHoaDon(now.ToString("dd/MM/yyyy"), now.ToString("dd/MM/yyyy"));

            long doanhThu = 0;
            if (lstHoaDon != null)
            {
                foreach (QuanLiHoaDon_DTO hd in lstHoaDon)
                {
                    doanhThu += long.Parse(hd.SoTien);
                }
            }
            CultureInfo cul = new CultureInfo("vi-VN");
            return doanhThu.ToString("c", cul);        
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            lbDoanhThu.Text = DoanhThu();
            List<QuanLiHoaDon_DTO> lstHoaDon = HoaDonOrder_BUS.LocHoaDon(dtpTuNgay.Text, dtpDenNgay.Text);
            if (lstHoaDon == null)
            {
                MessageBox.Show("Không có kết quả nào");
                return;
            }

            long doanhThu = 0;
            foreach (QuanLiHoaDon_DTO hd in lstHoaDon)
            {
                doanhThu += long.Parse(hd.SoTien);
            }
            CultureInfo cul = new CultureInfo("vi-VN");
            lbDT.Text = doanhThu.ToString("c", cul);

            DataTable dt = DataProvider.LayDataTable("select NgayThanhToan , sum(SoTien) as DoanhThu from HoaDon where NgayThanhToan >= '" + dtpTuNgay.Text + "' AND NgayThanhToan <= '" + dtpDenNgay.Text + "' group by NgayThanhToan", conn);
            chartDoanhThu.DataSource = dt;
            chartDoanhThu.ChartAreas["ChartArea1"].AxisX.Title = "Ngày";
            chartDoanhThu.ChartAreas["ChartArea1"].AxisY.Title = "Doanh Thu";

            chartDoanhThu.Series["Doanh Thu"].XValueMember = "NgayThanhToan";
            chartDoanhThu.Series["Doanh Thu"].YValueMembers = "DoanhThu";
        }

        private void cbNam_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = DataProvider.LayDataTable("select Substr(NgayThanhToan,4,2) as Thang, sum(SoTien) as DoanhThu from HoaDon where Substr(NgayThanhToan,7,4) = '" + cbNam.Text + "' group by Substr(NgayThanhToan,4,2) ", conn);
            chartDoanhThu.DataSource = dt;
            chartDoanhThu.ChartAreas["ChartArea1"].AxisX.Title = "Tháng";
            chartDoanhThu.ChartAreas["ChartArea1"].AxisY.Title = "Doanh Thu";

            chartDoanhThu.Series["Doanh Thu"].XValueMember = "Thang";
            chartDoanhThu.Series["Doanh Thu"].YValueMembers = "DoanhThu";
        }

        private void btnThongKeHangNam_Click(object sender, EventArgs e)
        {
            DataTable dt = DataProvider.LayDataTable("select Substr(NgayThanhToan,7,4) as Nam, Sum(Sotien) as DoanhThu from HoaDon group by Substr(NgayThanhToan,7,4) ", conn);
            chartDoanhThu.DataSource = dt;
            chartDoanhThu.ChartAreas["ChartArea1"].AxisX.Title = "Năm";
            chartDoanhThu.ChartAreas["ChartArea1"].AxisY.Title = "DoanhThu";

            chartDoanhThu.Series["Doanh Thu"].XValueMember = "Nam";
            chartDoanhThu.Series["Doanh Thu"].YValueMembers = "DoanhThu";
        }

        private void gnBTN_IN_Click(object sender, EventArgs e)
        {
            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("Test.pdf", FileMode.Create));
            doc.Open();
            Paragraph par = new Paragraph("Thống Kê Doanh Thu");
            doc.Add(par);
            var charimage = new MemoryStream();
            chartDoanhThu.SaveImage(charimage, ChartImageFormat.Png);
            iTextSharp.text.Image chart_image = iTextSharp.text.Image.GetInstance(charimage.GetBuffer());
            
            doc.Add(chart_image);

            doc.Close();
            System.Diagnostics.Process.Start("Test.pdf");
        }
    }
}
