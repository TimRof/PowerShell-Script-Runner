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
            adminWarningLabel = new Label();
            SuspendLayout();
            // 
            // scriptComboBox
            // 
            scriptComboBox.FormattingEnabled = true;
            scriptComboBox.Location = new Point(12, 27);
            scriptComboBox.Name = "scriptComboBox";
            scriptComboBox.Size = new Size(243, 23);
            scriptComboBox.TabIndex = 0;
            scriptComboBox.SelectedIndexChanged += ScriptComboBox_SelectedIndexChanged;
            // 
            // scriptComboBoxLabel
            // 
            scriptComboBoxLabel.AutoSize = true;
            scriptComboBoxLabel.Location = new Point(12, 9);
            scriptComboBoxLabel.Name = "scriptComboBoxLabel";
            scriptComboBoxLabel.Size = new Size(74, 15);
            scriptComboBoxLabel.TabIndex = 1;
            scriptComboBoxLabel.Text = "Select Script:";
            // 
            // parametersPanel
            // 
            parametersPanel.AutoScroll = true;
            parametersPanel.Location = new Point(12, 71);
            parametersPanel.Name = "parametersPanel";
            parametersPanel.Size = new Size(542, 199);
            parametersPanel.TabIndex = 6;
            // 
            // parametersPanelLabel
            // 
            parametersPanelLabel.AutoSize = true;
            parametersPanelLabel.Location = new Point(12, 53);
            parametersPanelLabel.Name = "parametersPanelLabel";
            parametersPanelLabel.Size = new Size(69, 15);
            parametersPanelLabel.TabIndex = 0;
            parametersPanelLabel.Text = "Parameters:";
            // 
            // panelBorder
            // 
            panelBorder.AutoScroll = true;
            panelBorder.BackColor = SystemColors.ControlDarkDark;
            panelBorder.Location = new Point(11, 70);
            panelBorder.Name = "panelBorder";
            panelBorder.Size = new Size(544, 201);
            panelBorder.TabIndex = 7;
            // 
            // runScriptButton
            // 
            runScriptButton.Location = new Point(10, 277);
            runScriptButton.Name = "runScriptButton";
            runScriptButton.Size = new Size(93, 30);
            runScriptButton.TabIndex = 3;
            runScriptButton.Text = "Run Script";
            runScriptButton.UseVisualStyleBackColor = true;
            runScriptButton.Click += RunScriptButton_Click;
            // 
            // scriptListRefreshButton
            // 
            scriptListRefreshButton.Font = new Font("Segoe UI Emoji", 8F);
            scriptListRefreshButton.Location = new Point(257, 27);
            scriptListRefreshButton.Margin = new Padding(0);
            scriptListRefreshButton.Name = "scriptListRefreshButton";
            scriptListRefreshButton.RightToLeft = RightToLeft.No;
            scriptListRefreshButton.Size = new Size(24, 21);
            scriptListRefreshButton.TabIndex = 8;
            scriptListRefreshButton.Text = "↻";
            scriptListRefreshButton.UseVisualStyleBackColor = true;
            scriptListRefreshButton.Click += ScriptListRefreshButton_Click;
            // 
            // adminWarningLabel
            // 
            adminWarningLabel.AutoSize = true;
            adminWarningLabel.Location = new Point(109, 285);
            adminWarningLabel.Name = "adminWarningLabel";
            adminWarningLabel.Size = new Size(110, 15);
            adminWarningLabel.TabIndex = 9;
            adminWarningLabel.Text = "Check Admin Label";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(566, 313);
            Controls.Add(adminWarningLabel);
            Controls.Add(scriptListRefreshButton);
            Controls.Add(parametersPanel);
            Controls.Add(parametersPanelLabel);
            Controls.Add(runScriptButton);
            Controls.Add(scriptComboBoxLabel);
            Controls.Add(scriptComboBox);
            Controls.Add(panelBorder);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
        private Label adminWarningLabel;
    }
}
