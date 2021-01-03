using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO;
using BUS;
using DAO;
using System.Threading;
using Guna;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace RestaurantManagerment
{
    public partial class Tab1_1BanAn : UserControl
    {
        public Tab1_1BanAn()
        {
            InitializeComponent();
            panelOrderMon.Visible = false;
        }

        private void Tab1_1_1BanAn_Load(object sender, EventArgs e)
        {
            LoadBanAn();
            LoadThoiGian();
            LoadNhomMonAn();
            loadMonAn();
        }

        //------------- lấy ngày tháng năm hiện tại ---------------------------
        void LoadThoiGian()
        {
            DateTime nowTime = DateTime.Now;
            lbNgay.Text = nowTime.ToString("dd/MM/yyyy");
        }

        public List<BanAn_DTO> danhSachBanAn;
        ListViewItem banAn;
        private void LoadBanAn()
        {
            danhSachBanAn = BanAn_BUS.LoadBanAn();
            if (danhSachBanAn == null)
                return;
            for (int i = 0; i < danhSachBanAn.Count; i++)
            {
                banAn = new ListViewItem();
                if (danhSachBanAn[i].TrangThai == "Có Người")
                {
                    banAn.ImageIndex = 0;
                }
                else
                    banAn.ImageIndex = 1;
                banAn.Text = danhSachBanAn[i].TenBan;
                listBanAn.Items.Add(banAn);
            }
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            listBanAn.Items.Clear();
            LoadBanAn();
            flplistNhomMon.Controls.Clear();
            LoadNhomMonAn();
            flplistMonAn.Controls.Clear();
            loadMonAn();
        }

        private void listBanAn_DoubleClick_1(object sender, EventArgs e)
        {
            if (listBanAn.Items[indexTable].ImageIndex == 0)
            {
                panelOrderMon.Visible = true;
                panelOrderMon.BringToFront();
            }
            else
            {
                MessageBox.Show("Mở bàn để chọn món");
            }
        }

        // ----------------------- Load Hóa Đơn -----------------------------
        List<HoaDonOrder_DTO> danhSachHoaDon;
        private void LoadHoaDon(int idBan)
        {
            int tongTien = 0;
            dgvHoaDonOrder.Rows.Clear();  // xóa hết các dòng trên listview đi để tránh cái sau đè lên cái trước
            danhSachHoaDon = HoaDonOrder_BUS.LoadHoaDon(idBan);  // lấy hóa đơn của bàn đang được click
            if (danhSachHoaDon == null)                    // nếu không có hóa đơn tiền = 0
            {
                lbTongTienThanhToan.Text = "0";
                lbThanhTien.Text = "0";
                return;
            }

            else    // nếu có hóa đơn
            {
                for (int i = 0; i < danhSachHoaDon.Count; i++)  // duyệt từ đầu danh sách hóa đơn đến cuối danh sách hóa đơn
                {

                    tongTien += int.Parse(danhSachHoaDon[i].ThanhTien.ToString());  // tính tổng tiền
                    dgvHoaDonOrder.Rows.Add(danhSachHoaDon[i].TenMonAn.ToString(), danhSachHoaDon[i].SoLuong.ToString(), danhSachHoaDon[i].Gia.ToString(), danhSachHoaDon[i].ThanhTien.ToString()); // thêm items vừa tạo vào listview
                }
                //CultureInfo cul = new CultureInfo("vi-VN");

                // Gán Tiền vào textbox
                lbTongTienThanhToan.Text = tongTien.ToString();
                lbThanhTien.Text = lbTongTienThanhToan.Text;
            }
        }

        // Sự kiện khi nhấn vào 1 bàn ăn thì tất cập nhật trạng thái và cập nhật hóa đơn cho bàn đó
        public int indexTable = -1; // lưu chỉ số của bàn đang được chọn
        private void listBanAn_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            indexTable = e.ItemIndex;  // khi click vào bàn nào thì lưu lại chỉ số của bàn đó

            for (int i = 0; i < listBanAn.Items.Count; i++)  // duyệt tất cả các bàn
            {
                if (listBanAn.Items[i].Selected)  // kiểm tra nếu bàn nào được click thì
                {
                    if (listBanAn.Items[i].ImageIndex == 1)  // nếu bàn đó đang trống thì màu chữ trạng thái có màu đỏ
                    {
                        lbBan.ForeColor = Color.Red;
                        lbTrangThai.ForeColor = Color.Red;
                        lbTrangThai.Text = "Trống";
                    }
                    else  // ngược lại thì màu chữ là màu xanh
                    {
                        lbBan.ForeColor = Color.ForestGreen;
                        lbTrangThai.ForeColor = Color.ForestGreen;
                        lbTrangThai.Text = "Có Người";
                    }
                    // load trạng thái cho label
                    lbBan.Text = danhSachBanAn[i].TenBan.ToString();

                    // hiển thị hóa đơn
                    LoadHoaDon(danhSachBanAn[i].ID);
                }
            }
        }

        //------------Mở bàn------------------------
        private void btnMoBan_Click(object sender, EventArgs e)
        {
            if (indexTable == -1) // nếu chưa có bàn nào được chọn
            {
                MessageBox.Show("Chọn 1 Bàn Để Mở", "Thông Báo");
                return;
            }
            if (listBanAn.Items[indexTable].ImageIndex == 0)  // Nếu Bàn Đang Mở thì không thêm hóa đơn nữa
            {
                MessageBox.Show("Bàn Đang Mở", "Thông Báo");
                return;
            }
            // khi click thì thêm 1 hóa đơn vào bảng hóa đơn

            // nếu Thêm hóa đơn thành công thì Bàn sẽ màu xanh và trạng thái thành có người
            if (HoaDonOrder_BUS.ThemHoaDon(danhSachBanAn[indexTable].ID) == true && BanAn_BUS.SuaTrangThaiBanAn(danhSachBanAn[indexTable].ID) == true)
            {

                lbTrangThai.Text = "Có Người";
                listBanAn.Items[indexTable].ImageIndex = 0;

                lbBan.ForeColor = Color.Green;
                lbTrangThai.ForeColor = Color.Green;

                MessageBox.Show("Đã Mở Bàn", "Thông Báo");
                panelOrderMon.Visible = true;
                return;
            }
            MessageBox.Show("Lỗi", "Thông Báo");
        }

        private void ButtonThanhToan_Click(object sender, EventArgs e)
        {
            if (indexTable == -1)
            {
                return;
            }
            if (listBanAn.Items[indexTable].ImageIndex == 1)
            {
                MessageBox.Show("Mở Bàn Để Thanh Tóan");
                return;
            }
            int tien = int.Parse(lbThanhTien.Text);

            CultureInfo cul = new CultureInfo("vi-VN"); // định dạng tiền việt
            string tienThanhToan = tien.ToString("c", cul);

            if (MessageBox.Show("Thanh Toán: " + tienThanhToan, "Thanh Toán", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                //  xóa hết món ăn trên bàn ăn đó 
                HoaDonOrder_BUS.XoaThongTinHoaDon(HoaDonOrder_BUS.layIDHoaDon(danhSachBanAn[indexTable].ID));

                // update số tiền
                DateTime time = DateTime.Now;
                string ngayThanhToan = time.ToString("dd/MM/yyyy");
                try
                {
                    HoaDonOrder_BUS.UpdateHoaDon(1, ngayThanhToan, int.Parse(lbThanhTien.Text), danhSachBanAn[indexTable].ID);
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi Rồi....");
                    return;
                }
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 5, 5, 2,2 );
                string timesTTF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.TTF");
                BaseFont arial = BaseFont.CreateFont(timesTTF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font f_24_Bold = new iTextSharp.text.Font(arial, 24, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font f_20_normal = new iTextSharp.text.Font(arial, 20, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font f_20_Bold = new iTextSharp.text.Font(arial, 20, iTextSharp.text.Font.BOLD);
                FileStream os = new FileStream(lbBan.Text.ToString()+".pdf", FileMode.Create);

                using (os) 
                {
                    PdfWriter wri = PdfWriter.GetInstance(doc, os);
                    doc.Open();
                    PdfPTable table = new PdfPTable(1);
                    iTextSharp.text.Image JPEG = iTextSharp.text.Image.GetInstance("restaurant-icons.png");
                    JPEG.BorderColor = iTextSharp.text.BaseColor.WHITE;
                    JPEG.Alignment = Element.ALIGN_CENTER;
                    table.WidthPercentage = 20;
                    table.AddCell(JPEG);

                    doc.Add(table);
                    PdfPTable table1 = new PdfPTable(1);
                    float[] width = new float[] { 20f, 40f };

                    PdfPCell cell1 = new PdfPCell(new Phrase("\nRESTAURANT", f_24_Bold));
                    
                    PdfPCell cell3 = new PdfPCell(new Phrase("\nĐT: 0854402341 - 0333304558", f_20_normal));
                    PdfPCell cell4 = new PdfPCell(new Phrase("-----------------------------------------", f_20_normal));
                    PdfPCell cell5 = new PdfPCell(new Phrase("Hóa Đơn Thanh Toán", f_24_Bold));
                    PdfPCell cell6 = new PdfPCell(new Phrase("Số HD: ", f_20_normal));
                    PdfPCell cell7 = new PdfPCell(new Phrase("Ngày in:   " + lbNgay.Text.ToString()+"                 Giờ in: "+DateTime.Now.ToString("HH:mm:ss tt"), f_20_normal));
                    PdfPCell cell8 = new PdfPCell(new Phrase("Bàn:       " + lbBan.Text.ToString(), f_20_Bold));
                    PdfPCell cell9 = new PdfPCell(new Phrase("Thu ngân:   ADMIN", f_20_normal));
                    PdfPCell cell10 = new PdfPCell(new Phrase("Số khách: ", f_20_normal));                

                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    
                    cell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell5.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell6.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell7.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell8.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell9.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell10.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    cell1.HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL;
                    
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell6.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    cell7.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    cell8.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    cell9.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    cell10.HorizontalAlignment = Element.ALIGN_JUSTIFIED;

                    cell1.VerticalAlignment = Element.ALIGN_TOP;
                    
                    cell3.VerticalAlignment = Element.ALIGN_CENTER;

                    
                    table1.WidthPercentage = 80;
                    table1.HorizontalAlignment = Element.ALIGN_CENTER;
                    
                    table1.AddCell(cell1);      
                    table1.AddCell(cell3);
                    table1.AddCell(cell4);
                    table1.AddCell(cell5);
                    table1.AddCell(cell6);
                    table1.AddCell(cell7);
                    table1.AddCell(cell8);
                    table1.AddCell(cell9);
                    table1.AddCell(cell10);
                    table1.SpacingAfter = 5;
                    table1.SpacingBefore = 5;

                    doc.Add(table1);

                    PdfPTable pdfTable = new PdfPTable(dgvHoaDonOrder.Columns.Count);
                    pdfTable.DefaultCell.Padding = 3;
                    pdfTable.WidthPercentage = 80;
                    pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfTable.SpacingAfter = 5;
                    pdfTable.SpacingBefore = 5;
                    foreach (DataGridViewColumn column in dgvHoaDonOrder.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText,f_20_Bold));
                        
                        cell.Border = iTextSharp.text.Font.BOLD+ iTextSharp.text.Rectangle.BOTTOM_BORDER;                
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        
                        pdfTable.AddCell(cell);         
                    }

                    foreach (DataGridViewRow row in dgvHoaDonOrder.Rows)
                    {
                        
                        foreach (DataGridViewCell cell in row.Cells)
                        {                            
                            PdfPCell c = new PdfPCell(new Phrase(cell.Value.ToString(), f_20_normal));
                            c.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            c.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(c);                          
                        }
                    }
                    doc.Add(pdfTable); 

                    //Thành tiền
                    PdfPTable table2 = new PdfPTable(1);
                    PdfPCell c1 = new PdfPCell(new Phrase("\nTổng Tiền:                                                       " + lbTongTienThanhToan.Text.ToString()+" VND", f_20_Bold));                  
                    PdfPCell c2 = new PdfPCell(new Phrase("\nKhuyến Mãi:                                                             " + txtKhuyenMaiPhanTram.Text.ToString()+"%"+"\n", f_20_Bold));
                    PdfPCell c3 = new PdfPCell(new Phrase("\nThành Tiền:                                                      " + lbThanhTien.Text.ToString()+" VND", f_20_Bold));
                    PdfPCell c4 = new PdfPCell(new Phrase("\n\n\nHân hạnh được phục vụ quý khách !", f_20_normal));

                    c1.Border = iTextSharp.text.Font.BOLD;
                    c2.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                    c3.Border= iTextSharp.text.Rectangle.NO_BORDER;
                    c4.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    c1.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    c2.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    c3.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                    c3.VerticalAlignment = Element.ALIGN_CENTER;
                    c4.HorizontalAlignment = Element.ALIGN_CENTER;

                    table2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table2.WidthPercentage = 80;
                    table2.AddCell(c1);
                    table2.AddCell(c2);
                    table2.AddCell(c3);
                    table2.AddCell(c4);
                    table2.SpacingAfter = 10;
                    table2.SpacingBefore = 10;
                    doc.Add(table2);

                    

                    doc.Close();
                    System.Diagnostics.Process.Start(lbBan.Text.ToString() + ".pdf");
                }    
                    
                // sửa lại trạng thái bàn ăn thành trống
                BanAn_BUS.SuaTrangThaiBanAn2(danhSachBanAn[indexTable].ID);

                listBanAn.Items[indexTable].ImageIndex = 1;

                lbTrangThai.Text = "Trống";

                lbBan.ForeColor = Color.Red; // load lại màu chữ
                lbTrangThai.ForeColor = Color.Red; // load lại màu chữ

                LoadHoaDon(danhSachBanAn[indexTable].ID); // sau khi xóa thì load lại hóa đơn
            }
        }

        List<NhomMonAn_DTO> listNhomMonAn;
        private void LoadNhomMonAn()
        {
            listNhomMonAn = NhomMonAn_DAO.LayNhomMonAn();
            for (int i = 0; i < listNhomMonAn.Count; i++)
            {
                Button btn = new Button() { Width = 100, Height = 35 };
                btn.Text = listNhomMonAn[i].TenNhomMonAn;
                btn.BackColor = Color.SkyBlue;
                btn.ForeColor = Color.White;
                btn.Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Click += btn_Click;
                btn.Tag = listNhomMonAn[i];
                flplistNhomMon.Controls.Add(btn);
            }
        }

        //--------------Hiển thị món ăn theo từng nhóm-------------------------------
        private void btn_Click(object sender, EventArgs e)
        {
            flplistMonAn.Controls.Clear();
            string tenNhomMonAn = ((sender as Button).Tag as NhomMonAn_DTO).TenNhomMonAn;
            listMonAn = MonAn_DAO.LoadMonAn();
            for (int i = 0; i < listMonAn.Count; i++)
            {
                string s = listMonAn[i].TenNhomMon;
                if (s == tenNhomMonAn)
                {
                    Guna.UI.WinForms.GunaAdvenceTileButton gunaBtn = new Guna.UI.WinForms.GunaAdvenceTileButton() { Width = 116, Height = 154 };
                    gunaBtn.Image = listMonAn[i].IMAGE;
                    gunaBtn.BaseColor = Color.LightGray;
                    gunaBtn.ImageSize = new Size(107, 90);
                    gunaBtn.Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                    gunaBtn.Text = listMonAn[i].TenMonAn + Environment.NewLine + listMonAn[i].Gia + " Đồng";
                    gunaBtn.DoubleClick += btn_DoubleClick;
                    gunaBtn.Tag = listMonAn[i];
                    flplistMonAn.Controls.Add(gunaBtn);
                }
            }
        }

        private void btnTatCaMonAn_Click(object sender, EventArgs e)
        {
            flplistMonAn.Controls.Clear();
            loadMonAn();
        }

        //--------------------------------Thêm Món Ăn ------------------------------
        List<MonAn_DTO> listMonAn;
        private void loadMonAn()
        {
            listMonAn = MonAn_DAO.LoadMonAn();
            for (int i = 0; i < listMonAn.Count; i++)
            {
                Guna.UI.WinForms.GunaAdvenceTileButton gunaBtn = new Guna.UI.WinForms.GunaAdvenceTileButton() { Width = 116, Height = 154 };
                gunaBtn.Image = listMonAn[i].IMAGE;
                gunaBtn.BaseColor = Color.LightGray;
                gunaBtn.ImageSize = new Size(107, 90);
                gunaBtn.Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
                gunaBtn.Text = listMonAn[i].TenMonAn + Environment.NewLine + listMonAn[i].Gia + " Đồng";
                gunaBtn.DoubleClick += btn_DoubleClick;
                gunaBtn.Tag = listMonAn[i];
                flplistMonAn.Controls.Add(gunaBtn);
            }
        }

        int soluong = 1;
        private void btn_DoubleClick(object sender, EventArgs e)
        {
            int iD = ((sender as Guna.UI.WinForms.GunaAdvenceTileButton).Tag as MonAn_DTO).ID;
            string tenMonAn = ((sender as Guna.UI.WinForms.GunaAdvenceTileButton).Tag as MonAn_DTO).TenMonAn;
            int gia = ((sender as Guna.UI.WinForms.GunaAdvenceTileButton).Tag as MonAn_DTO).Gia;

            if (listBanAn.Items.Count > 0)
            {
                for (int i = 0; i < dgvHoaDonOrder.RowCount; i++)
                {
                    try
                    {
                        if (dgvHoaDonOrder.Rows[i].Cells[0].Value.ToString() == tenMonAn)
                        {
                            MessageBox.Show("Món ăn đã có rồi", "Thông Báo");
                            return;
                        }
                    }
                    catch (Exception) // có lỗi ở chỗ này thì thoát
                    {
                        MessageBox.Show("Món ăn đã có rồi", "Thông Báo");
                        return;
                    }
                }
            }

            dgvHoaDonOrder.Rows.Add(tenMonAn, soluong , gia, soluong*gia);

            HoaDonOrder_BUS.ThemThongTinHoaDon(HoaDonOrder_BUS.layIDHoaDon(danhSachBanAn[indexTable].ID), iD, soluong);
            LoadHoaDon(danhSachBanAn[indexTable].ID); //sau khi thêm thì load lại hóa đơn
        }

        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            panelOrderMon.Visible = false;
        }

        //---------------------------------- KHUYẾN MÃI ----------------------------------------------------------------
        // chỉ cho phép nhập số vào textbox khuyến mãi
        private void txtKhuyenMaiVND_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        // khuyến mãi theo tiền
        private void txtKhuyenMaiVND_TextChanged(object sender, EventArgs e)
        {
            if (txtKhuyenMaiVND.Text != "" && lbTongTienThanhToan.Text != "")
            {
                int tongTien, khuyenMai;
                try
                {
                    tongTien = int.Parse(lbTongTienThanhToan.Text.ToString());
                    khuyenMai = int.Parse(txtKhuyenMaiVND.Text.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                if (khuyenMai > tongTien)
                {
                    MessageBox.Show("Khuyễn mãi không hợp lệ !");
                    txtKhuyenMaiVND.Text = "0";
                    return;
                }
                else
                {
                    lbThanhTien.Text = (tongTien - khuyenMai).ToString();
                }
            }
        }

        // chỉ cho phép nhập số vào textbox khuyến mãi theo %
        private void txtKhuyenMaiPhanTram_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        // khuyến mãi theo phần trăm
        private void txtKhuyenMaiPhanTram_TextChanged(object sender, EventArgs e)
        {
            if (txtKhuyenMaiPhanTram.Text != "" && lbTongTienThanhToan.Text != "")
            {

                int tongCong = int.Parse(lbTongTienThanhToan.Text.ToString());
                int khuyenMai = int.Parse(txtKhuyenMaiPhanTram.Text.ToString());

                if (khuyenMai > 100)
                {
                    MessageBox.Show("Khuyễn mãi không hợp lệ !");
                    txtKhuyenMaiPhanTram.Text = "0";
                    return;
                }
                else
                {
                    lbThanhTien.Text = (tongCong - ((tongCong * khuyenMai) / 100)).ToString();
                }
            }
        }
        //----------------------------------  END ------------------------------------------------------------------------


        //-------------Lấy chỉ số dòng được chọn trong dgvHoaDonOrder-------------
        int chiSoCuaLsvHoaDon = -1;
        private void dgvHoaDonOrder_SelectionChanged(object sender, EventArgs e)
        {
            chiSoCuaLsvHoaDon = dgvHoaDonOrder.CurrentCell.RowIndex;
        }

        //-----------------Tăng số lượng món ăn lên 1-----------------
        private void btnTangSoLuongMonAn_Click(object sender, EventArgs e)
        {
            int sl;
            try
            {
                sl = int.Parse(dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                return;
            }
            if (chiSoCuaLsvHoaDon != -1)
            {
                sl = sl + 1;
                dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[1].Value = sl.ToString(); // tăng số lượng món ăn trong lstHoaDon lên 1

                HoaDonOrder_BUS.CapNhatSoLuongMonAn(sl, HoaDonOrder_BUS.layIDHoaDon(danhSachBanAn[indexTable].ID), MonAn_BUS.layIDMonAn(dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[0].Value.ToString()));
                LoadHoaDon(danhSachBanAn[indexTable].ID); // sau khi thêm thì load lại hóa đơn 
            }
        }

        //-----------------giảm số lượng món ăn xuống 1-----------------
        private void btnGiamSoLuongMonAn_Click(object sender, EventArgs e)
        {
            int sl;
            try
            {
                sl = int.Parse(dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[1].Value.ToString());
            }
            catch (Exception)
            {
                return;
            }
            if (chiSoCuaLsvHoaDon != -1)
            {
                if (sl > 1)
                    sl = sl - 1;
                dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[1].Value = sl.ToString(); // Giảm số lượng món ăn đi 1
               
                // cập nhật lại số lượng món ăn trong cơ sở dữ liệu
                HoaDonOrder_BUS.CapNhatSoLuongMonAn(sl, HoaDonOrder_BUS.layIDHoaDon(danhSachBanAn[indexTable].ID), MonAn_BUS.layIDMonAn(dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[0].Value.ToString()));

                LoadHoaDon(danhSachBanAn[indexTable].ID);  // sau khi thêm thì load lại hóa đơn 
            }
        }

        private void btnXoaMonAn_Click(object sender, EventArgs e)
        {
            if (dgvHoaDonOrder.RowCount < 1)
            {
                return;
            }
            if (chiSoCuaLsvHoaDon != -1)
            {
                // xóa món ăn đã chọn dựa vào id hóa đơn và id món ăn
                try
                {
                    // xóa món ăn trong cơ sở dữ liệu dựa vào IDHoaDon và IDMonAN
                    HoaDonOrder_BUS.XoaMonAn(HoaDonOrder_BUS.layIDHoaDon(danhSachBanAn[indexTable].ID), MonAn_BUS.layIDMonAn(dgvHoaDonOrder.Rows[chiSoCuaLsvHoaDon].Cells[0].Value.ToString()));
                }
                catch (Exception)
                {
                    return;
                }

                dgvHoaDonOrder.Rows.RemoveAt(chiSoCuaLsvHoaDon); // xóa món ăn trên listview

                LoadHoaDon(danhSachBanAn[indexTable].ID); // sau khi xóa thì load lại hóa đơn 
            }
        }

        private void btnchuyenBan_Click(object sender, EventArgs e)
        {
            if (indexTable == -1)
            {
                MessageBox.Show("Chọn bàn muốn chuyển");
                return;
            }
            if (listBanAn.Items[indexTable].ImageIndex == 1)
            {
                MessageBox.Show("Mở bàn để chuyển bàn");
                return;
            }
            FormChuyenBan formChuyenBan = new FormChuyenBan();  // khởi tạo form chuyển bàn để lấy dữ liệu từ form
            formChuyenBan.ShowDialog();

            int idToTable = formChuyenBan.idToTable;   // lấy id của bàn muốn chuyển đến

            if (formChuyenBan.chuyenBan == false)   // chuyển bàn = false thì không được chuyển
            {
                return;
            }

            // chuyển từ bàn có id là idfromTable đến bàn có id là idTotable
            if (HoaDonOrder_BUS.ChuyenBan(danhSachBanAn[indexTable].ID, idToTable))
            {
                BanAn_BUS.SuaTrangThaiBanAn2(danhSachBanAn[indexTable].ID); // cập nhật lại trạng thái cho bàn vừa bị chuyển thành trống
                BanAn_BUS.SuaTrangThaiBanAn(idToTable); // cập nhật lại trạng thái cho bàn được chuyển đến thành có người
                listBanAn.Clear();   // load lại bàn ăn
                LoadBanAn();

                LoadHoaDon(danhSachBanAn[indexTable].ID); // load lại hóa đơn

                // cập nhật lại màu chữ cho label
                lbTrangThai.Text = "Trống";
                lbBan.ForeColor = Color.Red;
                lbTrangThai.ForeColor = Color.Red;

                MessageBox.Show("Chuyển thành công");
                return;
            }
            MessageBox.Show("Chuyển thất bại !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    }
}
