
namespace RE8FOV
{
    partial class MainUI
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
            this.normalFOVLabel = new System.Windows.Forms.Label();
            this.normalFOVTextBox = new System.Windows.Forms.TextBox();
            this.aimingFOVLabel = new System.Windows.Forms.Label();
            this.aimingFOVTextBox = new System.Windows.Forms.TextBox();
            this.applyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // normalFOVLabel
            // 
            this.normalFOVLabel.AutoSize = true;
            this.normalFOVLabel.Location = new System.Drawing.Point(6, 7);
            this.normalFOVLabel.Name = "normalFOVLabel";
            this.normalFOVLabel.Size = new System.Drawing.Size(72, 15);
            this.normalFOVLabel.TabIndex = 0;
            this.normalFOVLabel.Text = "Normal FOV";
            // 
            // normalFOVTextBox
            // 
            this.normalFOVTextBox.Location = new System.Drawing.Point(84, 4);
            this.normalFOVTextBox.Name = "normalFOVTextBox";
            this.normalFOVTextBox.Size = new System.Drawing.Size(59, 23);
            this.normalFOVTextBox.TabIndex = 1;
            this.normalFOVTextBox.Text = "81";
            this.normalFOVTextBox.TextChanged += new System.EventHandler(this.normalFOVTextBox_TextChanged);
            // 
            // aimingFOVLabel
            // 
            this.aimingFOVLabel.AutoSize = true;
            this.aimingFOVLabel.Location = new System.Drawing.Point(6, 36);
            this.aimingFOVLabel.Name = "aimingFOVLabel";
            this.aimingFOVLabel.Size = new System.Drawing.Size(71, 15);
            this.aimingFOVLabel.TabIndex = 2;
            this.aimingFOVLabel.Text = "Aiming FOV";
            // 
            // aimingFOVTextBox
            // 
            this.aimingFOVTextBox.Location = new System.Drawing.Point(84, 33);
            this.aimingFOVTextBox.Name = "aimingFOVTextBox";
            this.aimingFOVTextBox.Size = new System.Drawing.Size(59, 23);
            this.aimingFOVTextBox.TabIndex = 3;
            this.aimingFOVTextBox.Text = "70";
            this.aimingFOVTextBox.TextChanged += new System.EventHandler(this.aimingFOVTextBox_TextChanged);
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(84, 62);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(59, 23);
            this.applyButton.TabIndex = 4;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 92);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.aimingFOVTextBox);
            this.Controls.Add(this.aimingFOVLabel);
            this.Controls.Add(this.normalFOVTextBox);
            this.Controls.Add(this.normalFOVLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainUI";
            this.Text = "RE8FOV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainUI_FormClosing);
            this.Load += new System.EventHandler(this.MainUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label normalFOVLabel;
        private System.Windows.Forms.TextBox normalFOVTextBox;
        private System.Windows.Forms.Label aimingFOVLabel;
        private System.Windows.Forms.TextBox aimingFOVTextBox;
        private System.Windows.Forms.Button applyButton;
    }
}

