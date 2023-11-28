namespace AgDataHandle.WinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            label2 = new Label();
            richTextBox1 = new RichTextBox();
            button2 = new Button();
            label3 = new Label();
            textBox2 = new TextBox();
            button3 = new Button();
            label4 = new Label();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            label5 = new Label();
            textBox5 = new TextBox();
            label6 = new Label();
            textBox6 = new TextBox();
            label7 = new Label();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            button4 = new Button();
            richTextBox5 = new RichTextBox();
            label13 = new Label();
            richTextBox4 = new RichTextBox();
            label12 = new Label();
            richTextBox3 = new RichTextBox();
            label11 = new Label();
            richTextBox2 = new RichTextBox();
            label10 = new Label();
            tabPage1 = new TabPage();
            label14 = new Label();
            richTextBox6 = new RichTextBox();
            label9 = new Label();
            label8 = new Label();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(53, 33);
            label1.Name = "label1";
            label1.Size = new Size(111, 24);
            label1.TabIndex = 0;
            label1.Text = "原始点Excel:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(179, 31);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(581, 30);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(794, 30);
            button1.Name = "button1";
            button1.Size = new Size(57, 30);
            button1.TabIndex = 2;
            button1.Text = "浏览";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(55, 96);
            label2.Name = "label2";
            label2.Size = new Size(561, 24);
            label2.TabIndex = 3;
            label2.Text = "原始点投影信息（支持Esri、Proj4、ESPGCode、其他Code写法）：";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(53, 137);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(880, 135);
            richTextBox1.TabIndex = 4;
            richTextBox1.Text = "";
            // 
            // button2
            // 
            button2.Location = new Point(396, 673);
            button2.Name = "button2";
            button2.Size = new Size(188, 66);
            button2.TabIndex = 7;
            button2.Text = "执行";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(62, 465);
            label3.Name = "label3";
            label3.Size = new Size(93, 24);
            label3.TabIndex = 8;
            label3.Text = "输出Excel:";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(170, 462);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(581, 30);
            textBox2.TabIndex = 9;
            // 
            // button3
            // 
            button3.Location = new Point(780, 459);
            button3.Name = "button3";
            button3.Size = new Size(57, 30);
            button3.TabIndex = 10;
            button3.Text = "浏览";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(44, 521);
            label4.Name = "label4";
            label4.Size = new Size(127, 24);
            label4.TabIndex = 11;
            label4.Text = "原始点x字段：";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(170, 518);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(102, 30);
            textBox3.TabIndex = 12;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(170, 573);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(102, 30);
            textBox4.TabIndex = 14;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(44, 576);
            label5.Name = "label5";
            label5.Size = new Size(128, 24);
            label5.TabIndex = 13;
            label5.Text = "原始点y字段：";
            // 
            // textBox5
            // 
            textBox5.Location = new Point(475, 518);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(102, 30);
            textBox5.TabIndex = 16;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(322, 521);
            label6.Name = "label6";
            label6.Size = new Size(163, 24);
            label6.TabIndex = 15;
            label6.Text = "原始点结果x字段：";
            // 
            // textBox6
            // 
            textBox6.Location = new Point(475, 573);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(102, 30);
            textBox6.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(322, 576);
            label7.Name = "label7";
            label7.Size = new Size(164, 24);
            label7.TabIndex = 17;
            label7.Text = "原始点结果y字段：";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(964, 803);
            tabControl1.TabIndex = 19;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(richTextBox5);
            tabPage2.Controls.Add(label13);
            tabPage2.Controls.Add(richTextBox4);
            tabPage2.Controls.Add(label12);
            tabPage2.Controls.Add(richTextBox3);
            tabPage2.Controls.Add(label11);
            tabPage2.Controls.Add(richTextBox2);
            tabPage2.Controls.Add(label10);
            tabPage2.Location = new Point(4, 33);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(956, 766);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "坐标点处理";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(381, 698);
            button4.Name = "button4";
            button4.Size = new Size(168, 46);
            button4.TabIndex = 8;
            button4.Text = "计算结果";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // richTextBox5
            // 
            richTextBox5.Location = new Point(17, 373);
            richTextBox5.Name = "richTextBox5";
            richTextBox5.Size = new Size(919, 133);
            richTextBox5.TabIndex = 3;
            richTextBox5.Text = "";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(17, 334);
            label13.Name = "label13";
            label13.Size = new Size(678, 24);
            label13.TabIndex = 6;
            label13.Text = "输出点投影信息（默认WGS84）（支持Esri、Proj4、ESPGCode、其他Code写法）";
            // 
            // richTextBox4
            // 
            richTextBox4.Location = new Point(20, 197);
            richTextBox4.Name = "richTextBox4";
            richTextBox4.Size = new Size(919, 133);
            richTextBox4.TabIndex = 2;
            richTextBox4.Text = "";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(20, 158);
            label12.Name = "label12";
            label12.Size = new Size(543, 24);
            label12.TabIndex = 4;
            label12.Text = "原始点投影信息（支持Esri、Proj4、ESPGCode、其他Code写法）";
            // 
            // richTextBox3
            // 
            richTextBox3.Location = new Point(17, 540);
            richTextBox3.Name = "richTextBox3";
            richTextBox3.Size = new Size(922, 119);
            richTextBox3.TabIndex = 4;
            richTextBox3.Text = "";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(21, 513);
            label11.Name = "label11";
            label11.Size = new Size(100, 24);
            label11.TabIndex = 2;
            label11.Text = "输出点坐标";
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(24, 55);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(919, 99);
            richTextBox2.TabIndex = 1;
            richTextBox2.Text = "";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(24, 19);
            label10.Name = "label10";
            label10.Size = new Size(172, 24);
            label10.TabIndex = 0;
            label10.Text = "原始点坐标空格隔开";
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label14);
            tabPage1.Controls.Add(richTextBox6);
            tabPage1.Controls.Add(label9);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(textBox6);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(label7);
            tabPage1.Controls.Add(textBox1);
            tabPage1.Controls.Add(textBox5);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(label6);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(textBox4);
            tabPage1.Controls.Add(richTextBox1);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(textBox3);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(button3);
            tabPage1.Location = new Point(4, 33);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(956, 766);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "根据Excel批量处理坐标点";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(56, 291);
            label14.Name = "label14";
            label14.Size = new Size(696, 24);
            label14.TabIndex = 21;
            label14.Text = "输出点投影信息（默认WGS84）（支持Esri、Proj4、ESPGCode、其他Code写法）：";
            // 
            // richTextBox6
            // 
            richTextBox6.Location = new Point(53, 318);
            richTextBox6.Name = "richTextBox6";
            richTextBox6.Size = new Size(871, 135);
            richTextBox6.TabIndex = 22;
            richTextBox6.Text = "";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(584, 576);
            label9.Name = "label9";
            label9.Size = new Size(353, 24);
            label9.TabIndex = 20;
            label9.Text = "（可为空，xy都不为空时会生成比对信息）";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(584, 521);
            label8.Name = "label8";
            label8.Size = new Size(353, 24);
            label8.TabIndex = 19;
            label8.Text = "（可为空，xy都不为空时会生成比对信息）";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(964, 803);
            Controls.Add(tabControl1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "坐标转换小程序";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Label label2;
        private RichTextBox richTextBox1;
        private Button button2;
        private Label label3;
        private TextBox textBox2;
        private Button button3;
        private Label label4;
        private TextBox textBox3;
        private TextBox textBox4;
        private Label label5;
        private TextBox textBox5;
        private Label label6;
        private TextBox textBox6;
        private Label label7;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label9;
        private Label label8;
        private Label label10;
        private RichTextBox richTextBox5;
        private Label label13;
        private RichTextBox richTextBox4;
        private Label label12;
        private RichTextBox richTextBox3;
        private Label label11;
        private RichTextBox richTextBox2;
        private Button button4;
        private Label label14;
        private RichTextBox richTextBox6;
    }
}
