namespace Mining_Hub_Launcher
{
    partial class Main_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.App_Selection_GroupBox = new System.Windows.Forms.GroupBox();
            this.Both_RadioButton = new System.Windows.Forms.RadioButton();
            this.Viewer_RadioButton = new System.Windows.Forms.RadioButton();
            this.Worker_RadioButton = new System.Windows.Forms.RadioButton();
            this.Auto_Start_CheckBox = new System.Windows.Forms.CheckBox();
            this.Updates_ComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Start_Button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.App_Selection_GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // App_Selection_GroupBox
            // 
            this.App_Selection_GroupBox.Controls.Add(this.Both_RadioButton);
            this.App_Selection_GroupBox.Controls.Add(this.Viewer_RadioButton);
            this.App_Selection_GroupBox.Controls.Add(this.Worker_RadioButton);
            this.App_Selection_GroupBox.Location = new System.Drawing.Point(58, 56);
            this.App_Selection_GroupBox.Name = "App_Selection_GroupBox";
            this.App_Selection_GroupBox.Size = new System.Drawing.Size(136, 119);
            this.App_Selection_GroupBox.TabIndex = 0;
            this.App_Selection_GroupBox.TabStop = false;
            this.App_Selection_GroupBox.Text = "What Are You Doing?";
            // 
            // Both_RadioButton
            // 
            this.Both_RadioButton.AutoSize = true;
            this.Both_RadioButton.Location = new System.Drawing.Point(25, 95);
            this.Both_RadioButton.Name = "Both_RadioButton";
            this.Both_RadioButton.Size = new System.Drawing.Size(47, 17);
            this.Both_RadioButton.TabIndex = 2;
            this.Both_RadioButton.TabStop = true;
            this.Both_RadioButton.Text = "Both";
            this.Both_RadioButton.UseVisualStyleBackColor = true;
            // 
            // Viewer_RadioButton
            // 
            this.Viewer_RadioButton.AutoSize = true;
            this.Viewer_RadioButton.Location = new System.Drawing.Point(25, 61);
            this.Viewer_RadioButton.Name = "Viewer_RadioButton";
            this.Viewer_RadioButton.Size = new System.Drawing.Size(62, 17);
            this.Viewer_RadioButton.TabIndex = 1;
            this.Viewer_RadioButton.TabStop = true;
            this.Viewer_RadioButton.Text = "Viewing";
            this.Viewer_RadioButton.UseVisualStyleBackColor = true;
            // 
            // Worker_RadioButton
            // 
            this.Worker_RadioButton.AutoSize = true;
            this.Worker_RadioButton.Location = new System.Drawing.Point(25, 29);
            this.Worker_RadioButton.Name = "Worker_RadioButton";
            this.Worker_RadioButton.Size = new System.Drawing.Size(65, 17);
            this.Worker_RadioButton.TabIndex = 0;
            this.Worker_RadioButton.TabStop = true;
            this.Worker_RadioButton.Text = "Working";
            this.Worker_RadioButton.UseVisualStyleBackColor = true;
            // 
            // Auto_Start_CheckBox
            // 
            this.Auto_Start_CheckBox.AutoSize = true;
            this.Auto_Start_CheckBox.Checked = true;
            this.Auto_Start_CheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Auto_Start_CheckBox.Location = new System.Drawing.Point(83, 196);
            this.Auto_Start_CheckBox.Name = "Auto_Start_CheckBox";
            this.Auto_Start_CheckBox.Size = new System.Drawing.Size(79, 17);
            this.Auto_Start_CheckBox.TabIndex = 1;
            this.Auto_Start_CheckBox.Text = "Auto Start?";
            this.Auto_Start_CheckBox.UseVisualStyleBackColor = true;
            // 
            // Updates_ComboBox
            // 
            this.Updates_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Updates_ComboBox.FormattingEnabled = true;
            this.Updates_ComboBox.Items.AddRange(new object[] {
            "On App Start",
            "Once A Day",
            "Once A Week",
            "Once A Month"});
            this.Updates_ComboBox.Location = new System.Drawing.Point(64, 261);
            this.Updates_ComboBox.Name = "Updates_ComboBox";
            this.Updates_ComboBox.Size = new System.Drawing.Size(121, 21);
            this.Updates_ComboBox.TabIndex = 2;
            this.Updates_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Updates_ComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Check For Updates";
            // 
            // Start_Button
            // 
            this.Start_Button.Location = new System.Drawing.Point(83, 309);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(75, 23);
            this.Start_Button.TabIndex = 4;
            this.Start_Button.Text = "Start";
            this.Start_Button.UseVisualStyleBackColor = true;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(236, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Mining Hub Launcher";
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 346);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Start_Button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Updates_ComboBox);
            this.Controls.Add(this.Auto_Start_CheckBox);
            this.Controls.Add(this.App_Selection_GroupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main_Form";
            this.Text = "Mining Hub Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_Form_FormClosing);
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.App_Selection_GroupBox.ResumeLayout(false);
            this.App_Selection_GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox App_Selection_GroupBox;
        private System.Windows.Forms.RadioButton Both_RadioButton;
        private System.Windows.Forms.RadioButton Viewer_RadioButton;
        private System.Windows.Forms.RadioButton Worker_RadioButton;
        private System.Windows.Forms.CheckBox Auto_Start_CheckBox;
        private System.Windows.Forms.ComboBox Updates_ComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Label label2;
    }
}

