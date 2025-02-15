namespace App.Shared.Forms;

partial class BorderlessBaseForm
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
        // BorderlessBaseForm
        // 

        DoubleBuffered = true;
        ResizeRedraw = true;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.WindowsDefaultLocation;
        MinimizeBox = false;
        MdiChildrenMinimizedAnchorBottom = false;
        MaximizeBox = false;
        ShowIcon = false;
        ClientSize = new Size(1280, 720);
        Name = "BorderlessBaseForm";
        Text = "BorderlessBaseForm";
        ResumeLayout(false);
    }

    #endregion
}