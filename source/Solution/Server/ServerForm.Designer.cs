namespace EnigmaMM
{
    partial class ServerForm
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
            this.uxButtonsFlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.uxStartButton = new System.Windows.Forms.Button();
            this.uxRestartButton = new System.Windows.Forms.Button();
            this.uxStopButton = new System.Windows.Forms.Button();
            this.uxGracefulCheck = new System.Windows.Forms.CheckBox();
            this.uxCommandInput = new System.Windows.Forms.TextBox();
            this.uxStatusStrip = new System.Windows.Forms.StatusStrip();
            this.uxStatusServerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.uxStatusUsersOnlineLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.uxLogListview = new System.Windows.Forms.ListView();
            this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uxButtonsFlowLayout.SuspendLayout();
            this.uxStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // uxButtonsFlowLayout
            // 
            this.uxButtonsFlowLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.uxButtonsFlowLayout.Controls.Add(this.uxStartButton);
            this.uxButtonsFlowLayout.Controls.Add(this.uxRestartButton);
            this.uxButtonsFlowLayout.Controls.Add(this.uxStopButton);
            this.uxButtonsFlowLayout.Controls.Add(this.uxGracefulCheck);
            this.uxButtonsFlowLayout.Location = new System.Drawing.Point(0, 0);
            this.uxButtonsFlowLayout.Name = "uxButtonsFlowLayout";
            this.uxButtonsFlowLayout.Size = new System.Drawing.Size(82, 217);
            this.uxButtonsFlowLayout.TabIndex = 1;
            // 
            // uxStartButton
            // 
            this.uxStartButton.Location = new System.Drawing.Point(3, 3);
            this.uxStartButton.Name = "uxStartButton";
            this.uxStartButton.Size = new System.Drawing.Size(75, 23);
            this.uxStartButton.TabIndex = 0;
            this.uxStartButton.Text = "Start";
            this.uxStartButton.UseVisualStyleBackColor = true;
            this.uxStartButton.Click += new System.EventHandler(this.uxStartButton_Click);
            // 
            // uxRestartButton
            // 
            this.uxRestartButton.Location = new System.Drawing.Point(3, 32);
            this.uxRestartButton.Name = "uxRestartButton";
            this.uxRestartButton.Size = new System.Drawing.Size(75, 23);
            this.uxRestartButton.TabIndex = 2;
            this.uxRestartButton.Text = "Restart";
            this.uxRestartButton.UseVisualStyleBackColor = true;
            this.uxRestartButton.Click += new System.EventHandler(this.uxRestartButton_Click);
            // 
            // uxStopButton
            // 
            this.uxStopButton.Location = new System.Drawing.Point(3, 61);
            this.uxStopButton.Name = "uxStopButton";
            this.uxStopButton.Size = new System.Drawing.Size(75, 23);
            this.uxStopButton.TabIndex = 1;
            this.uxStopButton.Text = "Stop";
            this.uxStopButton.UseVisualStyleBackColor = true;
            this.uxStopButton.Click += new System.EventHandler(this.uxStopButton_Click);
            // 
            // uxGracefulCheck
            // 
            this.uxGracefulCheck.AutoSize = true;
            this.uxGracefulCheck.Checked = true;
            this.uxGracefulCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uxGracefulCheck.Location = new System.Drawing.Point(3, 90);
            this.uxGracefulCheck.Name = "uxGracefulCheck";
            this.uxGracefulCheck.Size = new System.Drawing.Size(72, 17);
            this.uxGracefulCheck.TabIndex = 3;
            this.uxGracefulCheck.Text = "Graceful?";
            this.uxGracefulCheck.UseVisualStyleBackColor = true;
            // 
            // uxCommandInput
            // 
            this.uxCommandInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxCommandInput.Location = new System.Drawing.Point(88, 197);
            this.uxCommandInput.Name = "uxCommandInput";
            this.uxCommandInput.Size = new System.Drawing.Size(396, 20);
            this.uxCommandInput.TabIndex = 1;
            this.uxCommandInput.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.uxCommandInput_PreviewKeyDown);
            // 
            // uxStatusStrip
            // 
            this.uxStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uxStatusServerStatusLabel,
            this.uxStatusUsersOnlineLabel});
            this.uxStatusStrip.Location = new System.Drawing.Point(0, 220);
            this.uxStatusStrip.Name = "uxStatusStrip";
            this.uxStatusStrip.Size = new System.Drawing.Size(603, 22);
            this.uxStatusStrip.TabIndex = 1;
            this.uxStatusStrip.Text = "statusStrip1";
            // 
            // uxStatusServerStatusLabel
            // 
            this.uxStatusServerStatusLabel.Name = "uxStatusServerStatusLabel";
            this.uxStatusServerStatusLabel.Size = new System.Drawing.Size(541, 17);
            this.uxStatusServerStatusLabel.Spring = true;
            this.uxStatusServerStatusLabel.Text = "Status";
            // 
            // uxStatusUsersOnlineLabel
            // 
            this.uxStatusUsersOnlineLabel.Name = "uxStatusUsersOnlineLabel";
            this.uxStatusUsersOnlineLabel.Size = new System.Drawing.Size(47, 17);
            this.uxStatusUsersOnlineLabel.Text = "Users: 0";
            // 
            // uxLogListview
            // 
            this.uxLogListview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLogListview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Message});
            this.uxLogListview.Location = new System.Drawing.Point(88, 3);
            this.uxLogListview.Name = "uxLogListview";
            this.uxLogListview.Size = new System.Drawing.Size(396, 188);
            this.uxLogListview.TabIndex = 2;
            this.uxLogListview.UseCompatibleStateImageBehavior = false;
            this.uxLogListview.View = System.Windows.Forms.View.Details;
            // 
            // Message
            // 
            this.Message.Text = "Message";
            this.Message.Width = 323;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 242);
            this.Controls.Add(this.uxLogListview);
            this.Controls.Add(this.uxButtonsFlowLayout);
            this.Controls.Add(this.uxCommandInput);
            this.Controls.Add(this.uxStatusStrip);
            this.MinimumSize = new System.Drawing.Size(550, 280);
            this.Name = "ServerForm";
            this.Text = "EMM Server";
            this.uxButtonsFlowLayout.ResumeLayout(false);
            this.uxButtonsFlowLayout.PerformLayout();
            this.uxStatusStrip.ResumeLayout(false);
            this.uxStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel uxButtonsFlowLayout;
        private System.Windows.Forms.Button uxStartButton;
        private System.Windows.Forms.TextBox uxCommandInput;
        private System.Windows.Forms.StatusStrip uxStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel uxStatusServerStatusLabel;
        private System.Windows.Forms.Button uxStopButton;
        private System.Windows.Forms.ListView uxLogListview;
        private System.Windows.Forms.Button uxRestartButton;
        private System.Windows.Forms.CheckBox uxGracefulCheck;
        private System.Windows.Forms.ToolStripStatusLabel uxStatusUsersOnlineLabel;
        private System.Windows.Forms.ColumnHeader Message;
    }
}

