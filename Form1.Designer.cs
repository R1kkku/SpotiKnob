namespace SpotiKnob
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private RoundedPanel mainCardPanel;
        private System.Windows.Forms.Label titleLabel;
        private RoundedPanel modePanel;
        private System.Windows.Forms.Panel modeDotPanel;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label toggleKeyCaptionLabel;
        private System.Windows.Forms.Label modeHintLabel;
        private MediaBindButton bindClockwiseButton;
        private MediaBindButton bindPressButton;
        private MediaBindButton bindCounterClockwiseButton;
        private System.Windows.Forms.Label creatorLabel;
        private System.Windows.Forms.Button settingsButton;
        private WindowControlButton minimizeButton;
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
            this.mainCardPanel = new SpotiKnob.RoundedPanel();
            this.settingsButton = new System.Windows.Forms.Button();
            this.minimizeButton = new SpotiKnob.WindowControlButton();
            this.closeButton = new SpotiKnob.WindowControlButton();
            this.creatorLabel = new System.Windows.Forms.Label();
            this.bindCounterClockwiseButton = new SpotiKnob.MediaBindButton();
            this.bindPressButton = new SpotiKnob.MediaBindButton();
            this.bindClockwiseButton = new SpotiKnob.MediaBindButton();
            this.modeHintLabel = new System.Windows.Forms.Label();
            this.toggleKeyCaptionLabel = new System.Windows.Forms.Label();
            this.modePanel = new SpotiKnob.RoundedPanel();
            this.modeLabel = new System.Windows.Forms.Label();
            this.modeDotPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.mainCardPanel.SuspendLayout();
            this.modePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainCardPanel
            // 
            this.mainCardPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.mainCardPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.mainCardPanel.BorderThickness = 1;
            this.mainCardPanel.Controls.Add(this.settingsButton);
            this.mainCardPanel.Controls.Add(this.minimizeButton);
            this.mainCardPanel.Controls.Add(this.closeButton);
            this.mainCardPanel.Controls.Add(this.creatorLabel);
            this.mainCardPanel.Controls.Add(this.bindCounterClockwiseButton);
            this.mainCardPanel.Controls.Add(this.bindPressButton);
            this.mainCardPanel.Controls.Add(this.bindClockwiseButton);
            this.mainCardPanel.Controls.Add(this.modeHintLabel);
            this.mainCardPanel.Controls.Add(this.toggleKeyCaptionLabel);
            this.mainCardPanel.Controls.Add(this.modePanel);
            this.mainCardPanel.Controls.Add(this.titleLabel);
            this.mainCardPanel.CornerRadius = 24;
            this.mainCardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainCardPanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.mainCardPanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.mainCardPanel.Location = new System.Drawing.Point(0, 0);
            this.mainCardPanel.Name = "mainCardPanel";
            this.mainCardPanel.Size = new System.Drawing.Size(792, 278);
            this.mainCardPanel.TabIndex = 0;
            this.mainCardPanel.UseGradient = false;
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.settingsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(75)))), ((int)(((byte)(97)))));
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Font = AppTheme.CreateUiFont(9.75F);
            this.settingsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.settingsButton.Location = new System.Drawing.Point(634, 224);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(106, 34);
            this.settingsButton.TabIndex = 4;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = false;
            // 
            // minimizeButton
            // 
            this.minimizeButton.BackColor = System.Drawing.Color.Transparent;
            this.minimizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.minimizeButton.FlatAppearance.BorderSize = 0;
            this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.minimizeButton.Glyph = SpotiKnob.WindowControlGlyph.Minimize;
            this.minimizeButton.Location = new System.Drawing.Point(704, 20);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(32, 32);
            this.minimizeButton.TabIndex = 9;
            this.minimizeButton.TabStop = false;
            this.minimizeButton.UseVisualStyleBackColor = false;
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.Transparent;
            this.closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.closeButton.Glyph = SpotiKnob.WindowControlGlyph.Close;
            this.closeButton.Location = new System.Drawing.Point(742, 20);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(32, 32);
            this.closeButton.TabIndex = 10;
            this.closeButton.TabStop = false;
            this.closeButton.UseVisualStyleBackColor = false;
            // 
            // creatorLabel
            // 
            this.creatorLabel.AutoSize = true;
            this.creatorLabel.Font = AppTheme.CreateMonoFont(9.25F);
            this.creatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.creatorLabel.Location = new System.Drawing.Point(42, 236);
            this.creatorLabel.Name = "creatorLabel";
            this.creatorLabel.Size = new System.Drawing.Size(105, 17);
            this.creatorLabel.TabIndex = 8;
            this.creatorLabel.Text = "Creator - R1kkku";
            // 
            // bindCounterClockwiseButton
            // 
            this.bindCounterClockwiseButton.BackColor = System.Drawing.Color.Transparent;
            this.bindCounterClockwiseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bindCounterClockwiseButton.FlatAppearance.BorderSize = 0;
            this.bindCounterClockwiseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bindCounterClockwiseButton.Glyph = SpotiKnob.MediaButtonGlyph.Previous;
            this.bindCounterClockwiseButton.IsListening = false;
            this.bindCounterClockwiseButton.Location = new System.Drawing.Point(548, 136);
            this.bindCounterClockwiseButton.Name = "bindCounterClockwiseButton";
            this.bindCounterClockwiseButton.Size = new System.Drawing.Size(44, 44);
            this.bindCounterClockwiseButton.TabIndex = 3;
            this.bindCounterClockwiseButton.TabStop = false;
            this.bindCounterClockwiseButton.UseVisualStyleBackColor = false;
            // 
            // bindPressButton
            // 
            this.bindPressButton.BackColor = System.Drawing.Color.Transparent;
            this.bindPressButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bindPressButton.FlatAppearance.BorderSize = 0;
            this.bindPressButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bindPressButton.Glyph = SpotiKnob.MediaButtonGlyph.Play;
            this.bindPressButton.IsListening = false;
            this.bindPressButton.Location = new System.Drawing.Point(604, 136);
            this.bindPressButton.Name = "bindPressButton";
            this.bindPressButton.Size = new System.Drawing.Size(44, 44);
            this.bindPressButton.TabIndex = 2;
            this.bindPressButton.TabStop = false;
            this.bindPressButton.UseVisualStyleBackColor = false;
            // 
            // bindClockwiseButton
            // 
            this.bindClockwiseButton.BackColor = System.Drawing.Color.Transparent;
            this.bindClockwiseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bindClockwiseButton.FlatAppearance.BorderSize = 0;
            this.bindClockwiseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bindClockwiseButton.Glyph = SpotiKnob.MediaButtonGlyph.Next;
            this.bindClockwiseButton.IsListening = false;
            this.bindClockwiseButton.Location = new System.Drawing.Point(660, 136);
            this.bindClockwiseButton.Name = "bindClockwiseButton";
            this.bindClockwiseButton.Size = new System.Drawing.Size(44, 44);
            this.bindClockwiseButton.TabIndex = 1;
            this.bindClockwiseButton.TabStop = false;
            this.bindClockwiseButton.UseVisualStyleBackColor = false;
            // 
            // modeHintLabel
            // 
            this.modeHintLabel.Font = AppTheme.CreateUiFont(11F);
            this.modeHintLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.modeHintLabel.Location = new System.Drawing.Point(42, 92);
            this.modeHintLabel.Name = "modeHintLabel";
            this.modeHintLabel.Size = new System.Drawing.Size(420, 62);
            this.modeHintLabel.TabIndex = 5;
            this.modeHintLabel.Text = "Spotify mode - Controls Next and Previous, can play/pause by pressing knob";
            // 
            // toggleKeyCaptionLabel
            // 
            this.toggleKeyCaptionLabel.AutoSize = true;
            this.toggleKeyCaptionLabel.Font = AppTheme.CreateMonoFont(9.25F);
            this.toggleKeyCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.toggleKeyCaptionLabel.Location = new System.Drawing.Point(486, 36);
            this.toggleKeyCaptionLabel.Name = "toggleKeyCaptionLabel";
            this.toggleKeyCaptionLabel.Size = new System.Drawing.Size(96, 19);
            this.toggleKeyCaptionLabel.TabIndex = 7;
            this.toggleKeyCaptionLabel.Text = "Toggle key  -  ";
            // 
            // modePanel
            // 
            this.modePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.modePanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.modePanel.BorderThickness = 1;
            this.modePanel.Controls.Add(this.modeLabel);
            this.modePanel.Controls.Add(this.modeDotPanel);
            this.modePanel.CornerRadius = 20;
            this.modePanel.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(8)))), ((int)(((byte)(14)))));
            this.modePanel.GradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(12)))), ((int)(((byte)(20)))));
            this.modePanel.Location = new System.Drawing.Point(230, 24);
            this.modePanel.Name = "modePanel";
            this.modePanel.Size = new System.Drawing.Size(236, 38);
            this.modePanel.TabIndex = 6;
            this.modePanel.UseGradient = false;
            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.Font = AppTheme.CreateMonoFont(8.75F);
            this.modeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(197)))), ((int)(((byte)(253)))));
            this.modeLabel.Location = new System.Drawing.Point(18, 11);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(91, 14);
            this.modeLabel.TabIndex = 1;
            this.modeLabel.Text = "Spotify Mode";
            // 
            // modeDotPanel
            // 
            this.modeDotPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(197)))), ((int)(((byte)(253)))));
            this.modeDotPanel.Location = new System.Drawing.Point(206, 14);
            this.modeDotPanel.Name = "modeDotPanel";
            this.modeDotPanel.Size = new System.Drawing.Size(10, 10);
            this.modeDotPanel.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = AppTheme.CreateMonoFont(24F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.titleLabel.Location = new System.Drawing.Point(38, 24);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(179, 37);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Spotiknob";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(792, 278);
            this.Controls.Add(this.mainCardPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SpotiKnob";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.mainCardPanel.ResumeLayout(false);
            this.mainCardPanel.PerformLayout();
            this.modePanel.ResumeLayout(false);
            this.modePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
