using System.Drawing;
using System.Windows.Forms;

namespace SpotiKnob
{
    public partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private RoundedPanel cardPanel;
        private Label titleLabel;
        private SettingsPillToggle adminToggle;
        private SettingsPillToggle startMinimizedToggle;
        private SettingsPillButton bindModeCycleButton;
        private PictureBox adminUacIcon;
        private ImageActionButton githubIconButton;
        private ImageActionButton websiteIconButton;
        private Label githubLabel;
        private Label websiteLabel;
        private ImageActionButton coffeeButton;
        private LinkLabel issueLinkLabel;
        private WindowControlButton closeButton;

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
            this.cardPanel = new SpotiKnob.RoundedPanel();
            this.closeButton = new SpotiKnob.WindowControlButton();
            this.issueLinkLabel = new System.Windows.Forms.LinkLabel();
            this.coffeeButton = new SpotiKnob.ImageActionButton();
            this.websiteLabel = new System.Windows.Forms.Label();
            this.githubLabel = new System.Windows.Forms.Label();
            this.websiteIconButton = new SpotiKnob.ImageActionButton();
            this.githubIconButton = new SpotiKnob.ImageActionButton();
            this.adminUacIcon = new System.Windows.Forms.PictureBox();
            this.bindModeCycleButton = new SpotiKnob.SettingsPillButton();
            this.startMinimizedToggle = new SpotiKnob.SettingsPillToggle();
            this.adminToggle = new SpotiKnob.SettingsPillToggle();
            this.titleLabel = new System.Windows.Forms.Label();
            this.cardPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminUacIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // cardPanel
            // 
            this.cardPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(20)))), ((int)(((byte)(34)))));
            this.cardPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(60)))), ((int)(((byte)(110)))));
            this.cardPanel.BorderThickness = 1;
            this.cardPanel.Controls.Add(this.closeButton);
            this.cardPanel.Controls.Add(this.issueLinkLabel);
            this.cardPanel.Controls.Add(this.coffeeButton);
            this.cardPanel.Controls.Add(this.websiteLabel);
            this.cardPanel.Controls.Add(this.githubLabel);
            this.cardPanel.Controls.Add(this.websiteIconButton);
            this.cardPanel.Controls.Add(this.githubIconButton);
            this.cardPanel.Controls.Add(this.adminUacIcon);
            this.cardPanel.Controls.Add(this.bindModeCycleButton);
            this.cardPanel.Controls.Add(this.startMinimizedToggle);
            this.cardPanel.Controls.Add(this.adminToggle);
            this.cardPanel.Controls.Add(this.titleLabel);
            this.cardPanel.CornerRadius = 36;
            this.cardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.cardPanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.cardPanel.Location = new System.Drawing.Point(0, 0);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Size = new System.Drawing.Size(804, 300);
            this.cardPanel.TabIndex = 1;
            this.cardPanel.UseGradient = true;
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.closeButton.Glyph = SpotiKnob.WindowControlGlyph.Close;
            this.closeButton.Location = new System.Drawing.Point(740, 18);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(32, 32);
            this.closeButton.TabIndex = 11;
            this.closeButton.TabStop = false;
            this.closeButton.UseVisualStyleBackColor = false;
            // 
            // issueLinkLabel
            // 
            this.issueLinkLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.issueLinkLabel.AutoSize = true;
            this.issueLinkLabel.Font = new System.Drawing.Font("Consolas", 11F);
            this.issueLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 22);
            this.issueLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.issueLinkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.issueLinkLabel.Location = new System.Drawing.Point(584, 242);
            this.issueLinkLabel.Name = "issueLinkLabel";
            this.issueLinkLabel.Size = new System.Drawing.Size(188, 23);
            this.issueLinkLabel.TabIndex = 10;
            this.issueLinkLabel.TabStop = true;
            this.issueLinkLabel.Text = "Report Issue on Github";
            this.issueLinkLabel.UseCompatibleTextRendering = true;
            this.issueLinkLabel.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            // 
            // coffeeButton
            // 
            this.coffeeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.coffeeButton.DisplayImage = null;
            this.coffeeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coffeeButton.Location = new System.Drawing.Point(450, 166);
            this.coffeeButton.Name = "coffeeButton";
            this.coffeeButton.Size = new System.Drawing.Size(176, 49);
            this.coffeeButton.TabIndex = 9;
            this.coffeeButton.UseVisualStyleBackColor = true;
            // 
            // websiteLabel
            // 
            this.websiteLabel.AutoSize = true;
            this.websiteLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.websiteLabel.Font = new System.Drawing.Font("Consolas", 14F);
            this.websiteLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.websiteLabel.Location = new System.Drawing.Point(520, 107);
            this.websiteLabel.Name = "websiteLabel";
            this.websiteLabel.Size = new System.Drawing.Size(160, 22);
            this.websiteLabel.TabIndex = 8;
            this.websiteLabel.Text = "Coderikku.cloud";
            // 
            // githubLabel
            // 
            this.githubLabel.AutoSize = true;
            this.githubLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubLabel.Font = new System.Drawing.Font("Consolas", 14F);
            this.githubLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.githubLabel.Location = new System.Drawing.Point(520, 43);
            this.githubLabel.Name = "githubLabel";
            this.githubLabel.Size = new System.Drawing.Size(70, 22);
            this.githubLabel.TabIndex = 7;
            this.githubLabel.Text = "R1kkku";
            // 
            // websiteIconButton
            // 
            this.websiteIconButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.websiteIconButton.DisplayImage = null;
            this.websiteIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.websiteIconButton.Location = new System.Drawing.Point(446, 100);
            this.websiteIconButton.Name = "websiteIconButton";
            this.websiteIconButton.Size = new System.Drawing.Size(62, 44);
            this.websiteIconButton.TabIndex = 6;
            this.websiteIconButton.UseVisualStyleBackColor = true;
            // 
            // githubIconButton
            // 
            this.githubIconButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.githubIconButton.DisplayImage = null;
            this.githubIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.githubIconButton.Location = new System.Drawing.Point(446, 36);
            this.githubIconButton.Name = "githubIconButton";
            this.githubIconButton.Size = new System.Drawing.Size(62, 44);
            this.githubIconButton.TabIndex = 5;
            this.githubIconButton.UseVisualStyleBackColor = true;
            // 
            // adminUacIcon
            // 
            this.adminUacIcon.BackColor = System.Drawing.Color.Transparent;
            this.adminUacIcon.Location = new System.Drawing.Point(216, 43);
            this.adminUacIcon.Name = "adminUacIcon";
            this.adminUacIcon.Size = new System.Drawing.Size(31, 33);
            this.adminUacIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.adminUacIcon.TabIndex = 3;
            this.adminUacIcon.TabStop = false;
            // 
            // bindModeCycleButton
            // 
            this.bindModeCycleButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bindModeCycleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bindModeCycleButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.bindModeCycleButton.IsActive = false;
            this.bindModeCycleButton.Location = new System.Drawing.Point(54, 166);
            this.bindModeCycleButton.Name = "bindModeCycleButton";
            this.bindModeCycleButton.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.bindModeCycleButton.Size = new System.Drawing.Size(212, 44);
            this.bindModeCycleButton.TabIndex = 2;
            this.bindModeCycleButton.Text = "Change Toggle key";
            this.bindModeCycleButton.UseVisualStyleBackColor = true;
            // 
            // startMinimizedToggle
            // 
            this.startMinimizedToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.startMinimizedToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startMinimizedToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startMinimizedToggle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.startMinimizedToggle.IsActive = false;
            this.startMinimizedToggle.Location = new System.Drawing.Point(54, 102);
            this.startMinimizedToggle.Name = "startMinimizedToggle";
            this.startMinimizedToggle.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.startMinimizedToggle.Size = new System.Drawing.Size(212, 44);
            this.startMinimizedToggle.TabIndex = 1;
            this.startMinimizedToggle.Text = "Start Minimized";
            this.startMinimizedToggle.UseVisualStyleBackColor = true;
            // 
            // adminToggle
            // 
            this.adminToggle.Appearance = System.Windows.Forms.Appearance.Button;
            this.adminToggle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.adminToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.adminToggle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.adminToggle.IsActive = false;
            this.adminToggle.Location = new System.Drawing.Point(54, 38);
            this.adminToggle.Name = "adminToggle";
            this.adminToggle.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.adminToggle.Size = new System.Drawing.Size(212, 44);
            this.adminToggle.TabIndex = 0;
            this.adminToggle.Text = "Run as Admin Always";
            this.adminToggle.UseVisualStyleBackColor = true;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(144)))), ((int)(((byte)(144)))));
            this.titleLabel.Location = new System.Drawing.Point(20, 12);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(58, 19);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Settings";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(21)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(804, 300);
            this.Controls.Add(this.cardPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SpotiKnob Settings";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsForm_KeyDown);
            this.cardPanel.ResumeLayout(false);
            this.cardPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adminUacIcon)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
