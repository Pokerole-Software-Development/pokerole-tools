using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Pokerole.Core
{
	public interface IDataItem
	{
		public DataId DataId { get; }
	}
	public abstract record BaseDataItem : IDataItem
	{
		private readonly DataId dataId;
		public DataId DataId => dataId;
		protected BaseDataItem(DataId id)
		{
			dataId = id;
		}
	}
	public abstract class ItemBuilder<T>
	{
		/// <summary>
		/// Whether or not all of the required Properites of this instance are set to build a new
		/// <see cref="T"/>. <see cref="Build"/> will throw an exception if this returns false.
		/// </summary>
		public abstract bool IsValid { get; }
		/// <summary>
		/// Build and instance of <see cref="T"/> from this Builder
		/// </summary>
		/// <returns>A new instance of <see cref="T"/></returns>
		/// <exception cref="InvalidOperationException">If this method is called when not all required properties
		/// have been set</exception>
		public abstract T Build();
		public static void BuildList(IEnumerable<ItemBuilder<T>> list, List<T> destinationList)
		{
			destinationList.AddRange(list.Select(item => item.Build()));
		}
		public static List<T> BuildList(IEnumerable<ItemBuilder<T>> list)
		{
			return list.Select(item => item.Build()).ToList();
		}

	}
	public abstract class DataItemBuilder<T> : ItemBuilder<T> where T : IDataItem
	{
		[XmlElement(IsNullable = false)]
		public DataId? DataId {get;set; }
	}
	public interface IEffect { }
	public class ImageRef { }
	public record Height
	{
		public Height(String value)
		{
			Value = value;
		}
		[XmlText]
		public String Value { get; }
	}
	public record Weight
	{
		public Weight(String value)
		{
			Value = value;
		}
		[XmlText]
		public String Value { get; }
	}
	public record ItemReference<T> where T : IDataItem
	{
		public ItemReference(DataId id) : this(id, null) { }
		public ItemReference(DataId id, String? name)
		{
			DataId = id;
			DisplayName = name;
		}
		[XmlAttribute]
		public String? DisplayName { get; }
		[XmlElement(IsNullable = false)]
		public DataId DataId { get; }
	}
	[XmlRoot("PokeroleData")]
	public class PokeroleXmlData
	{
		[XmlAnyAttribute]
		public XmlAttribute[]? AnyAttribute { get; set; }
		[XmlAnyElement]
		public XmlElement[]? Any { get; set; }
		//custom types not implemented yet
		[XmlArray]
		[XmlArrayItem("Move")]
		public List<Move.Builder> Moves { get; set; } = new List<Move.Builder>();
		[XmlArray]
		[XmlArrayItem("DexEntry")]
		public List<DexEntry.Builder> DexEntries { get; set; } = new List<DexEntry.Builder>();
		[XmlArray]
		[XmlArrayItem("Ability")]
		public List<Ability.Builder> Abilities { get; set; } = new List<Ability.Builder>();
		[XmlArray]
		[XmlArrayItem("EvolutionList")]
		public List<EvolutionList.Builder> EvolutionLists { get; set; } = new List<EvolutionList.Builder>();
		[XmlArray]
		[XmlArrayItem("MonInstance")]
		public List<MonInstance.Builder> MonInstances { get; set; } = new List<MonInstance.Builder>();
		//[XmlArray]
		//[XmlArrayItem("DexEntry")]
		//public List<DexEntry.Builder> DexEntries { get; set; } = new List<DexEntry.Builder>();

	}
	class DataItems
	{
	}
}
