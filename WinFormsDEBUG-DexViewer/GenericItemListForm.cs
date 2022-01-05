﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Pokerole.Core;
using System.Linq;

namespace WinFormsDEBUG_DexViewer
{
	public partial class GenericItemListForm : Form
	{
		readonly List<IItemBuilder> unbuildable = new List<IItemBuilder>();
		public GenericItemListForm(Type type, IEnumerable<IItemBuilder> rawList)
		{
			InitializeComponent();
			ConfigureAndPopulateListForType(type, rawList);
			if (unbuildable.Count == 0)
			{
				lblError.Visible = false;
				btnShowBroken.Visible = false;
			}
			else
			{
				lblError.Visible = true;
				lblError.Text = $"{unbuildable.Count} instance(s) failed to build. Press \"Show Broken Entries\" to see them";
				btnShowBroken.Visible = true;
			}
		}
		public static bool TypeImplemented(Type type)
		{

			switch (type)
			{
				case Type _ when type == typeof(DexEntry.Builder):
					return true;
				case Type _ when type == typeof(Move.Builder):
				case Type _ when type == typeof(ImageRef.Builder):
				case Type _ when type == typeof(Item.Builder):
				case Type _ when type == typeof(Ability.Builder):
				case Type _ when type == typeof(EvolutionList.Builder):
				case Type _ when type == typeof(MonInstance.Builder):
					return false;
			}
			throw new InvalidOperationException($"Unknown type {type}");
		}

		private void ConfigureAndPopulateListForType(Type type, IEnumerable<IItemBuilder> rawList)
		{
			//configure
			Func<object, ListViewItem> constructListViewItem;
			switch (type)
			{
				case Type _ when type == typeof(DexEntry.Builder):
					lstItems.Columns.Clear();
					lstItems.Columns.AddRange(new ColumnHeader[]
					{
						new ColumnHeader()
						{
							Text = "Dex Num"
						},
						new ColumnHeader()
						{
							Text = "Sprite"
						},
						new ColumnHeader()
						{
							Text = "Entry Name"
						}
					});
					rawList = rawList.Cast<DexEntry.Builder>().OrderBy(item => item.DexNum).ThenBy(item => item.Name);
					constructListViewItem = raw =>
					{
						DexEntry entry = (DexEntry)raw;
						//lookup the sprite
						ListViewItem result = new ListViewItem(new String[] { entry.DexNum.ToString(), entry.Name });
						Image? sprite = Form1.LoadImage(entry.SpriteImage);
						if (sprite != null)
						{
							var imageList = lstItems.SmallImageList;
							int index = imageList.Images.Count;
							imageList.Images.Add(sprite);
							result.ImageIndex = index;
						}
						return result;
					};
					break;
				case Type _ when type == typeof(Move.Builder):
				case Type _ when type == typeof(ImageRef.Builder):
				case Type _ when type == typeof(Item.Builder):
				case Type _ when type == typeof(Ability.Builder):
				case Type _ when type == typeof(EvolutionList.Builder):
				case Type _ when type == typeof(MonInstance.Builder):
					throw new NotImplementedException();
				default: throw new InvalidOperationException($"Unknown type {type}");
			}
			foreach (var item in rawList)
			{
				if (!item.IsValid)
				{
					unbuildable.Add(item);
				}
				else
				{
					lstItems.Items.Add(constructListViewItem(item.Build()));
				}
			}
		}

		private void btnShowBroken_Click(object sender, EventArgs e)
		{

		}
	}
}