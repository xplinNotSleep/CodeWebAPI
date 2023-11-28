using DotSpatial.Projections;
using ServiceCenter.Core;
using ServiceCenter.Core.NPOI;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

namespace AgDataHandle.WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件|*.xls*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = ofd.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件|*.xls";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = saveFileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox2.Text.IsNullOrEmpty())
            {
                MessageBox.Show("输出Excel不能为空");
                return;
            }
            if (this.textBox1.Text.IsNullOrEmpty())
            {
                MessageBox.Show("输入Excel不能为空");
                return;
            }
            if (this.textBox1.Text == this.textBox2.Text)
            {
                MessageBox.Show("输入Excel与输出Excel文件名不能相同！");
                return;
            }
            if (this.textBox3.Text.IsNullOrEmpty() || this.textBox4.Text.IsNullOrEmpty())
            {
                MessageBox.Show("原始点坐标字段不能为空");
                return;
            }
            if (this.richTextBox1.Text.IsNullOrEmpty() || this.richTextBox6.Text.IsNullOrEmpty())
            {
                MessageBox.Show("投影信息不能为空！");
                return;
            }
            ConfigHelper.Instance.Write("textBox1", this.textBox1.Text);
            ConfigHelper.Instance.Write("textBox2", this.textBox2.Text);
            ConfigHelper.Instance.Write("textBox3", this.textBox3.Text);
            ConfigHelper.Instance.Write("textBox4", this.textBox4.Text);
            ConfigHelper.Instance.Write("textBox5", this.textBox5.Text);
            ConfigHelper.Instance.Write("textBox6", this.textBox6.Text);
            ConfigHelper.Instance.Write("richTextBox1", this.richTextBox1.Text);
            ConfigHelper.Instance.Write("richTextBox6", this.richTextBox6.Text);
            ConfigHelper.Instance.Write("tabControl1", this.tabControl1.SelectedIndex.ToString());
            NPOIService service = new NPOIService();
            var dt = service.XlSToDataTable(this.textBox1.Text);
            List<double> yszbs = new List<double>();
            List<double> jwds = new List<double>();
            foreach (DataRow item in dt.Rows)
            {
                yszbs.Add(Convert.ToDouble(item[this.textBox3.Text]));
                yszbs.Add(Convert.ToDouble(item[this.textBox4.Text]));
                jwds.Add(Convert.ToDouble(item[this.textBox5.Text]));
                jwds.Add(Convert.ToDouble(item[this.textBox6.Text]));
            }
            ProjectionInfo projection = GetProjectionInfo(this.richTextBox1.Text); 
            ProjectionInfo targetProj= GetProjectionInfo(this.richTextBox6.Text);
            if(projection == null||targetProj==null)
            {
                return;
            }
            var yszbsArr = yszbs.ToArray();
            Reproject.ReprojectPoints(yszbsArr, null, projection, targetProj, 0, yszbs.Count / 2);
            DataTable dtRes = new DataTable();
            dtRes.TableName = "输出坐标";
            dtRes.Columns.Add(new DataColumn(this.textBox3.Text, typeof(double)));
            dtRes.Columns.Add(new DataColumn(this.textBox4.Text, typeof(double)));
            if (!this.textBox5.Text.IsNullOrEmpty() && !this.textBox6.Text.IsNullOrEmpty())
            {
                dtRes.Columns.Add(new DataColumn(this.textBox5.Text, typeof(double)));
                dtRes.Columns.Add(new DataColumn(this.textBox6.Text, typeof(double)));
            }
            dtRes.Columns.Add(new DataColumn("输出绝对x", typeof(double)));
            dtRes.Columns.Add(new DataColumn("输出绝对y", typeof(double)));
            dtRes.Columns.Add(new DataColumn("输出比对值x", typeof(double)));
            dtRes.Columns.Add(new DataColumn("输出比对值y", typeof(double)));
            for (int i = 0; i < yszbs.Count; i += 2)
            {
                int nIndex = 0;
                var dr = dtRes.NewRow();
                dr[nIndex++] = yszbs[i];
                dr[nIndex++] = yszbs[i + 1];
                if (!this.textBox5.Text.IsNullOrEmpty() && !this.textBox6.Text.IsNullOrEmpty())
                {
                    dr[nIndex++] = jwds[i];
                    dr[nIndex++] = jwds[i + 1];
                }
                dr[nIndex++] = yszbsArr[i];
                dr[nIndex++] = yszbsArr[i + 1];
                if (!this.textBox5.Text.IsNullOrEmpty() && !this.textBox6.Text.IsNullOrEmpty())
                {
                    dr[nIndex++] = yszbsArr[i] - jwds[i];
                    dr[nIndex] = yszbsArr[i + 1] - jwds[i + 1];
                }
                dtRes.Rows.Add(dr);
            }

            var meomery = service.DataTableToXLS(dtRes);
            File.WriteAllBytes(this.textBox2.Text, meomery.ToArray());
            MessageBox.Show("执行完成");
        }

        ProjectionInfo GetProjectionInfo(string projinfo)
        {
            try
            {


                ProjectionInfo projection = null;
                if (projinfo.StartsWithIgnoreCase("PROJCS"))
                {
                    projection = ProjectionInfo.FromEsriString(projinfo);
                }
                else if (projinfo.All(char.IsDigit))
                {
                    projection = ProjectionInfo.FromEpsgCode(int.Parse(projinfo));
                }
                else if (Regex.IsMatch(projinfo, @"[\S]+:[\d]+"))
                {
                    var arr = projinfo.Split(':');
                    projection = ProjectionInfo.FromAuthorityCode(arr[0], int.Parse(arr[1]));
                }
                else
                {
                    projection = ProjectionInfo.FromProj4String(projinfo);
                }
                return projection;
            }
            catch
            {
                MessageBox.Show("请输入正确的投影信息！");
                return null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var textBox1 = ConfigHelper.Instance.Read("textBox1");
            if (textBox1 != null)
            {
                this.textBox1.Text = textBox1;
                this.tabControl1.SelectedIndex= Convert.ToInt32(ConfigHelper.Instance.Read("tabControl1") ?? "0");
                this.textBox2.Text = ConfigHelper.Instance.Read("textBox2") ?? "";
                this.textBox3.Text = ConfigHelper.Instance.Read("textBox3") ?? "绝对x";
                this.textBox4.Text = ConfigHelper.Instance.Read("textBox4") ?? "绝对y";
                this.textBox5.Text = ConfigHelper.Instance.Read("textBox5") ?? "";
                this.textBox6.Text = ConfigHelper.Instance.Read("textBox6") ?? "";
                this.richTextBox1.Text = ConfigHelper.Instance.Read("richTextBox1") ?? "";
                this.richTextBox2.Text = ConfigHelper.Instance.Read("richTextBox2") ?? "";
                this.richTextBox4.Text = ConfigHelper.Instance.Read("richTextBox4") ?? "";
                this.richTextBox5.Text = ConfigHelper.Instance.Read("richTextBox5") ?? "EPSG:4490";
                this.richTextBox6.Text = ConfigHelper.Instance.Read("richTextBox6") ?? "EPSG:4490";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(this.richTextBox2.Text.IsNullOrEmpty()||  this.richTextBox4.Text.IsNullOrEmpty()|| this.richTextBox5.Text.IsNullOrEmpty())
            {
                MessageBox.Show("请补传信息！");
                return;
            }
            ConfigHelper.Instance.Write("richTextBox2", this.richTextBox2.Text);
            ConfigHelper.Instance.Write("richTextBox4", this.richTextBox4.Text);
            ConfigHelper.Instance.Write("richTextBox5", this.richTextBox5.Text);
            ConfigHelper.Instance.Write("tabControl1", this.tabControl1.SelectedIndex.ToString());
            var orignProj=GetProjectionInfo(this.richTextBox4.Text);
            var targetProj= GetProjectionInfo(this.richTextBox5.Text);
            if (orignProj == null || targetProj == null)
            {
                return;
            }
            var orignPoints=this.richTextBox2.Text.Split(' ').Where(p=>!p.IsNullOrEmpty()).Select(p=>double.Parse(p)).ToArray();
            Reproject.ReprojectPoints(orignPoints, null, orignProj, targetProj, 0, orignPoints.Length / 2);
            this.richTextBox3.Text=orignPoints.Join(" ");
        }
    }
}
