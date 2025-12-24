namespace ProgramLab
{
    partial class SLAU
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
            this.dataGridViewInput = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromGoogleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gaussToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jordanGaussToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cramerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMethodsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numericUpDownN = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnStop = new System.Windows.Forms.Button();
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.labelGaussTime = new System.Windows.Forms.Label();
            this.labelJordanTime = new System.Windows.Forms.Label();
            this.labelCramerTime = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInput)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewInput
            // 
            this.dataGridViewInput.AllowUserToAddRows = false;
            this.dataGridViewInput.AllowUserToDeleteRows = false;
            this.dataGridViewInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewInput.Location = new System.Drawing.Point(12, 85);
            this.dataGridViewInput.Name = "dataGridViewInput";
            this.dataGridViewInput.RowHeadersWidth = 51;
            this.dataGridViewInput.Size = new System.Drawing.Size(550, 400);
            this.dataGridViewInput.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.calculateToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1002, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromExcelToolStripMenuItem,
            this.loadFromGoogleToolStripMenuItem,
            this.generateToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileToolStripMenuItem.Text = "Файл";
            // 
            // loadFromExcelToolStripMenuItem
            // 
            this.loadFromExcelToolStripMenuItem.Name = "loadFromExcelToolStripMenuItem";
            this.loadFromExcelToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadFromExcelToolStripMenuItem.Text = "Загрузить из Excel";
            this.loadFromExcelToolStripMenuItem.Click += new System.EventHandler(this.LoadFromExcelToolStripMenuItem_Click);
            // 
            // loadFromGoogleToolStripMenuItem
            // 
            this.loadFromGoogleToolStripMenuItem.Name = "loadFromGoogleToolStripMenuItem";
            this.loadFromGoogleToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadFromGoogleToolStripMenuItem.Text = "Загрузить из Google";
            this.loadFromGoogleToolStripMenuItem.Click += new System.EventHandler(this.LoadFromGoogleToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.generateToolStripMenuItem.Text = "Сгенерировать";
            this.generateToolStripMenuItem.Click += new System.EventHandler(this.GenerateToolStripMenuItem_Click);
            // 
            // calculateToolStripMenuItem
            // 
            this.calculateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gaussToolStripMenuItem,
            this.jordanGaussToolStripMenuItem,
            this.cramerToolStripMenuItem,
            this.allMethodsToolStripMenuItem});
            this.calculateToolStripMenuItem.Name = "calculateToolStripMenuItem";
            this.calculateToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.calculateToolStripMenuItem.Text = "Вычислить";
            // 
            // gaussToolStripMenuItem
            // 
            this.gaussToolStripMenuItem.Name = "gaussToolStripMenuItem";
            this.gaussToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.gaussToolStripMenuItem.Text = "Метод Гаусса";
            this.gaussToolStripMenuItem.Click += new System.EventHandler(this.GaussToolStripMenuItem_Click);
            // 
            // jordanGaussToolStripMenuItem
            // 
            this.jordanGaussToolStripMenuItem.Name = "jordanGaussToolStripMenuItem";
            this.jordanGaussToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.jordanGaussToolStripMenuItem.Text = "Метод Жордана-Гаусса";
            this.jordanGaussToolStripMenuItem.Click += new System.EventHandler(this.JordanGaussToolStripMenuItem_Click);
            // 
            // cramerToolStripMenuItem
            // 
            this.cramerToolStripMenuItem.Name = "cramerToolStripMenuItem";
            this.cramerToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cramerToolStripMenuItem.Text = "Метод Крамера";
            this.cramerToolStripMenuItem.Click += new System.EventHandler(this.CramerToolStripMenuItem_Click);
            // 
            // allMethodsToolStripMenuItem
            // 
            this.allMethodsToolStripMenuItem.Name = "allMethodsToolStripMenuItem";
            this.allMethodsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.allMethodsToolStripMenuItem.Text = "Все методы сравнить";
            this.allMethodsToolStripMenuItem.Click += new System.EventHandler(this.AllMethodsToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.clearToolStripMenuItem.Text = "Очистить";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // numericUpDownN
            // 
            this.numericUpDownN.Location = new System.Drawing.Point(111, 27);
            this.numericUpDownN.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownN.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownN.Name = "numericUpDownN";
            this.numericUpDownN.Size = new System.Drawing.Size(100, 20);
            this.numericUpDownN.TabIndex = 3;
            this.numericUpDownN.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDownN.ValueChanged += new System.EventHandler(this.NumericUpDownN_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Размер N (2-50):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Матрица A и вектор B (A|B в таблице):";
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTime.Location = new System.Drawing.Point(12, 510);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(144, 15);
            this.labelTime.TabIndex = 7;
            this.labelTime.Text = "Время выполнения: ";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 540);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(550, 20);
            this.progressBar.TabIndex = 8;
            this.progressBar.Visible = false;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(12, 570);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(120, 30);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Остановить";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // dataGridViewResults
            // 
            this.dataGridViewResults.AllowUserToAddRows = false;
            this.dataGridViewResults.AllowUserToDeleteRows = false;
            this.dataGridViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.Location = new System.Drawing.Point(580, 85);
            this.dataGridViewResults.Name = "dataGridViewResults";
            this.dataGridViewResults.ReadOnly = true;
            this.dataGridViewResults.RowHeadersWidth = 51;
            this.dataGridViewResults.Size = new System.Drawing.Size(400, 515);
            this.dataGridViewResults.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(580, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(142, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Результаты всех методов:";
            // 
            // labelGaussTime
            // 
            this.labelGaussTime.AutoSize = true;
            this.labelGaussTime.Location = new System.Drawing.Point(12, 610);
            this.labelGaussTime.Name = "labelGaussTime";
            this.labelGaussTime.Size = new System.Drawing.Size(118, 13);
            this.labelGaussTime.TabIndex = 12;
            this.labelGaussTime.Text = "Гаусса: не вычислено";
            // 
            // labelJordanTime
            // 
            this.labelJordanTime.AutoSize = true;
            this.labelJordanTime.Location = new System.Drawing.Point(12, 630);
            this.labelJordanTime.Name = "labelJordanTime";
            this.labelJordanTime.Size = new System.Drawing.Size(168, 13);
            this.labelJordanTime.TabIndex = 13;
            this.labelJordanTime.Text = "Жордана-Гаусса: не вычислено";
            // 
            // labelCramerTime
            // 
            this.labelCramerTime.AutoSize = true;
            this.labelCramerTime.Location = new System.Drawing.Point(12, 650);
            this.labelCramerTime.Name = "labelCramerTime";
            this.labelCramerTime.Size = new System.Drawing.Size(128, 13);
            this.labelCramerTime.TabIndex = 14;
            this.labelCramerTime.Text = "Крамера: не вычислено";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(150, 577);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(117, 13);
            this.labelStatus.TabIndex = 15;
            this.labelStatus.Text = "Готов к вычислениям";
            // 
            // SLAU
            // 
            this.ClientSize = new System.Drawing.Size(1002, 688);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelCramerTime);
            this.Controls.Add(this.labelJordanTime);
            this.Controls.Add(this.labelGaussTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridViewResults);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownN);
            this.Controls.Add(this.dataGridViewInput);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1018, 727);
            this.Name = "SLAU";
            this.Text = "Решение СЛАУ A·X + B = 0";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInput)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewInput;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromGoogleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gaussToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jordanGaussToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cramerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allMethodsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown numericUpDownN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.DataGridView dataGridViewResults;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelGaussTime;
        private System.Windows.Forms.Label labelJordanTime;
        private System.Windows.Forms.Label labelCramerTime;
        private System.Windows.Forms.Label labelStatus;
    }
}