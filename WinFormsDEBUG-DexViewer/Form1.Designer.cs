
namespace WinFormsDEBUG_DexViewer
{
	partial class Form1
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
			this.pnlPrimary = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			// 
			// pnlPrimary
			// 
			this.pnlPrimary.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.pnlPrimary.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlPrimary.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.pnlPrimary.Location = new System.Drawing.Point(0, 0);
			this.pnlPrimary.Name = "pnlPrimary";
			this.pnlPrimary.Size = new System.Drawing.Size(390, 462);
			this.pnlPrimary.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(390, 462);
			this.Controls.Add(this.pnlPrimary);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel pnlPrimary;
	}
}

