namespace App.Shared.Forms;

partial class StandardBaseForm
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
        // StandardBaseForm
        // 
        ClientSize = new Size(1280, 720);
        FormBorderStyle = FormBorderStyle.Sizable;
        StartPosition = FormStartPosition.WindowsDefaultLocation;
        MinimizeBox = true;
        MdiChildrenMinimizedAnchorBottom = true;
        MaximizeBox = true;
        ShowIcon = true;
        Name = "StandardBaseForm";
        Text = "StandardBaseForm";
        ResumeLayout(false);
    }

    #endregion
}