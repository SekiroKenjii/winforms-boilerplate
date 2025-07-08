namespace WinformsBoilerplate.App.Components.Forms;

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
        tsmiExit = new ToolStripMenuItem();
        tsmiTools = new ToolStripMenuItem();
        tsmiOptions = new ToolStripMenuItem();
        tsmiHelp = new ToolStripMenuItem();
        tsmiAbout = new ToolStripMenuItem();
        ntySysTray = new NotifyIcon(components);
        cmsSysTray = new ContextMenuStrip(components);
        tsmiSysTrayOptions = new ToolStripMenuItem();
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
        tsmiFile.DropDownItems.AddRange(new ToolStripItem[] { tsmiExit });
        tsmiFile.Name = "tsmiFile";
        tsmiFile.Size = new Size(37, 24);
        tsmiFile.Text = "&File";
        // 
        // tsmiExit
        // 
        tsmiExit.ImageScaling = ToolStripItemImageScaling.None;
        tsmiExit.Name = "tsmiExit";
        tsmiExit.Size = new Size(92, 22);
        tsmiExit.Text = "&Exit";
        // 
        // tsmiTools
        // 
        tsmiTools.DropDownItems.AddRange(new ToolStripItem[] { tsmiOptions });
        tsmiTools.Name = "tsmiTools";
        tsmiTools.Size = new Size(47, 24);
        tsmiTools.Text = "&Tools";
        // 
        // tsmiOptions
        // 
        tsmiOptions.Image = Properties.ImageRes16x.Settings;
        tsmiOptions.ImageScaling = ToolStripItemImageScaling.None;
        tsmiOptions.Name = "tsmiOptions";
        tsmiOptions.Size = new Size(125, 22);
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
        tsmiAbout.Image = Properties.ImageRes16x.AboutBox;
        tsmiAbout.ImageScaling = ToolStripItemImageScaling.None;
        tsmiAbout.Name = "tsmiAbout";
        tsmiAbout.Size = new Size(107, 22);
        tsmiAbout.Text = "&About";
        // 
        // ntySysTray
        // 
        ntySysTray.ContextMenuStrip = cmsSysTray;
        ntySysTray.Icon = (Icon)resources.GetObject("ntySysTray.Icon");
        ntySysTray.Text = "Application Name";
        ntySysTray.DoubleClick += NtySysTray_DoubleClick;
        // 
        // cmsSysTray
        // 
        cmsSysTray.Items.AddRange(new ToolStripItem[] { tsmiSysTrayOptions, toolStripSeparator1, tsmiSysTrayExit });
        cmsSysTray.Name = "cmsSysTray";
        cmsSysTray.Size = new Size(126, 54);
        // 
        // tsmiSysTrayOptions
        // 
        tsmiSysTrayOptions.Image = Properties.ImageRes16x.Settings;
        tsmiSysTrayOptions.ImageScaling = ToolStripItemImageScaling.None;
        tsmiSysTrayOptions.Name = "tsmiSysTrayOptions";
        tsmiSysTrayOptions.Size = new Size(125, 22);
        tsmiSysTrayOptions.Text = "Options...";
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(122, 6);
        // 
        // tsmiSysTrayExit
        // 
        tsmiSysTrayExit.Name = "tsmiSysTrayExit";
        tsmiSysTrayExit.Size = new Size(125, 22);
        tsmiSysTrayExit.Text = "Exit";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        ClientSize = new Size(784, 561);
        Controls.Add(mnuMain);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = mnuMain;
        Name = "MainForm";
        Text = "Application Name";
        mnuMain.ResumeLayout(false);
        mnuMain.PerformLayout();
        cmsSysTray.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip mnuMain;
    private ToolStripMenuItem tsmiFile;
    private ToolStripMenuItem tsmiExit;
    private ToolStripMenuItem tsmiTools;
    private ToolStripMenuItem tsmiOptions;
    private ToolStripMenuItem tsmiHelp;
    private ToolStripMenuItem tsmiAbout;
    private NotifyIcon ntySysTray;
    private ContextMenuStrip cmsSysTray;
    private ToolStripMenuItem tsmiSysTrayOptions;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem tsmiSysTrayExit;
}
