namespace WinformsBoilerplate.App.Shared.Forms;

partial class StandardFormBase
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
        SuspendLayout();
        //
        // StandardFormBase
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        ClientSize = new Size(1280, 720);
        DoubleBuffered = true;
        Name = "StandardFormBase";
        Text = "StandardFormBase";
        ResumeLayout(false);
    }

    #endregion
}
