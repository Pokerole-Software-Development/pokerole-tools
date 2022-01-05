
namespace WinFormsDEBUG_DexViewer
{
	partial class GenericItemListForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.btnShowBroken = new System.Windows.Forms.Button();
			this.lblError = new System.Windows.Forms.Label();
			this.lstItems = new System.Windows.Forms.ListView();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.btnShowBroken, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.lblError, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.lstItems, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// btnShowBroken
			// 
			this.btnShowBroken.Location = new System.Drawing.Point(657, 424);
			this.btnShowBroken.Name = "btnShowBroken";
			this.btnShowBroken.Size = new System.Drawing.Size(140, 23);
			this.btnShowBroken.TabIndex = 0;
			this.btnShowBroken.Text = "Show Broken Entries";
			this.btnShowBroken.UseVisualStyleBackColor = true;
			this.btnShowBroken.Click += new System.EventHandler(this.btnShowBroken_Click);
			// 
			// lblError
			// 
			this.lblError.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblError.AutoSize = true;
			this.lblError.ForeColor = System.Drawing.Color.Red;
			this.lblError.Location = new System.Drawing.Point(3, 428);
			this.lblError.Name = "lblError";
			this.lblError.Size = new System.Drawing.Size(38, 15);
			this.lblError.TabIndex = 1;
			this.lblError.Text = "label1";
			// 
			// lstItems
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.lstItems, 2);
			this.lstItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstItems.HideSelection = false;
			this.lstItems.Location = new System.Drawing.Point(3, 3);
			this.lstItems.Name = "lstItems";
			this.lstItems.Size = new System.Drawing.Size(794, 415);
			this.lstItems.TabIndex = 3;
			this.lstItems.UseCompatibleStateImageBehavior = false;
			// 
			// GenericItemListForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "GenericItemListForm";
			this.Text = "GenericItemListForm";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnShowBroken;
		private System.Windows.Forms.Label lblError;
		private System.Windows.Forms.ListView lstItems;
	}
}