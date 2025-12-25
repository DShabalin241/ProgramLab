namespace ProgramLab
{
    partial class Sort
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromExcelToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox algorithmsGroupBox;
        private System.Windows.Forms.CheckBox bubbleSortCheckBox;
        private System.Windows.Forms.CheckBox insertionSortCheckBox;
        private System.Windows.Forms.CheckBox shakerSortCheckBox;
        private System.Windows.Forms.CheckBox quickSortCheckBox;
        private System.Windows.Forms.CheckBox bogoSortCheckBox;
        private System.Windows.Forms.GroupBox sortOrderGroupBox;
        private System.Windows.Forms.RadioButton ascendingRadioButton;
        private System.Windows.Forms.RadioButton descendingRadioButton;
        private System.Windows.Forms.ListBox resultsListBox;
        private System.Windows.Forms.TextBox sortedArrayTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button startSortingButton;
        private System.Windows.Forms.Button generateDataButton;
        private System.Windows.Forms.Button clearDataButton;
        private System.Windows.Forms.NumericUpDown countNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.algorithmsGroupBox = new System.Windows.Forms.GroupBox();
            this.bogoSortCheckBox = new System.Windows.Forms.CheckBox();
            this.quickSortCheckBox = new System.Windows.Forms.CheckBox();
            this.shakerSortCheckBox = new System.Windows.Forms.CheckBox();
            this.insertionSortCheckBox = new System.Windows.Forms.CheckBox();
            this.bubbleSortCheckBox = new System.Windows.Forms.CheckBox();
            this.sortOrderGroupBox = new System.Windows.Forms.GroupBox();
            this.descendingRadioButton = new System.Windows.Forms.RadioButton();
            this.ascendingRadioButton = new System.Windows.Forms.RadioButton();
            this.resultsListBox = new System.Windows.Forms.ListBox();
            this.sortedArrayTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.startSortingButton = new System.Windows.Forms.Button();
            this.generateDataButton = new System.Windows.Forms.Button();
            this.clearDataButton = new System.Windows.Forms.Button();
            this.countNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.algorithmsGroupBox.SuspendLayout();
            this.sortOrderGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dataToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.fileToolStripMenuItem.Text = "Файл";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startToolStripMenuItem.Text = "Начать сортировку";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startSortingButton_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stopToolStripMenuItem.Text = "Остановить";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clearToolStripMenuItem.Text = "Очистить все";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Выход";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromExcelToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.dataToolStripMenuItem.Text = "Данные";
            // 
            // loadFromExcelToolStripMenuItem
            // 
            this.loadFromExcelToolStripMenuItem.Name = "loadFromExcelToolStripMenuItem";
            this.loadFromExcelToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.loadFromExcelToolStripMenuItem.Text = "Загрузить из Excel";
            this.loadFromExcelToolStripMenuItem.Click += new System.EventHandler(this.loadFromExcelToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(300, 150);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            // 
            // algorithmsGroupBox
            // 
            this.algorithmsGroupBox.Controls.Add(this.bogoSortCheckBox);
            this.algorithmsGroupBox.Controls.Add(this.quickSortCheckBox);
            this.algorithmsGroupBox.Controls.Add(this.shakerSortCheckBox);
            this.algorithmsGroupBox.Controls.Add(this.insertionSortCheckBox);
            this.algorithmsGroupBox.Controls.Add(this.bubbleSortCheckBox);
            this.algorithmsGroupBox.Location = new System.Drawing.Point(12, 250);
            this.algorithmsGroupBox.Name = "algorithmsGroupBox";
            this.algorithmsGroupBox.Size = new System.Drawing.Size(300, 150);
            this.algorithmsGroupBox.TabIndex = 2;
            this.algorithmsGroupBox.TabStop = false;
            this.algorithmsGroupBox.Text = "Выбор алгоритмов";
            // 
            // bogoSortCheckBox
            // 
            this.bogoSortCheckBox.AutoSize = true;
            this.bogoSortCheckBox.Location = new System.Drawing.Point(160, 100);
            this.bogoSortCheckBox.Name = "bogoSortCheckBox";
            this.bogoSortCheckBox.Size = new System.Drawing.Size(119, 17);
            this.bogoSortCheckBox.TabIndex = 4;
            this.bogoSortCheckBox.Text = "BOGO сортировка";
            this.bogoSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // quickSortCheckBox
            // 
            this.quickSortCheckBox.AutoSize = true;
            this.quickSortCheckBox.Location = new System.Drawing.Point(160, 60);
            this.quickSortCheckBox.Name = "quickSortCheckBox";
            this.quickSortCheckBox.Size = new System.Drawing.Size(132, 17);
            this.quickSortCheckBox.TabIndex = 3;
            this.quickSortCheckBox.Text = "Быстрая сортировка";
            this.quickSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // shakerSortCheckBox
            // 
            this.shakerSortCheckBox.AutoSize = true;
            this.shakerSortCheckBox.Location = new System.Drawing.Point(160, 20);
            this.shakerSortCheckBox.Name = "shakerSortCheckBox";
            this.shakerSortCheckBox.Size = new System.Drawing.Size(145, 17);
            this.shakerSortCheckBox.TabIndex = 2;
            this.shakerSortCheckBox.Text = "Шейкерная сортировка";
            this.shakerSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // insertionSortCheckBox
            // 
            this.insertionSortCheckBox.AutoSize = true;
            this.insertionSortCheckBox.Location = new System.Drawing.Point(20, 100);
            this.insertionSortCheckBox.Name = "insertionSortCheckBox";
            this.insertionSortCheckBox.Size = new System.Drawing.Size(144, 17);
            this.insertionSortCheckBox.TabIndex = 1;
            this.insertionSortCheckBox.Text = "Сортировка вставками";
            this.insertionSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // bubbleSortCheckBox
            // 
            this.bubbleSortCheckBox.AutoSize = true;
            this.bubbleSortCheckBox.Checked = true;
            this.bubbleSortCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bubbleSortCheckBox.Location = new System.Drawing.Point(20, 20);
            this.bubbleSortCheckBox.Name = "bubbleSortCheckBox";
            this.bubbleSortCheckBox.Size = new System.Drawing.Size(157, 17);
            this.bubbleSortCheckBox.TabIndex = 0;
            this.bubbleSortCheckBox.Text = "Пузырьковая сортировка";
            this.bubbleSortCheckBox.UseVisualStyleBackColor = true;
            // 
            // sortOrderGroupBox
            // 
            this.sortOrderGroupBox.Controls.Add(this.descendingRadioButton);
            this.sortOrderGroupBox.Controls.Add(this.ascendingRadioButton);
            this.sortOrderGroupBox.Location = new System.Drawing.Point(12, 410);
            this.sortOrderGroupBox.Name = "sortOrderGroupBox";
            this.sortOrderGroupBox.Size = new System.Drawing.Size(300, 80);
            this.sortOrderGroupBox.TabIndex = 3;
            this.sortOrderGroupBox.TabStop = false;
            this.sortOrderGroupBox.Text = "Порядок сортировки";
            // 
            // descendingRadioButton
            // 
            this.descendingRadioButton.AutoSize = true;
            this.descendingRadioButton.Location = new System.Drawing.Point(160, 35);
            this.descendingRadioButton.Name = "descendingRadioButton";
            this.descendingRadioButton.Size = new System.Drawing.Size(93, 17);
            this.descendingRadioButton.TabIndex = 1;
            this.descendingRadioButton.Text = "По убыванию";
            this.descendingRadioButton.UseVisualStyleBackColor = true;
            // 
            // ascendingRadioButton
            // 
            this.ascendingRadioButton.AutoSize = true;
            this.ascendingRadioButton.Checked = true;
            this.ascendingRadioButton.Location = new System.Drawing.Point(20, 35);
            this.ascendingRadioButton.Name = "ascendingRadioButton";
            this.ascendingRadioButton.Size = new System.Drawing.Size(109, 17);
            this.ascendingRadioButton.TabIndex = 0;
            this.ascendingRadioButton.TabStop = true;
            this.ascendingRadioButton.Text = "По возрастанию";
            this.ascendingRadioButton.UseVisualStyleBackColor = true;
            // 
            // resultsListBox
            // 
            this.resultsListBox.FormattingEnabled = true;
            this.resultsListBox.Location = new System.Drawing.Point(330, 40);
            this.resultsListBox.Name = "resultsListBox";
            this.resultsListBox.Size = new System.Drawing.Size(300, 160);
            this.resultsListBox.TabIndex = 4;
            // 
            // sortedArrayTextBox
            // 
            this.sortedArrayTextBox.Location = new System.Drawing.Point(330, 230);
            this.sortedArrayTextBox.Multiline = true;
            this.sortedArrayTextBox.Name = "sortedArrayTextBox";
            this.sortedArrayTextBox.ReadOnly = true;
            this.sortedArrayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sortedArrayTextBox.Size = new System.Drawing.Size(300, 100);
            this.sortedArrayTextBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(330, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Результаты сортировки:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(330, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(143, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Отсортированный массив:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Входные данные:";
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(330, 350);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(642, 290);
            this.chart1.TabIndex = 9;
            this.chart1.Text = "chart1";
            // 
            // startSortingButton
            // 
            this.startSortingButton.Location = new System.Drawing.Point(12, 550);
            this.startSortingButton.Name = "startSortingButton";
            this.startSortingButton.Size = new System.Drawing.Size(300, 40);
            this.startSortingButton.TabIndex = 10;
            this.startSortingButton.Text = "НАЧАТЬ СОРТИРОВКУ";
            this.startSortingButton.UseVisualStyleBackColor = true;
            this.startSortingButton.Click += new System.EventHandler(this.startSortingButton_Click);
            // 
            // generateDataButton
            // 
            this.generateDataButton.Location = new System.Drawing.Point(12, 200);
            this.generateDataButton.Name = "generateDataButton";
            this.generateDataButton.Size = new System.Drawing.Size(145, 30);
            this.generateDataButton.TabIndex = 11;
            this.generateDataButton.Text = "Сгенерировать";
            this.generateDataButton.UseVisualStyleBackColor = true;
            this.generateDataButton.Click += new System.EventHandler(this.generateDataButton_Click);
            // 
            // clearDataButton
            // 
            this.clearDataButton.Location = new System.Drawing.Point(167, 200);
            this.clearDataButton.Name = "clearDataButton";
            this.clearDataButton.Size = new System.Drawing.Size(145, 30);
            this.clearDataButton.TabIndex = 12;
            this.clearDataButton.Text = "Очистить";
            this.clearDataButton.UseVisualStyleBackColor = true;
            this.clearDataButton.Click += new System.EventHandler(this.clearDataButton_Click);
            // 
            // countNumericUpDown
            // 
            this.countNumericUpDown.Location = new System.Drawing.Point(12, 520);
            this.countNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countNumericUpDown.Name = "countNumericUpDown";
            this.countNumericUpDown.Size = new System.Drawing.Size(300, 20);
            this.countNumericUpDown.TabIndex = 13;
            this.countNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.countNumericUpDown.ValueChanged += new System.EventHandler(this.countNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 500);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Количество чисел (1-1000):";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(827, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 30);
            this.button1.TabIndex = 15;
            this.button1.Text = "Назад";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Sort
            // 
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.countNumericUpDown);
            this.Controls.Add(this.clearDataButton);
            this.Controls.Add(this.generateDataButton);
            this.Controls.Add(this.startSortingButton);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sortedArrayTextBox);
            this.Controls.Add(this.resultsListBox);
            this.Controls.Add(this.sortOrderGroupBox);
            this.Controls.Add(this.algorithmsGroupBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "Sort";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Визуализатор алгоритмов сортировки";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.algorithmsGroupBox.ResumeLayout(false);
            this.algorithmsGroupBox.PerformLayout();
            this.sortOrderGroupBox.ResumeLayout(false);
            this.sortOrderGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button button1;
    }
}