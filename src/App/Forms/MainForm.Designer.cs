namespace App.Forms;

partial class MainForm
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
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        mnuMain = new MenuStrip();
        tsmiFile = new ToolStripMenuItem();
        tsmiFileFeature1 = new ToolStripMenuItem();
        tsmiFileFeature2 = new ToolStripMenuItem();
        tssFile1 = new ToolStripSeparator();
        tsmiFileFeature3 = new ToolStripMenuItem();
        tsmiFileFeature4 = new ToolStripMenuItem();
        tssFile2 = new ToolStripSeparator();
        tsmiExit = new ToolStripMenuItem();
        tsmiTools = new ToolStripMenuItem();
        tsmiToolFeature1 = new ToolStripMenuItem();
        tsmiToolFeature2 = new ToolStripMenuItem();
        tssTools1 = new ToolStripSeparator();
        tsmiOptions = new ToolStripMenuItem();
        tsmiHelp = new ToolStripMenuItem();
        tsmiAbout = new ToolStripMenuItem();
        ntySysTray = new NotifyIcon(components);
        cmsSysTray = new ContextMenuStrip(components);
        tsmiSysTrayOptions = new ToolStripMenuItem();
        tsmiSysTrayFeature1 = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        tsmiSysTrayExit = new ToolStripMenuItem();
        mnuMain.SuspendLayout();
        cmsSysTray.SuspendLayout();
        SuspendLayout();
        // 
        // mnuMain
        // 
        mnuMain.ImageScalingSize = new Size(28, 28);
        mnuMain.Items.AddRange(new ToolStripItem[] { tsmiFile, tsmiTools, tsmiHelp });
        mnuMain.Location = new Point(0, 0);
        mnuMain.Name = "mnuMain";
        mnuMain.Padding = new Padding(4, 0, 0, 0);
        mnuMain.Size = new Size(784, 24);
        mnuMain.TabIndex = 0;
        mnuMain.Text = "Main Menu";
        // 
        // tsmiFile
        // 
        tsmiFile.DropDownItems.AddRange(new ToolStripItem[] { tsmiFileFeature1, tsmiFileFeature2, tssFile1, tsmiFileFeature3, tsmiFileFeature4, tssFile2, tsmiExit });
        tsmiFile.Name = "tsmiFile";
        tsmiFile.Size = new Size(37, 24);
        tsmiFile.Text = "&File";
        // 
        // tsmiFileFeature1
        // 
        tsmiFileFeature1.Name = "tsmiFileFeature1";
        tsmiFileFeature1.Size = new Size(143, 22);
        tsmiFileFeature1.Text = "&File Feature 1";
        // 
        // tsmiFileFeature2
        // 
        tsmiFileFeature2.Name = "tsmiFileFeature2";
        tsmiFileFeature2.Size = new Size(143, 22);
        tsmiFileFeature2.Text = "&File Feature 2";
        // 
        // tssFile1
        // 
        tssFile1.Name = "tssFile1";
        tssFile1.Size = new Size(140, 6);
        // 
        // tsmiFileFeature3
        // 
        tsmiFileFeature3.Name = "tsmiFileFeature3";
        tsmiFileFeature3.Size = new Size(143, 22);
        tsmiFileFeature3.Text = "&File Feature 3";
        // 
        // tsmiFileFeature4
        // 
        tsmiFileFeature4.Name = "tsmiFileFeature4";
        tsmiFileFeature4.Size = new Size(143, 22);
        tsmiFileFeature4.Text = "&File Feature 4";
        // 
        // tssFile2
        // 
        tssFile2.AccessibleDescription = "";
        tssFile2.Name = "tssFile2";
        tssFile2.Size = new Size(140, 6);
        // 
        // tsmiExit
        // 
        tsmiExit.Name = "tsmiExit";
        tsmiExit.Size = new Size(143, 22);
        tsmiExit.Text = "&Exit";
        tsmiExit.Click += TsmiExit_Click;
        // 
        // tsmiTools
        // 
        tsmiTools.DropDownItems.AddRange(new ToolStripItem[] { tsmiToolFeature1, tsmiToolFeature2, tssTools1, tsmiOptions });
        tsmiTools.Name = "tsmiTools";
        tsmiTools.Size = new Size(47, 24);
        tsmiTools.Text = "&Tools";
        // 
        // tsmiToolFeature1
        // 
        tsmiToolFeature1.Name = "tsmiToolFeature1";
        tsmiToolFeature1.Size = new Size(148, 22);
        tsmiToolFeature1.Text = "Tool Feature 1";
        // 
        // tsmiToolFeature2
        // 
        tsmiToolFeature2.Name = "tsmiToolFeature2";
        tsmiToolFeature2.Size = new Size(148, 22);
        tsmiToolFeature2.Text = "Tool Feature 2";
        // 
        // tssTools1
        // 
        tssTools1.Name = "tssTools1";
        tssTools1.Size = new Size(145, 6);
        // 
        // tsmiOptions
        // 
        tsmiOptions.Name = "tsmiOptions";
        tsmiOptions.Size = new Size(148, 22);
        tsmiOptions.Text = "&Options...";
        // 
        // tsmiHelp
        // 
        tsmiHelp.DropDownItems.AddRange(new ToolStripItem[] { tsmiAbout });
        tsmiHelp.Name = "tsmiHelp";
        tsmiHelp.Size = new Size(44, 24);
        tsmiHelp.Text = "&Help";
        // 
        // tsmiAbout
        // 
        tsmiAbout.Name = "tsmiAbout";
        tsmiAbout.Size = new Size(107, 22);
        tsmiAbout.Text = "&About";
        // 
        // ntySysTray
        // 
        ntySysTray.ContextMenuStrip = cmsSysTray;
        ntySysTray.Icon = (Icon)resources.GetObject("ntySysTray.Icon");
        ntySysTray.Text = "Application Name";
        ntySysTray.Visible = true;
        ntySysTray.DoubleClick += NtySysTray_DoubleClick;
        // 
        // cmsSysTray
        // 
        cmsSysTray.Items.AddRange(new ToolStripItem[] { tsmiSysTrayOptions, tsmiSysTrayFeature1, toolStripSeparator1, tsmiSysTrayExit });
        cmsSysTray.Name = "cmsSysTray";
        cmsSysTray.Size = new Size(189, 76);
        // 
        // tsmiSysTrayOptions
        // 
        tsmiSysTrayOptions.Name = "tsmiSysTrayOptions";
        tsmiSysTrayOptions.Size = new Size(188, 22);
        tsmiSysTrayOptions.Text = "Options...";
        // 
        // tsmiSysTrayFeature1
        // 
        tsmiSysTrayFeature1.Name = "tsmiSysTrayFeature1";
        tsmiSysTrayFeature1.Size = new Size(188, 22);
        tsmiSysTrayFeature1.Text = "System Tray Feature 1";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(185, 6);
        // 
        // tsmiSysTrayExit
        // 
        tsmiSysTrayExit.Name = "tsmiSysTrayExit";
        tsmiSysTrayExit.Size = new Size(188, 22);
        tsmiSysTrayExit.Text = "Exit";
        tsmiSysTrayExit.Click += TsmiSysTrayExit_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(784, 441);
        Controls.Add(mnuMain);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = mnuMain;
        Margin = new Padding(2);
        Name = "MainForm";
        Text = "MainForm";
        FormClosing += MainForm_FormClosing;
        Shown += MainForm_Shown;
        mnuMain.ResumeLayout(false);
        mnuMain.PerformLayout();
        cmsSysTray.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip mnuMain;
    private ToolStripMenuItem tsmiFile;
    private ToolStripMenuItem tsmiFileFeature1;
    private ToolStripMenuItem tsmiFileFeature2;
    private ToolStripSeparator tssFile1;
    private ToolStripMenuItem tsmiFileFeature3;
    private ToolStripMenuItem tsmiFileFeature4;
    private ToolStripSeparator tssFile2;
    private ToolStripMenuItem tsmiExit;
    private ToolStripMenuItem tsmiTools;
    private ToolStripMenuItem tsmiToolFeature1;
    private ToolStripMenuItem tsmiToolFeature2;
    private ToolStripSeparator tssTools1;
    private ToolStripMenuItem tsmiOptions;
    private ToolStripMenuItem tsmiHelp;
    private ToolStripMenuItem tsmiAbout;
    private NotifyIcon ntySysTray;
    private ContextMenuStrip cmsSysTray;
    private ToolStripMenuItem tsmiSysTrayOptions;
    private ToolStripMenuItem tsmiSysTrayFeature1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem tsmiSysTrayExit;
}