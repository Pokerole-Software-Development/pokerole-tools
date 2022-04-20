using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Pokerole.Core
{
	[XmlRoot]
	public class TypeTableData
	{
		List<TypeTableEntry> Types { get; set; } = new List<TypeTableEntry>();
	}
	public class TypeTableEntry
	{
		public List<ItemReference<ITypeDefinition>> Resistances { get; set; } = new List<ItemReference<ITypeDefinition>>();
		public List<ItemReference<ITypeDefinition>> Weaknesses { get; set; } = new List<ItemReference<ITypeDefinition>>();
		public List<ItemReference<ITypeDefinition>> Immunities { get; set; } = new List<ItemReference<ITypeDefinition>>();
	}
}
