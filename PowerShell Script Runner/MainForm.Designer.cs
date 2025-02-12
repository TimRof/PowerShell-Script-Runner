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
            runScriptButton = new Button();
            outputRichTextBox = new RichTextBox();
            outputRichTextBoxLabel = new Label();
            parametersPanel = new Panel();
            parametersPanelLabel = new Label();
            panelBorder = new Panel();
            SuspendLayout();
            // 
            // scriptComboBox
            // 
            scriptComboBox.FormattingEnabled = true;
            scriptComboBox.Location = new Point(12, 27);
            scriptComboBox.Name = "scriptComboBox";
            scriptComboBox.Size = new Size(243, 23);
            scriptComboBox.TabIndex = 0;
            scriptComboBox.SelectedIndexChanged += scriptComboBox_SelectedIndexChanged;
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
            // runScriptButton
            // 
            runScriptButton.Location = new Point(12, 276);
            runScriptButton.Name = "runScriptButton";
            runScriptButton.Size = new Size(93, 30);
            runScriptButton.TabIndex = 3;
            runScriptButton.Text = "Run Script";
            runScriptButton.UseVisualStyleBackColor = true;
            runScriptButton.Click += runScriptButton_Click;
            // 
            // outputRichTextBox
            // 
            outputRichTextBox.Location = new Point(12, 327);
            outputRichTextBox.Name = "outputRichTextBox";
            outputRichTextBox.Size = new Size(542, 280);
            outputRichTextBox.TabIndex = 4;
            outputRichTextBox.Text = "";
            // 
            // outputRichTextBoxLabel
            // 
            outputRichTextBoxLabel.AutoSize = true;
            outputRichTextBoxLabel.Location = new Point(15, 309);
            outputRichTextBoxLabel.Name = "outputRichTextBoxLabel";
            outputRichTextBoxLabel.Size = new Size(71, 15);
            outputRichTextBoxLabel.TabIndex = 5;
            outputRichTextBoxLabel.Text = "Output Log:";
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(566, 619);
            Controls.Add(parametersPanel);
            Controls.Add(parametersPanelLabel);
            Controls.Add(outputRichTextBoxLabel);
            Controls.Add(outputRichTextBox);
            Controls.Add(runScriptButton);
            Controls.Add(scriptComboBoxLabel);
            Controls.Add(scriptComboBox);
            Controls.Add(panelBorder);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "MainForm";
            ShowIcon = false;
            Text = "PowerShell Script Runner";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox scriptComboBox;
        private Label scriptComboBoxLabel;
        private Button runScriptButton;
        private RichTextBox outputRichTextBox;
        private Label outputRichTextBoxLabel;
        private Panel parametersPanel;
        private Label parametersPanelLabel;
        private Panel panelBorder;
    }
}
