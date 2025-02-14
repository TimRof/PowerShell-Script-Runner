namespace PowerShellScriptRunner
{
    partial class MainForm
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
            scriptComboBox = new ComboBox();
            scriptComboBoxLabel = new Label();
            parametersPanel = new Panel();
            parametersPanelLabel = new Label();
            panelBorder = new Panel();
            runScriptButton = new Button();
            scriptListRefreshButton = new Button();
            SuspendLayout();
            // 
            // scriptComboBox
            // 
            scriptComboBox.FormattingEnabled = true;
            scriptComboBox.Location = new Point(14, 36);
            scriptComboBox.Margin = new Padding(3, 4, 3, 4);
            scriptComboBox.Name = "scriptComboBox";
            scriptComboBox.Size = new Size(277, 28);
            scriptComboBox.TabIndex = 0;
            scriptComboBox.SelectedIndexChanged += ScriptComboBox_SelectedIndexChanged;
            // 
            // scriptComboBoxLabel
            // 
            scriptComboBoxLabel.AutoSize = true;
            scriptComboBoxLabel.Location = new Point(14, 12);
            scriptComboBoxLabel.Name = "scriptComboBoxLabel";
            scriptComboBoxLabel.Size = new Size(94, 20);
            scriptComboBoxLabel.TabIndex = 1;
            scriptComboBoxLabel.Text = "Select Script:";
            // 
            // parametersPanel
            // 
            parametersPanel.AutoScroll = true;
            parametersPanel.Location = new Point(14, 95);
            parametersPanel.Margin = new Padding(3, 4, 3, 4);
            parametersPanel.Name = "parametersPanel";
            parametersPanel.Size = new Size(619, 265);
            parametersPanel.TabIndex = 6;
            // 
            // parametersPanelLabel
            // 
            parametersPanelLabel.AutoSize = true;
            parametersPanelLabel.Location = new Point(14, 71);
            parametersPanelLabel.Name = "parametersPanelLabel";
            parametersPanelLabel.Size = new Size(85, 20);
            parametersPanelLabel.TabIndex = 0;
            parametersPanelLabel.Text = "Parameters:";
            // 
            // panelBorder
            // 
            panelBorder.AutoScroll = true;
            panelBorder.BackColor = SystemColors.ControlDarkDark;
            panelBorder.Location = new Point(13, 93);
            panelBorder.Margin = new Padding(3, 4, 3, 4);
            panelBorder.Name = "panelBorder";
            panelBorder.Size = new Size(622, 268);
            panelBorder.TabIndex = 7;
            // 
            // runScriptButton
            // 
            runScriptButton.Location = new Point(12, 369);
            runScriptButton.Margin = new Padding(3, 4, 3, 4);
            runScriptButton.Name = "runScriptButton";
            runScriptButton.Size = new Size(106, 40);
            runScriptButton.TabIndex = 3;
            runScriptButton.Text = "Run Script";
            runScriptButton.UseVisualStyleBackColor = true;
            runScriptButton.Click += runScriptButton_Click;
            // 
            // scriptListRefreshButton
            // 
            scriptListRefreshButton.Font = new Font("Segoe UI Emoji", 8F);
            scriptListRefreshButton.Location = new Point(294, 36);
            scriptListRefreshButton.Margin = new Padding(0);
            scriptListRefreshButton.Name = "scriptListRefreshButton";
            scriptListRefreshButton.RightToLeft = RightToLeft.No;
            scriptListRefreshButton.Size = new Size(28, 28);
            scriptListRefreshButton.TabIndex = 8;
            scriptListRefreshButton.Text = "↻";
            scriptListRefreshButton.UseVisualStyleBackColor = true;
            scriptListRefreshButton.Click += ScriptListRefreshButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(647, 417);
            Controls.Add(scriptListRefreshButton);
            Controls.Add(parametersPanel);
            Controls.Add(parametersPanelLabel);
            Controls.Add(runScriptButton);
            Controls.Add(scriptComboBoxLabel);
            Controls.Add(scriptComboBox);
            Controls.Add(panelBorder);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            Text = "PowerShell Script Runner";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox scriptComboBox;
        private Label scriptComboBoxLabel;
        private Panel parametersPanel;
        private Label parametersPanelLabel;
        private Panel panelBorder;
        private Button runScriptButton;
        private Button scriptListRefreshButton;
    }
}
