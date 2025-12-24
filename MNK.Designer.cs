namespace ProgramLab
{
    partial class MNK
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnLoadExcel = new System.Windows.Forms.Button();
            this.btnLoadGoogle = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLinear = new System.Windows.Forms.TextBox();
            this.txtQuadratic = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLinearError = new System.Windows.Forms.TextBox();
            this.txtQuadraticError = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numPoints = new System.Windows.Forms.NumericUpDown();
            this.minXValue = new System.Windows.Forms.NumericUpDown();
            this.maxXValue = new System.Windows.Forms.NumericUpDown();
            this.minYValue = new System.Windows.Forms.NumericUpDown();
            this.maxYValue = new System.Windows.Forms.NumericUpDown();
            this.quadraticRadio = new System.Windows.Forms.RadioButton();
            this.linearRadio = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minXValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minYValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYValue)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnX,
            this.ColumnY});
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(300, 300);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView_CellValidating);
            this.dataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValueChanged);
            // 
            // ColumnX
            // 
            this.ColumnX.HeaderText = "X";
            this.ColumnX.Name = "ColumnX";
            this.ColumnX.Width = 120;
            // 
            // ColumnY
            // 
            this.ColumnY.HeaderText = "Y";
            this.ColumnY.Name = "ColumnY";
            this.ColumnY.Width = 120;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(318, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(500, 300);
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // btnCalculate
            // 
            this.btnCalculate.Location = new System.Drawing.Point(12, 318);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(140, 30);
            this.btnCalculate.TabIndex = 2;
            this.btnCalculate.Text = "Рассчитать";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(158, 318);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(154, 30);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(12, 354);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(140, 30);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Сгенерировать данные";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnLoadExcel
            // 
            this.btnLoadExcel.Location = new System.Drawing.Point(158, 354);
            this.btnLoadExcel.Name = "btnLoadExcel";
            this.btnLoadExcel.Size = new System.Drawing.Size(154, 30);
            this.btnLoadExcel.TabIndex = 5;
            this.btnLoadExcel.Text = "Загрузить из файла";
            this.btnLoadExcel.UseVisualStyleBackColor = true;
            this.btnLoadExcel.Click += new System.EventHandler(this.btnLoadExcel_Click);
            // 
            // btnLoadGoogle
            // 
            this.btnLoadGoogle.Location = new System.Drawing.Point(12, 390);
            this.btnLoadGoogle.Name = "btnLoadGoogle";
            this.btnLoadGoogle.Size = new System.Drawing.Size(300, 30);
            this.btnLoadGoogle.TabIndex = 6;
            this.btnLoadGoogle.Text = "Загрузить из Google Sheets";
            this.btnLoadGoogle.UseVisualStyleBackColor = true;
            this.btnLoadGoogle.Click += new System.EventHandler(this.btnLoadGoogle_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(318, 320);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Линейная (n=1):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(318, 360);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Квадратичная (n=2):";
            // 
            // txtLinear
            // 
            this.txtLinear.BackColor = System.Drawing.Color.White;
            this.txtLinear.Location = new System.Drawing.Point(453, 317);
            this.txtLinear.Name = "txtLinear";
            this.txtLinear.ReadOnly = true;
            this.txtLinear.Size = new System.Drawing.Size(200, 20);
            this.txtLinear.TabIndex = 9;
            // 
            // txtQuadratic
            // 
            this.txtQuadratic.BackColor = System.Drawing.Color.White;
            this.txtQuadratic.Location = new System.Drawing.Point(453, 357);
            this.txtQuadratic.Name = "txtQuadratic";
            this.txtQuadratic.ReadOnly = true;
            this.txtQuadratic.Size = new System.Drawing.Size(200, 20);
            this.txtQuadratic.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(659, 320);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Ошибка:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(659, 360);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Ошибка:";
            // 
            // txtLinearError
            // 
            this.txtLinearError.BackColor = System.Drawing.Color.White;
            this.txtLinearError.Location = new System.Drawing.Point(721, 317);
            this.txtLinearError.Name = "txtLinearError";
            this.txtLinearError.ReadOnly = true;
            this.txtLinearError.Size = new System.Drawing.Size(97, 20);
            this.txtLinearError.TabIndex = 13;
            // 
            // txtQuadraticError
            // 
            this.txtQuadraticError.BackColor = System.Drawing.Color.White;
            this.txtQuadraticError.Location = new System.Drawing.Point(721, 357);
            this.txtQuadraticError.Name = "txtQuadraticError";
            this.txtQuadraticError.ReadOnly = true;
            this.txtQuadraticError.Size = new System.Drawing.Size(97, 20);
            this.txtQuadraticError.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numPoints);
            this.groupBox1.Controls.Add(this.minXValue);
            this.groupBox1.Controls.Add(this.maxXValue);
            this.groupBox1.Controls.Add(this.minYValue);
            this.groupBox1.Controls.Add(this.maxYValue);
            this.groupBox1.Controls.Add(this.quadraticRadio);
            this.groupBox1.Controls.Add(this.linearRadio);
            this.groupBox1.Location = new System.Drawing.Point(12, 426);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(806, 120);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки генерации";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(550, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Модель генерации:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(213, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Диапазон шума Y (мин/макс):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(243, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Диапазон X (мин/макс):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Количество точек:";
            // 
            // numPoints
            // 
            this.numPoints.Location = new System.Drawing.Point(140, 30);
            this.numPoints.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numPoints.Name = "numPoints";
            this.numPoints.Size = new System.Drawing.Size(70, 20);
            this.numPoints.TabIndex = 0;
            this.numPoints.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // minXValue
            // 
            this.minXValue.DecimalPlaces = 2;
            this.minXValue.Location = new System.Drawing.Point(380, 30);
            this.minXValue.Name = "minXValue";
            this.minXValue.Size = new System.Drawing.Size(70, 20);
            this.minXValue.TabIndex = 0;
            // 
            // maxXValue
            // 
            this.maxXValue.DecimalPlaces = 2;
            this.maxXValue.Location = new System.Drawing.Point(470, 30);
            this.maxXValue.Name = "maxXValue";
            this.maxXValue.Size = new System.Drawing.Size(70, 20);
            this.maxXValue.TabIndex = 1;
            this.maxXValue.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // minYValue
            // 
            this.minYValue.DecimalPlaces = 2;
            this.minYValue.Location = new System.Drawing.Point(380, 60);
            this.minYValue.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.minYValue.Name = "minYValue";
            this.minYValue.Size = new System.Drawing.Size(70, 20);
            this.minYValue.TabIndex = 2;
            this.minYValue.Value = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            // 
            // maxYValue
            // 
            this.maxYValue.DecimalPlaces = 2;
            this.maxYValue.Location = new System.Drawing.Point(470, 60);
            this.maxYValue.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.maxYValue.Name = "maxYValue";
            this.maxYValue.Size = new System.Drawing.Size(70, 20);
            this.maxYValue.TabIndex = 3;
            this.maxYValue.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // quadraticRadio
            // 
            this.quadraticRadio.AutoSize = true;
            this.quadraticRadio.Location = new System.Drawing.Point(560, 60);
            this.quadraticRadio.Name = "quadraticRadio";
            this.quadraticRadio.Size = new System.Drawing.Size(96, 17);
            this.quadraticRadio.TabIndex = 1;
            this.quadraticRadio.TabStop = true;
            this.quadraticRadio.Text = "Квадратичная";
            this.quadraticRadio.UseVisualStyleBackColor = true;
            // 
            // linearRadio
            // 
            this.linearRadio.AutoSize = true;
            this.linearRadio.Checked = true;
            this.linearRadio.Location = new System.Drawing.Point(560, 30);
            this.linearRadio.Name = "linearRadio";
            this.linearRadio.Size = new System.Drawing.Size(75, 17);
            this.linearRadio.TabIndex = 0;
            this.linearRadio.TabStop = true;
            this.linearRadio.Text = "Линейная";
            this.linearRadio.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(666, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 30);
            this.button1.TabIndex = 16;
            this.button1.Text = "Назад";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MNK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(830, 558);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtQuadraticError);
            this.Controls.Add(this.txtLinearError);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtQuadratic);
            this.Controls.Add(this.txtLinear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadGoogle);
            this.Controls.Add(this.btnLoadExcel);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MNK";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Аппроксимация методом наименьших квадратов";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minXValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxXValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minYValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxYValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnY;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnLoadExcel;
        private System.Windows.Forms.Button btnLoadGoogle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLinear;
        private System.Windows.Forms.TextBox txtQuadratic;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLinearError;
        private System.Windows.Forms.TextBox txtQuadraticError;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPoints;
        private System.Windows.Forms.NumericUpDown minXValue;
        private System.Windows.Forms.NumericUpDown maxXValue;
        private System.Windows.Forms.NumericUpDown minYValue;
        private System.Windows.Forms.NumericUpDown maxYValue;
        private System.Windows.Forms.RadioButton quadraticRadio;
        private System.Windows.Forms.RadioButton linearRadio;
        private System.Windows.Forms.Button button1;
    }
}