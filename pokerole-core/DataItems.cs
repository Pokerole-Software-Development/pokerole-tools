/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Pokerole.Core
{
	public readonly struct DataId : IEquatable<DataId>
	{
		internal static readonly XmlSerializer dataIdSerializer = new XmlSerializer(typeof(DataId.Builder));
		//This is a nullable int since "0" is a valid db id and we don't want to have to worry about that issue
		public int? DbId { get; }
		public Guid Uuid { get; }
		public DataId(int? dbId, Guid uuid)
		{
			DbId = dbId;
			Uuid = uuid;
		}
		[XmlType(nameof(DataId), Namespace = "https://www.pokeroleproject.com/schemas/ExternalTypes.xsd")]
		public class Builder
		{
			public Builder() { }
			public Builder(DataId id)
			{
				DbId = id.DbId;
				Uuid = id.Uuid;
			}
			public int? DbId { get; set; }
			public Guid Uuid { get; set; }
			public DataId Build()
			{
				return new DataId(DbId, Uuid);
			}
		}

		public override bool Equals(object? obj) => obj is DataId id && Equals(id);
		public bool Equals(DataId other) => DbId == other.DbId && Uuid.Equals(other.Uuid);
		public override int GetHashCode() => HashCode.Combine(DbId, Uuid);

		public static bool operator ==(DataId left, DataId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(DataId left, DataId right)
		{
			return !(left == right);
		}
		public override string ToString()
		{
			return $"DbId = {DbId}, Uuid = {Uuid}";
		}
	}
	public readonly struct ItemReference<T> : IEquatable<ItemReference<T>> where T : IDataItem<T>
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

		public override bool Equals(object? obj) => obj is ItemReference<T> reference && Equals(reference);

		public bool Equals([AllowNull] ItemReference<T> other) => DataId == other.DataId && DisplayName == other.DisplayName;
		public override int GetHashCode() => HashCode.Combine(DisplayName, DataId);
		public override string ToString()
		{
			if (String.IsNullOrEmpty(DisplayName))
			{
				return DataId.ToString();
			}
			return $"DisplayName = {DisplayName}, {DataId}";
		}

		//[XmlType(nameof(ItemReference<T>), Namespace = "https://www.pokeroleproject.com/schemas/ExternalTypes.xsd")]
		//have to implement IXmlSerializable to make things work?
		public class Builder : ItemBuilder<ItemReference<T>>, IXmlSerializable
		{
			//private static readonly XmlSerializer dataIdSerializer = new XmlSerializer(typeof(DataId));
			[XmlIgnore]
			public DataId DataId { get; set; }
			//[Browsable(false)]
			//[DebuggerHidden]
			//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			//public DataId.Builder DataIdXmlAccessor
			//{
			//	get => new DataId.Builder(DataId);
			//	set => DataId = value.Build();
			//}
			public String? DisplayName { get; set; }
			public Builder() { }
			public Builder(ItemReference<T> item)
			{
				DataId = item.DataId;
				DisplayName = item.DisplayName;
			}
			//an empty instance of this struct is technically valid
			public override bool IsValid => true;
			public override ItemReference<T> Build()
			{
				return new ItemReference<T>(DataId, DisplayName);
			}
			public override List<string> MissingValues => new List<string>(0);

			public XmlSchema? GetSchema() => null;
			public void ReadXml(XmlReader reader)
			{
				reader.MoveToContent();
				if (reader.HasAttributes)
				{
					DisplayName = reader.GetAttribute(nameof(DisplayName));
				}
				reader.ReadStartElement();
				DataId = ((DataId.Builder)DataId.dataIdSerializer.Deserialize(reader)).Build();
				reader.ReadEndElement();
			}
			public void WriteXml(XmlWriter writer)
			{
				if (!String.IsNullOrEmpty(DisplayName))
				{
					writer.WriteAttributeString(nameof(DisplayName), DisplayName);
				}
				DataId.dataIdSerializer.Serialize(writer, new DataId.Builder(DataId));
			}
		}
	}
	public interface IDataItem<T> where T : IDataItem<T>
	{
		public DataId DataId { get; }
		public ItemReference<T> ItemReference { get; }
	}
	public abstract record BaseDataItem<T> : IDataItem<T> where T : BaseDataItem<T>
	{
		private readonly DataId dataId;
		public DataId DataId => dataId;
		public abstract ItemReference<T> ItemReference { get; }
		/// <summary>
		/// Whether or not this item is out of date. If true, you should replace this instance with a fresher one from
		/// wherever you get your data (like a database)
		/// </summary>
		public bool OutOfDate { get; private set; }
		public void MarkOutOfDate() => OutOfDate = true;

		protected BaseDataItem(DataId id)
		{
			dataId = id;
		}
	}
	//for reflection convinience
	public interface IItemBuilder
	{
		bool IsValid { get; }
		Object Build();
		/// <summary>
		/// Which properties of this instance are not set, but should be set. Generally for debugging.
		/// </summary>
		List<String> MissingValues { get; }
	}
	public abstract class ItemBuilder<T> : IItemBuilder
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
		Object IItemBuilder.Build()
		{
			return Build()!;
		}
		public static void BuildList(IEnumerable<ItemBuilder<T>> list, List<T> destinationList)
		{
			destinationList.AddRange(list.Select(item => item.Build()));
		}
		public static List<T> BuildList(IEnumerable<ItemBuilder<T>> list)
		{
			return list.Select(item => item.Build()).ToList();
		}
		public abstract List<String> MissingValues { get; }

	}
	public abstract class DataItemBuilder<T> : ItemBuilder<T> where T : IDataItem<T>
	{
		[XmlIgnore]
		public DataId? DataId {get;set; }


		[Browsable(false)]
		[DebuggerHidden]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlElement("DataId", IsNullable = false)]
		public DataId.Builder DataIdXmlAccessor
		{
			get => new DataId.Builder(DataId ?? default);
			set => DataId = value.Build();
		}
		public abstract ItemReference<T>? ItemReference { get; }

		//[Browsable(false)]
		//[DebuggerHidden]
		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		//[XmlElement("DataId", IsNullable = false)]
		//public DataId DataIdAccessor
		//{
		//	get => DataId ?? default;
		//	set => DataId = value;
		//}
	}
	public interface IEffect { }
	public partial record DexEntry
	{
		public partial class Builder
		{
			public override string ToString()
			{
				return $"Dex number: {DexNum} Name: {Name}";
			}
		}
	}
	//public class ImageRef {
	//	public String ImagePath { get; }
	//	//public 
	//}
	[XmlRoot("PokeroleData", Namespace = "https://www.pokeroleproject.com/schemas/Structures.xsd")]
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
		[XmlArrayItem("ImageRef")]
		public List<ImageRef.Builder> Images { get; set; } = new List<ImageRef.Builder>();
		[XmlArray]
		[XmlArrayItem("Item")]
		public List<Item.Builder> Items { get; set; } = new List<Item.Builder>();
		[XmlArray]
		[XmlArrayItem("DexEntry")]
		public List<DexEntry.Builder> DexEntries { get; set; } = new List<DexEntry.Builder>();
		[XmlArray]
		[XmlArrayItem("Ability")]
		public List<Ability.Builder> Abilities { get; set; } = new List<Ability.Builder>();
		[XmlArray]
		[XmlArrayItem("EvolutionLists")]
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
