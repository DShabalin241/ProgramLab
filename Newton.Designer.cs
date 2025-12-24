namespace ProgramLab
{
    partial class Newton
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.buttonBack = new System.Windows.Forms.Button();
            this.textBoxF = new System.Windows.Forms.TextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.chartFunc = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBoxYMin = new System.Windows.Forms.TextBox();
            this.textBoxXMin = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxE = new System.Windows.Forms.TextBox();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.textBoxA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelA = new System.Windows.Forms.Label();
            this.labelF = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartFunc)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBack
            // 
            this.buttonBack.Location = new System.Drawing.Point(718, 414);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBack.TabIndex = 50;
            this.buttonBack.Text = "Назад";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click_1);
            // 
            // textBoxF
            // 
            this.textBoxF.Location = new System.Drawing.Point(44, 13);
            this.textBoxF.Name = "textBoxF";
            this.textBoxF.Size = new System.Drawing.Size(100, 20);
            this.textBoxF.TabIndex = 49;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(198, 384);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(87, 53);
            this.buttonStop.TabIndex = 48;
            this.buttonStop.Text = "Стоп";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(105, 384);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(87, 53);
            this.buttonClear.TabIndex = 47;
            this.buttonClear.Text = "Очистить";
            this.buttonClear.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 384);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(87, 53);
            this.buttonStart.TabIndex = 46;
            this.buttonStart.Text = "Расчитать";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // chartFunc
            // 
            chartArea2.Name = "ChartArea1";
            this.chartFunc.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartFunc.Legends.Add(legend2);
            this.chartFunc.Location = new System.Drawing.Point(299, 23);
            this.chartFunc.Name = "chartFunc";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartFunc.Series.Add(series2);
            this.chartFunc.Size = new System.Drawing.Size(494, 377);
            this.chartFunc.TabIndex = 45;
            this.chartFunc.Text = "chart1";
            // 
            // textBoxYMin
            // 
            this.textBoxYMin.Location = new System.Drawing.Point(34, 333);
            this.textBoxYMin.Name = "textBoxYMin";
            this.textBoxYMin.Size = new System.Drawing.Size(100, 20);
            this.textBoxYMin.TabIndex = 44;
            // 
            // textBoxXMin
            // 
            this.textBoxXMin.Location = new System.Drawing.Point(34, 300);
            this.textBoxXMin.Name = "textBoxXMin";
            this.textBoxXMin.Size = new System.Drawing.Size(100, 20);
            this.textBoxXMin.TabIndex = 43;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 333);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(20, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Y=";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 303);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "X=";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 275);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 13);
            this.label8.TabIndex = 40;
            this.label8.Text = "Минимум функции:";
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(44, 170);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(100, 20);
            this.textBoxX.TabIndex = 38;
            // 
            // textBoxE
            // 
            this.textBoxE.Location = new System.Drawing.Point(44, 112);
            this.textBoxE.Name = "textBoxE";
            this.textBoxE.Size = new System.Drawing.Size(100, 20);
            this.textBoxE.TabIndex = 37;
            // 
            // textBoxB
            // 
            this.textBoxB.Location = new System.Drawing.Point(44, 69);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(100, 20);
            this.textBoxB.TabIndex = 36;
            // 
            // textBoxA
            // 
            this.textBoxA.Location = new System.Drawing.Point(44, 43);
            this.textBoxA.Name = "textBoxA";
            this.textBoxA.Size = new System.Drawing.Size(100, 20);
            this.textBoxA.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(167, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Введите начальную координату";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Введите количество знаков после запятой";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "X=";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "E=";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "B=";
            // 
            // labelA
            // 
            this.labelA.AutoSize = true;
            this.labelA.Location = new System.Drawing.Point(9, 46);
            this.labelA.Name = "labelA";
            this.labelA.Size = new System.Drawing.Size(20, 13);
            this.labelA.TabIndex = 27;
            this.labelA.Text = "A=";
            // 
            // labelF
            // 
            this.labelF.AutoSize = true;
            this.labelF.Location = new System.Drawing.Point(8, 20);
            this.labelF.Name = "labelF";
            this.labelF.Size = new System.Drawing.Size(30, 13);
            this.labelF.TabIndex = 26;
            this.labelF.Text = "F(x)=";
            // 
            // Newton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.textBoxF);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.chartFunc);
            this.Controls.Add(this.textBoxYMin);
            this.Controls.Add(this.textBoxXMin);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.textBoxE);
            this.Controls.Add(this.textBoxB);
            this.Controls.Add(this.textBoxA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelA);
            this.Controls.Add(this.labelF);
            this.Name = "Newton";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chartFunc)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.TextBox textBoxF;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartFunc;
        private System.Windows.Forms.TextBox textBoxYMin;
        private System.Windows.Forms.TextBox textBoxXMin;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.TextBox textBoxE;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.TextBox textBoxA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelA;
        private System.Windows.Forms.Label labelF;
    }
}