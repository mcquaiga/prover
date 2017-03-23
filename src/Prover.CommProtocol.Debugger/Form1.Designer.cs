namespace Prover.CommProtocol.Debugger
{
    partial class Form1
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
            this.button_MiniMax = new System.Windows.Forms.Button();
            this.button_MiniAT = new System.Windows.Forms.Button();
            this.textBox_CommPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_BaudRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_MiniMax
            // 
            this.button_MiniMax.Location = new System.Drawing.Point(241, 36);
            this.button_MiniMax.Name = "button_MiniMax";
            this.button_MiniMax.Size = new System.Drawing.Size(198, 57);
            this.button_MiniMax.TabIndex = 0;
            this.button_MiniMax.Text = "Mini-Max";
            this.button_MiniMax.UseVisualStyleBackColor = true;
            this.button_MiniMax.Click += new System.EventHandler(this.button_MiniMax_Click);
            // 
            // button_MiniAT
            // 
            this.button_MiniAT.Location = new System.Drawing.Point(456, 36);
            this.button_MiniAT.Name = "button_MiniAT";
            this.button_MiniAT.Size = new System.Drawing.Size(194, 57);
            this.button_MiniAT.TabIndex = 1;
            this.button_MiniAT.Text = "Mini-AT";
            this.button_MiniAT.UseVisualStyleBackColor = true;
            // 
            // textBox_CommPort
            // 
            this.textBox_CommPort.Location = new System.Drawing.Point(89, 36);
            this.textBox_CommPort.Name = "textBox_CommPort";
            this.textBox_CommPort.Size = new System.Drawing.Size(97, 20);
            this.textBox_CommPort.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Comm. Port";
            // 
            // textBox_BaudRate
            // 
            this.textBox_BaudRate.Location = new System.Drawing.Point(89, 62);
            this.textBox_BaudRate.Name = "textBox_BaudRate";
            this.textBox_BaudRate.Size = new System.Drawing.Size(97, 20);
            this.textBox_BaudRate.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Baud Rate";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(344, 99);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(306, 343);
            this.dataGridView1.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 454);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_BaudRate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_CommPort);
            this.Controls.Add(this.button_MiniAT);
            this.Controls.Add(this.button_MiniMax);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_MiniMax;
        private System.Windows.Forms.Button button_MiniAT;
        private System.Windows.Forms.TextBox textBox_CommPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_BaudRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}

