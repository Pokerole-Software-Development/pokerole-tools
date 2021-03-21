using System;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Pokerole.Core;

namespace PdfHandler
{ 
	public class PdfParser
	{
		public static PokeroleXmlData ParsePdf(String file)
		{
			PdfDocument document = PdfReader.Open(file, PdfDocumentOpenMode.ReadOnly);

			throw new NotImplementedException();
		}
	}
}
