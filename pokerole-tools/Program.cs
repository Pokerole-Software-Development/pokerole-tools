using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Pokerole.Core;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using PdfHandler;

namespace Pokerole.Tools
{
	class Program
	{
		static void Main(string[] args)
		{

			//ImageFetcher fetcher = new ImageFetcher();
			//fetcher.FetchImages();
			//String filename = "";
			//var values = PdfParser.ParsePdf(filename);

			InitialDataImporter importer = new InitialDataImporter();
			importer.DoImport();
		}

	}
}
