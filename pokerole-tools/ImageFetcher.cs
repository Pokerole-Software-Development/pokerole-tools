using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace Pokerole.Tools
{
	internal class ImageFetcher
	{
		const string RootURL = "https://archives.bulbagarden.net/wiki/Category:HOME_artwork";
		private string destDirectory = "";

		public void FetchImages()
		{
			destDirectory = Path.GetFullPath(".");
			destDirectory = Path.Combine(destDirectory, "Images");
			Directory.CreateDirectory(destDirectory);

			Parallel.ForEach(IteratePages, ProcessImage);

		}
		private IEnumerable<String> IteratePages()
		{
			HtmlWeb pageFetcher = new HtmlWeb();
			HtmlDocument doc = pageFetcher.Load(RootURL);
			do
			{
				foreach (var item in IteratePage(doc))
				{
					yield return item;
				}


			} while (doc != null);
		}
		private IEnumerable<String> IteratePage(HtmlDocument page)
		{
			var items = page.DocumentNode.SelectNodes("//li");
			foreach(var item in items)
			{
				if (item.GetAttributeValue("class", "") != "gallerybox")
				{
					Debugger.Break();
				}
				var link = item.SelectSingleNode("//a");
				String href = link.GetAttributeValue("href", null);
				if (href == null)
				{
					Debugger.Break();
				}
				yield return "https://archives.bulbagarden.net" + href;
			}
		}
		private void ProcessImage(String url)
		{
			throw new NotImplementedException();
		}
	}
}