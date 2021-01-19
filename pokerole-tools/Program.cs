using System;
using System.Xml.Serialization;
using Pokerole.Core;

namespace Pokerole.Tools
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PokeroleXmlData));
			PokeroleXmlData data = new PokeroleXmlData();
			xmlSerializer.Serialize(Console.Out, data);
			Console.WriteLine("Hello World!");
		}
	}
}
