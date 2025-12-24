namespace ProgramLab
{
    partial class Integral
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.textBoxFunction = new System.Windows.Forms.TextBox();
            this.buttonClearIntegral = new System.Windows.Forms.Button();
            this.buttonCalculateIntegral = new System.Windows.Forms.Button();
            this.chartIntegration = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxEpsilon = new System.Windows.Forms.TextBox();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.textBoxA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelA = new System.Windows.Forms.Label();
            this.labelF = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxN = new System.Windows.Forms.TextBox();
            this.checkBoxAutoN = new System.Windows.Forms.CheckBox();
            this.buttonStopIntegral = new System.Windows.Forms.Button();
            this.radioButtonRectangles = new System.Windows.Forms.RadioButton();
            this.radioButtonTrapezoids = new System.Windows.Forms.RadioButton();
            this.radioButtonSimpson = new System.Windows.Forms.RadioButton();
            this.labelInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartIntegration)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxFunction
            // 
            this.textBoxFunction.Location = new System.Drawing.Point(44, 13);
            this.textBoxFunction.Name = "textBoxFunction";
            this.textBoxFunction.Size = new System.Drawing.Size(100, 20);
            this.textBoxFunction.TabIndex = 48;
            // 
            // buttonClearIntegral
            // 
            this.buttonClearIntegral.Location = new System.Drawing.Point(105, 384);
            this.buttonClearIntegral.Name = "buttonClearIntegral";
            this.buttonClearIntegral.Size = new System.Drawing.Size(87, 53);
            this.buttonClearIntegral.TabIndex = 46;
            this.buttonClearIntegral.Text = "Очистить";
            this.buttonClearIntegral.UseVisualStyleBackColor = true;
            // 
            // buttonCalculateIntegral
            // 
            this.buttonCalculateIntegral.Location = new System.Drawing.Point(12, 384);
            this.buttonCalculateIntegral.Name = "buttonCalculateIntegral";
            this.buttonCalculateIntegral.Size = new System.Drawing.Size(87, 53);
            this.buttonCalculateIntegral.TabIndex = 45;
            this.buttonCalculateIntegral.Text = "Расчитать";
            this.buttonCalculateIntegral.UseVisualStyleBackColor = true;
            // 
            // chartIntegration
            // 
            chartArea1.Name = "ChartArea1";
            this.chartIntegration.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartIntegration.Legends.Add(legend1);
            this.chartIntegration.Location = new System.Drawing.Point(307, 12);
            this.chartIntegration.Name = "chartIntegration";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chartIntegration.Series.Add(series1);
            this.chartIntegration.Size = new System.Drawing.Size(494, 377);
            this.chartIntegration.TabIndex = 44;
            this.chartIntegration.Text = "chart1";
            // 
            // textBoxResult
            // 
            this.textBoxResult.Location = new System.Drawing.Point(74, 325);
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.Size = new System.Drawing.Size(100, 20);
            this.textBoxResult.TabIndex = 42;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 328);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 13);
            this.label9.TabIndex = 40;
            this.label9.Text = "Реультат=";
            // 
            // textBoxEpsilon
            // 
            this.textBoxEpsilon.Location = new System.Drawing.Point(44, 112);
            this.textBoxEpsilon.Name = "textBoxEpsilon";
            this.textBoxEpsilon.Size = new System.Drawing.Size(100, 20);
            this.textBoxEpsilon.TabIndex = 36;
            // 
            // textBoxB
            // 
            this.textBoxB.Location = new System.Drawing.Point(44, 69);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(100, 20);
            this.textBoxB.TabIndex = 35;
            // 
            // textBoxA
            // 
            this.textBoxA.Location = new System.Drawing.Point(44, 43);
            this.textBoxA.Name = "textBoxA";
            this.textBoxA.Size = new System.Drawing.Size(100, 20);
            this.textBoxA.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Введите количество знаков после запятой";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "E=";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "B=";
            // 
            // labelA
            // 
            this.labelA.AutoSize = true;
            this.labelA.Location = new System.Drawing.Point(9, 46);
            this.labelA.Name = "labelA";
            this.labelA.Size = new System.Drawing.Size(20, 13);
            this.labelA.TabIndex = 26;
            this.labelA.Text = "A=";
            // 
            // labelF
            // 
            this.labelF.AutoSize = true;
            this.labelF.Location = new System.Drawing.Point(8, 20);
            this.labelF.Name = "labelF";
            this.labelF.Size = new System.Drawing.Size(30, 13);
            this.labelF.TabIndex = 25;
            this.labelF.Text = "F(x)=";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 257);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "N=";
            // 
            // textBoxN
            // 
            this.textBoxN.Location = new System.Drawing.Point(31, 254);
            this.textBoxN.Name = "textBoxN";
            this.textBoxN.Size = new System.Drawing.Size(100, 20);
            this.textBoxN.TabIndex = 51;
            // 
            // checkBoxAutoN
            // 
            this.checkBoxAutoN.AutoSize = true;
            this.checkBoxAutoN.Location = new System.Drawing.Point(10, 284);
            this.checkBoxAutoN.Name = "checkBoxAutoN";
            this.checkBoxAutoN.Size = new System.Drawing.Size(121, 17);
            this.checkBoxAutoN.TabIndex = 52;
            this.checkBoxAutoN.Text = "Автоматическое N";
            this.checkBoxAutoN.UseVisualStyleBackColor = true;
            // 
            // buttonStopIntegral
            // 
            this.buttonStopIntegral.Location = new System.Drawing.Point(198, 385);
            this.buttonStopIntegral.Name = "buttonStopIntegral";
            this.buttonStopIntegral.Size = new System.Drawing.Size(87, 53);
            this.buttonStopIntegral.TabIndex = 53;
            this.buttonStopIntegral.Text = "Стоп";
            this.buttonStopIntegral.UseVisualStyleBackColor = true;
            // 
            // radioButtonRectangles
            // 
            this.radioButtonRectangles.AutoSize = true;
            this.radioButtonRectangles.Location = new System.Drawing.Point(10, 144);
            this.radioButtonRectangles.Name = "radioButtonRectangles";
            this.radioButtonRectangles.Size = new System.Drawing.Size(150, 17);
            this.radioButtonRectangles.TabIndex = 54;
            this.radioButtonRectangles.TabStop = true;
            this.radioButtonRectangles.Text = "Метод прямоугольников";
            this.radioButtonRectangles.UseVisualStyleBackColor = true;
            // 
            // radioButtonTrapezoids
            // 
            this.radioButtonTrapezoids.AutoSize = true;
            this.radioButtonTrapezoids.Location = new System.Drawing.Point(10, 167);
            this.radioButtonTrapezoids.Name = "radioButtonTrapezoids";
            this.radioButtonTrapezoids.Size = new System.Drawing.Size(109, 17);
            this.radioButtonTrapezoids.TabIndex = 55;
            this.radioButtonTrapezoids.TabStop = true;
            this.radioButtonTrapezoids.Text = "Метод Трапеций";
            this.radioButtonTrapezoids.UseVisualStyleBackColor = true;
            // 
            // radioButtonSimpson
            // 
            this.radioButtonSimpson.AutoSize = true;
            this.radioButtonSimpson.Location = new System.Drawing.Point(10, 190);
            this.radioButtonSimpson.Name = "radioButtonSimpson";
            this.radioButtonSimpson.Size = new System.Drawing.Size(105, 17);
            this.radioButtonSimpson.TabIndex = 56;
            this.radioButtonSimpson.TabStop = true;
            this.radioButtonSimpson.Text = "Метод Симсона";
            this.radioButtonSimpson.UseVisualStyleBackColor = true;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(392, 404);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(21, 13);
            this.labelInfo.TabIndex = 57;
            this.labelInfo.Text = "N=";
            // 
            // Integral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.radioButtonSimpson);
            this.Controls.Add(this.radioButtonTrapezoids);
            this.Controls.Add(this.radioButtonRectangles);
            this.Controls.Add(this.buttonStopIntegral);
            this.Controls.Add(this.checkBoxAutoN);
            this.Controls.Add(this.textBoxN);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxFunction);
            this.Controls.Add(this.buttonClearIntegral);
            this.Controls.Add(this.buttonCalculateIntegral);
            this.Controls.Add(this.chartIntegration);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxEpsilon);
            this.Controls.Add(this.textBoxB);
            this.Controls.Add(this.textBoxA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelA);
            this.Controls.Add(this.labelF);
            this.Name = "Integral";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chartIntegration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFunction;
        private System.Windows.Forms.Button buttonClearIntegral;
        private System.Windows.Forms.Button buttonCalculateIntegral;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartIntegration;
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxEpsilon;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.TextBox textBoxA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelA;
        private System.Windows.Forms.Label labelF;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxN;
        private System.Windows.Forms.CheckBox checkBoxAutoN;
        private System.Windows.Forms.Button buttonStopIntegral;
        private System.Windows.Forms.RadioButton radioButtonRectangles;
        private System.Windows.Forms.RadioButton radioButtonTrapezoids;
        private System.Windows.Forms.RadioButton radioButtonSimpson;
        private System.Windows.Forms.Label labelInfo;
    }
}