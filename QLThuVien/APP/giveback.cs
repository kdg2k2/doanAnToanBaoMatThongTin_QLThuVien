﻿using ClosedXML.Excel;
using ExcelDataReader;
using QLThuVien.DAL;
using QLThuVien.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Z.Dapper.Plus;

namespace QLThuVien.APP
{
    public partial class @return : Form
    {
        public @return()
        {
            InitializeComponent();
        }

        SqlConnection con = DBConnect.GetDBConnection();

        public void HienThi()
        {
            string sqlSelect = "select * from giveback";
            SqlCommand cmd = new SqlCommand(sqlSelect, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dataView.DataSource = dt;
        }
        int dem = 0;
        private void KiemTraMa(string TenBang, string TenField, string DieuKien)
        {
            DataSet ds = new DataSet();
            string strSQL = " Select * From " + TenBang;
            if (TenField != "" && DieuKien != "")
            {
                strSQL += " Where " + TenField + "='" + DieuKien + "'";
            }
            SqlDataAdapter da = new SqlDataAdapter(strSQL, con);
            da.Fill(ds, TenBang);
            DataTable table = ds.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                dem++;
            }
        }
        private void return_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLThuVienDataSet.giveback' table. You can move, or remove it, as needed.
            this.givebackTableAdapter.Fill(this.qLThuVienDataSet.giveback);
            con.Open();
            HienThi();
        }

        private void return_FormClosing(object sender, FormClosingEventArgs e)
        {
            con.Close();
        }

        private void btInsert_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbReturn_id.Text) || String.IsNullOrEmpty(tbIssue_id.Text) || String.IsNullOrEmpty(tbStaff.Text) || String.IsNullOrEmpty(tbBook_id.Text))
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu vào");
                return;
            }

            KiemTraMa("giveback", "return_id", tbReturn_id.Text);
            if (dem > 0)
            {
                MessageBox.Show("Mã đã tồn tại");
                MessageBox.Show("Hãy nhập mã khác");
                dem = 0;
                return;
            }
            else
            {
                string sqlThem = "INSERT INTO giveback " +
                                "VALUES (@return_id, @issue_id, @date_return, @staff_id, @book_id)";
                SqlCommand cmd = new SqlCommand(sqlThem, con);
                cmd.Parameters.AddWithValue("return_id", tbReturn_id.Text);
                cmd.Parameters.AddWithValue("issue_id", tbIssue_id.Text);
                cmd.Parameters.AddWithValue("date_return", dateReturn.Value);
                cmd.Parameters.AddWithValue("staff_id", tbStaff.Text);
                cmd.Parameters.AddWithValue("book_id", tbBook_id.Text);
                cmd.ExecuteNonQuery();
                HienThi();
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbReturn_id.Text) || String.IsNullOrEmpty(tbIssue_id.Text) || String.IsNullOrEmpty(tbStaff.Text) || String.IsNullOrEmpty(tbBook_id.Text))
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu vào");
                return;
            }

            string sqlThem = "update giveback " +
                                "set return_id=@return_id, issue_id=@issue_id, date_return=@date_return, staff_id=@staff_id, book_id=@book_id " +
                                "where return_id=@return_id";
            SqlCommand cmd = new SqlCommand(sqlThem, con);
            cmd.Parameters.AddWithValue("return_id", tbReturn_id.Text);
            cmd.Parameters.AddWithValue("issue_id", tbIssue_id.Text);
            cmd.Parameters.AddWithValue("date_return", dateReturn.Value);
            cmd.Parameters.AddWithValue("staff_id", tbStaff.Text);
            cmd.Parameters.AddWithValue("book_id", tbBook_id.Text);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbReturn_id.Text))
            {
                MessageBox.Show("Bạn phải nhập mã trả vào");
                return;
            }

            string sqlThem = "delete from giveback " +
                                "where return_id=@return_id";
            SqlCommand cmd = new SqlCommand(sqlThem, con);
            cmd.Parameters.AddWithValue("return_id", tbReturn_id.Text);
            cmd.Parameters.AddWithValue("issue_id", tbIssue_id.Text);
            cmd.Parameters.AddWithValue("date_return", dateReturn.Value);
            cmd.Parameters.AddWithValue("staff_id", tbStaff.Text);
            cmd.Parameters.AddWithValue("book_id", tbBook_id.Text);
            cmd.ExecuteNonQuery();
            HienThi();
        }

        private void btTimKiem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbNoiDungTimKiem.Text))
            {
                MessageBox.Show("Bạn phải nhập đầy đủ dữ liệu vào");
                return;
            }

            string sqlThem = "SELECT * " +
                                    "FROM giveback " +
                                    "WHERE return_id=@return_id";
            SqlCommand cmd = new SqlCommand(sqlThem, con);
            cmd.Parameters.AddWithValue("return_id", tbNoiDungTimKiem.Text);
            cmd.Parameters.AddWithValue("issue_id", tbIssue_id.Text);
            cmd.Parameters.AddWithValue("date_return", dateReturn.Value);
            cmd.Parameters.AddWithValue("staff_id", tbStaff.Text);
            cmd.Parameters.AddWithValue("book_id", tbBook_id.Text);
            cmd.ExecuteNonQuery();
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dataView.DataSource = dt;
        }

        DataTableCollection tables;
        private void btTimFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel File|*.xls;*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    tbFileName.Text = ofd.FileName;
                    using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });
                            tables = result.Tables;
                            cbbSheet.Items.Clear();

                            foreach (DataTable table in tables)
                                cbbSheet.Items.Add(table.TableName);//add sheet

                            this.cbbSheet.Text = "giveback";//Mặc định
                        }
                    }
                }
            }
        }

        private void cbbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = tables[cbbSheet.SelectedItem.ToString()];
            if (dt != null)
            {
                List<DTO_giveback> list = new List<DTO_giveback>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DTO_giveback obj = new DTO_giveback();
                    obj.return_id = dt.Rows[i]["return_id"].ToString();
                    obj.issue_id = dt.Rows[i]["issue_id"].ToString();
                    obj.date_return = dt.Rows[i]["date_return"].ToString();
                    obj.staff_id = dt.Rows[i]["staff_id"].ToString();
                    obj.book_id = dt.Rows[i]["book_id"].ToString();
                    list.Add(obj);
                }
                givebackBindingSource.DataSource = list;
            }
        }

        private void btImport_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tbFileName.Text))
            {
                MessageBox.Show("Bạn phải chọn tệp dữ liệu để nhập vào");
                return;
            }

            try
            {
                string connecionString = "Server=DG;Database=QLThuVien;User Id=sa;Password=a12345678;";
                DapperPlusManager.Entity<DTO_giveback>().Table("giveback");
                List<DTO_giveback> temp = givebackBindingSource.DataSource as List<DTO_giveback>;
                if (temp != null)
                {
                    using (IDbConnection db = new SqlConnection(connecionString))
                    {
                        db.BulkInsert(temp);
                    }
                    MessageBox.Show("Imported successfully");
                    HienThi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel File|*.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            workbook.Worksheets.Add(this.qLThuVienDataSet.giveback.CopyToDataTable(), "giveback");
                            workbook.SaveAs(sfd.FileName);
                        }
                        MessageBox.Show("Xuất tệp Excel thành công", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            this.tbReturn_id.Clear();
            this.tbIssue_id.Clear();
            this.tbStaff.Clear();
            this.tbFileName.Clear();
            this.tbBook_id.Clear();
            HienThi();
        }
    }
}