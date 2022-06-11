/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
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
using Newtonsoft.Json.Schema.Generation;

namespace Pokerole.Tools.InitUpdate
{
	class Program
	{
		static void Main(string[] args)
		{

			JSchemaGenerator generator = new JSchemaGenerator();
			var schema = generator.Generate(typeof(PokeroleXmlData));
			Console.Write(schema.ToString());
			//ImageFetcher fetcher = new ImageFetcher();
			//fetcher.FetchImages();
			//String filename = "";
			//var values = PdfParser.ParsePdf(filename);

			//InitialDataImporter importer = new InitialDataImporter();
			//importer.DoImport();
		}

	}
}
