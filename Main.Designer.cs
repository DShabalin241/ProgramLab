namespace ProgramLab
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.DichotomyButton = new System.Windows.Forms.Button();
            this.Gold = new System.Windows.Forms.Button();
            this.SLAU = new System.Windows.Forms.Button();
            this.Intedral = new System.Windows.Forms.Button();
            this.MNK = new System.Windows.Forms.Button();
            this.Newton = new System.Windows.Forms.Button();
            this.Sort = new System.Windows.Forms.Button();
            this.buttonMPKS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DichotomyButton
            // 
            this.DichotomyButton.Location = new System.Drawing.Point(91, 26);
            this.DichotomyButton.Name = "DichotomyButton";
            this.DichotomyButton.Size = new System.Drawing.Size(117, 85);
            this.DichotomyButton.TabIndex = 1;
            this.DichotomyButton.Text = "Дихотомия";
            this.DichotomyButton.UseVisualStyleBackColor = true;
            this.DichotomyButton.Click += new System.EventHandler(this.DichotomyButton_Click);
            // 
            // Gold
            // 
            this.Gold.Location = new System.Drawing.Point(272, 26);
            this.Gold.Name = "Gold";
            this.Gold.Size = new System.Drawing.Size(117, 85);
            this.Gold.TabIndex = 2;
            this.Gold.Text = "Золотое сечение";
            this.Gold.UseVisualStyleBackColor = true;
            this.Gold.Click += new System.EventHandler(this.Gold_Click);
            // 
            // SLAU
            // 
            this.SLAU.Location = new System.Drawing.Point(457, 26);
            this.SLAU.Name = "SLAU";
            this.SLAU.Size = new System.Drawing.Size(117, 85);
            this.SLAU.TabIndex = 3;
            this.SLAU.Text = "СЛАУ";
            this.SLAU.UseVisualStyleBackColor = true;
            this.SLAU.Click += new System.EventHandler(this.SLAU_Click);
            // 
            // Intedral
            // 
            this.Intedral.Location = new System.Drawing.Point(457, 132);
            this.Intedral.Name = "Intedral";
            this.Intedral.Size = new System.Drawing.Size(117, 85);
            this.Intedral.TabIndex = 4;
            this.Intedral.Text = "Интеграл";
            this.Intedral.UseVisualStyleBackColor = true;
            this.Intedral.Click += new System.EventHandler(this.Intedral_Click);
            // 
            // MNK
            // 
            this.MNK.Location = new System.Drawing.Point(91, 132);
            this.MNK.Name = "MNK";
            this.MNK.Size = new System.Drawing.Size(117, 85);
            this.MNK.TabIndex = 5;
            this.MNK.Text = "МНК";
            this.MNK.UseVisualStyleBackColor = true;
            this.MNK.Click += new System.EventHandler(this.MNK_Click);
            // 
            // Newton
            // 
            this.Newton.Location = new System.Drawing.Point(272, 132);
            this.Newton.Name = "Newton";
            this.Newton.Size = new System.Drawing.Size(117, 85);
            this.Newton.TabIndex = 6;
            this.Newton.Text = "Метод Ньютона";
            this.Newton.UseVisualStyleBackColor = true;
            this.Newton.Click += new System.EventHandler(this.Newton_Click);
            // 
            // Sort
            // 
            this.Sort.Location = new System.Drawing.Point(637, 26);
            this.Sort.Name = "Sort";
            this.Sort.Size = new System.Drawing.Size(117, 85);
            this.Sort.TabIndex = 7;
            this.Sort.Text = "Сортировки";
            this.Sort.UseVisualStyleBackColor = true;
            this.Sort.Click += new System.EventHandler(this.Sort_Click);
            // 
            // buttonMPKS
            // 
            this.buttonMPKS.Location = new System.Drawing.Point(637, 132);
            this.buttonMPKS.Name = "buttonMPKS";
            this.buttonMPKS.Size = new System.Drawing.Size(117, 85);
            this.buttonMPKS.TabIndex = 8;
            this.buttonMPKS.Text = "Метод покоординатного спуска";
            this.buttonMPKS.UseVisualStyleBackColor = true;
            this.buttonMPKS.Click += new System.EventHandler(this.buttonMPKS_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonMPKS);
            this.Controls.Add(this.Sort);
            this.Controls.Add(this.Newton);
            this.Controls.Add(this.MNK);
            this.Controls.Add(this.Intedral);
            this.Controls.Add(this.SLAU);
            this.Controls.Add(this.Gold);
            this.Controls.Add(this.DichotomyButton);
            this.Name = "Main";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DichotomyButton;
        private System.Windows.Forms.Button Gold;
        private System.Windows.Forms.Button SLAU;
        private System.Windows.Forms.Button Intedral;
        private System.Windows.Forms.Button MNK;
        private System.Windows.Forms.Button Newton;
        private System.Windows.Forms.Button Sort;
        private System.Windows.Forms.Button buttonMPKS;
    }
}

