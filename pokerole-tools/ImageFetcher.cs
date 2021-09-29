/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.XPath;
using HtmlAgilityPack;
using System.Linq;

namespace Pokerole.Tools
{
	internal class ImageFetcher
	{
		//const string RootURL = "https://archives.bulbagarden.net/wiki/Category:HOME_artwork";
		private readonly string categoryUrl;
		private readonly string destDir;

		public ImageFetcher(string categoryUrl, string destDir)
		{
			this.categoryUrl = categoryUrl;
			this.destDir = Path.GetFullPath(destDir);
		}


		public void FetchImages()
		{
			//destDirectory = Path.GetFullPath(".");
			//destDirectory = Path.Combine(destDirectory, "Images");
			Directory.CreateDirectory(destDir);
			//ParallelOptions opts = new ParallelOptions
			//{
			//	MaxDegreeOfParallelism = 5
			//};
			var toDownload = IteratePages().AsParallel().Select(SelectImage).ToList();///*.Distinct()*/.ForAll(ProcessImage);
			toDownload.Sort();
			//now we has list...
			toDownload.AsParallel().ForAll(ProcessImage);
			//Parallel.ForEach(IteratePages().Select(), ProcessImage);

		}
		private IEnumerable<String> IteratePages()
		{
			HtmlWeb pageFetcher = new HtmlWeb();
			HtmlDocument doc = pageFetcher.Load(categoryUrl);
			do
			{
				foreach (var item in IteratePage(doc))
				{
					yield return item;
				}
				String? next = null;
				foreach (var node in doc.DocumentNode.SelectNodes("//a"))
				{
					if (node.InnerText == "next page")
					{
						next = node.GetAttributeValue("href", null);
						if (next == null)
						{
							Debugger.Break();
						}
						next = "https://archives.bulbagarden.net" + next;
						break;
					}
				}
				if (next == null)
				{
					break;
				}
				next = WebUtility.HtmlDecode(next);
				doc = pageFetcher.Load(next);
			} while (true);
		}
		private IEnumerable<String> IteratePage(HtmlDocument page)
		{
			var items = page.DocumentNode.SelectNodes("//li[@class=\"gallerybox\"]");
			foreach(var item in items)
			{
				if (item.GetAttributeValue("class", "") != "gallerybox")
				{
					Debugger.Break();
				}
				var link = item.SelectSingleNode(".//a");
				String href = link.GetAttributeValue("href", null);
				if (href == null)
				{
					Debugger.Break();
				}
				yield return "https://archives.bulbagarden.net" + href;
			}
		}
		private String SelectImage(String url)
		{
			HtmlWeb web = new HtmlWeb();
			HtmlDocument doc = web.Load(url);
			HtmlNode fullImage = doc.DocumentNode.SelectSingleNode("//div[@id=\"file\"]");
			HtmlNode link = fullImage.FirstChild;
			//luckily for us, this is a full url!
			String imageUrl = link.GetAttributeValue("href", null);
			if (imageUrl == null)
			{
				Debugger.Break();
				throw new InvalidOperationException();
			}
			return imageUrl;
		}
		private void ProcessImage(String url) { 
			//Download!
			using (WebClient client = new WebClient())
			{
				client.DownloadFile(url, Path.Combine(destDir, Path.GetFileName(url).Replace("File:", "")));
			}
		}
		public static void DownloadWikiCategoryImages(String categoryUrl, String destDir)
		{
			ImageFetcher fetcher = new ImageFetcher(categoryUrl, destDir);

			fetcher.FetchImages();
		}
	}
}