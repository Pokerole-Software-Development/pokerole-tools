using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pokerole.Core;

namespace WinFormsDEBUG_DexViewer
{
	public partial class BrokenItemLister : Form
	{
		private readonly List<IItemBuilder> unbuildable;

		public BrokenItemLister(List<IItemBuilder> unbuildable)
		{
			if (unbuildable.Count < 0)
			{
				throw new ArgumentException("No items in list");
			}
			this.unbuildable = unbuildable;
			InitializeComponent();
			nudIndex.Maximum = unbuildable.Count - 1;
			nudIndex.Minimum = 0;
			SetSelected();
		}

		private void nudIndex_ValueChanged(object sender, EventArgs e)
		{
			SetSelected();
		}
		private void SetSelected()
		{
			var item = unbuildable[(int)nudIndex.Value];
			lblMissing.Text = $"Missing Values: {String.Join(", ", item.MissingValues)}";
			propGrid.SelectedObject = item;
		}
	}
}
