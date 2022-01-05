using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Pokerole.Core;

namespace WinFormsDEBUG_DexViewer
{
	public partial class Form1 : Form
	{
		public static PokeroleXmlData Data { get; }
		private static ReadOnlyDictionary<ItemReference<ImageRef>, ImageRef> ImageIndex { get; }
		private static readonly ConcurrentDictionary<ItemReference<ImageRef>, Image?> images =
			new ConcurrentDictionary<ItemReference<ImageRef>, Image?>();

		static Form1()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(PokeroleXmlData));
			String content = File.ReadAllText("output.xml");
			Data = (PokeroleXmlData)serializer.Deserialize(new StringReader(content));
			ImageIndex = new ReadOnlyDictionary<ItemReference<ImageRef>, ImageRef>(Data.Images.Where(item =>
					item.IsValid).ToDictionary(item => item.ItemReference!.Value, item => item.Build()));
		}
		public static Image? LoadImage(ItemReference<ImageRef> reference)
		{
			if (!ImageIndex.TryGetValue(reference, out ImageRef imageRef))
			{
				return null;
			}
			return LoadImage(imageRef);
		}
		public static Image? LoadImage(ImageRef imageRef)
		{
			return images.GetOrAdd(imageRef.ItemReference, key =>
			{
				byte[]? data = imageRef.Data;
				if (data == null || data.Length == 0)
				{
					return null;
				}
				Image image = Image.FromStream(new MemoryStream(data));
				return image;
			});
		}
		public Form1()
		{
			InitializeComponent();

			AddButtons();
		}

		private void AddButtons()
		{
			//reflection for future proofing
			Type root = Data.GetType();
			foreach (var propItem in root.GetProperties())
			{
				//local reference for lambda to avoid foreach issues
				var prop = propItem;
				if (!prop.CanRead || !prop.CanWrite)
				{
					continue;
				}
				Type listType = prop.PropertyType;
				if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>))
				{
					continue;
				}
				Type type = listType.GetGenericArguments()[0];
				Button btnOpenType = new Button()
				{
					Name = "btnOpen" + prop.Name,
					Text = prop.Name,
					Width = 200
				};
				btnOpenType.Click += (sender, e) =>
				{
					ShowListFormForType(this, type, prop.GetValue(Data) ?? throw new InvalidOperationException());
				};
				pnlPrimary.Controls.Add(btnOpenType);
			}
		}

		internal static void ShowListFormForType(Form parent, Type type, object rawValue)
		{
			Form? next = CreateListFormForType(type, rawValue);
			if (next == null)
			{
				MessageBox.Show($"List window for {type} is not implemented");
			}
			else
			{
				next.ShowDialog(parent);
			}
		}
		private static Form? CreateListFormForType(Type type, object rawValue)
		{
			if (!GenericItemListForm.TypeImplemented(type))
			{
				return null;
			}
			return new GenericItemListForm(type, (IEnumerable<IItemBuilder>)rawValue);
		}
	}
}
