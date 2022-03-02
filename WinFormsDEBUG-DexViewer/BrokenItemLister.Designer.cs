
namespace WinFormsDEBUG_DexViewer
{
	partial class BrokenItemLister
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
			this.nudIndex = new System.Windows.Forms.NumericUpDown();
			this.propGrid = new System.Windows.Forms.PropertyGrid();
			this.lblMissing = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.nudIndex)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// nudIndex
			// 
			this.nudIndex.Location = new System.Drawing.Point(3, 3);
			this.nudIndex.Name = "nudIndex";
			this.nudIndex.Size = new System.Drawing.Size(120, 23);
			this.nudIndex.TabIndex = 0;
			this.nudIndex.ValueChanged += new System.EventHandler(this.nudIndex_ValueChanged);
			// 
			// propGrid
			// 
			this.propGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propGrid.Location = new System.Drawing.Point(3, 47);
			this.propGrid.Name = "propGrid";
			this.propGrid.Size = new System.Drawing.Size(794, 400);
			this.propGrid.TabIndex = 1;
			// 
			// lblMissing
			// 
			this.lblMissing.AutoSize = true;
			this.lblMissing.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblMissing.Location = new System.Drawing.Point(3, 29);
			this.lblMissing.Name = "lblMissing";
			this.lblMissing.Size = new System.Drawing.Size(794, 15);
			this.lblMissing.TabIndex = 2;
			this.lblMissing.Text = "label1";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.nudIndex, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.propGrid, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblMissing, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// BrokenItemLister
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BrokenItemLister";
			this.Text = "Unbuildable Items";
			((System.ComponentModel.ISupportInitialize)(this.nudIndex)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown nudIndex;
		private System.Windows.Forms.PropertyGrid propGrid;
		private System.Windows.Forms.Label lblMissing;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}