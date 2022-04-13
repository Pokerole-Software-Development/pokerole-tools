/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
		public ItemReference(DataId id, String? name): this(id, name, false) { }
		internal ItemReference(DataId id, String? name, bool builtIn)
		{
			DataId = id;
			DisplayName = name;
			BuiltIn = builtIn;
		}
		[XmlAttribute]
		public String? DisplayName { get; }
		[XmlElement(IsNullable = false)]
		public DataId DataId { get; }
		/// <summary>
		/// Hint to the caller about whether or not this ItemReference references something built-in like the Normal Type
		/// </summary>
		public bool BuiltIn { get; }

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
			/// <summary>
			/// Hint to the caller about whether or not this ItemRefernce references something built-in like the Normal Type
			/// </summary>
			public bool BuiltIn { get; internal set; }
			public Builder() { }
			public Builder(ItemReference<T> item)
			{
				DataId = item.DataId;
				DisplayName = item.DisplayName;
				BuiltIn = item.BuiltIn;
			}
			//an empty instance of this struct is technically valid
			public override bool IsValid => true;
			public override ItemReference<T> Build()
			{
				return new ItemReference<T>(DataId, DisplayName, BuiltIn);
			}
			public override List<string> MissingValues => new List<string>(0);
			public override (string, object?)[] Values => new (string, object?)[] {
				(nameof(DataId), DataId),
				(nameof(DisplayName), DisplayName),
				(nameof(BuiltIn), BuiltIn)
			};

			public XmlSchema? GetSchema() => null;
			public void ReadXml(XmlReader reader)
			{
				reader.MoveToContent();
				if (reader.HasAttributes)
				{
					DisplayName = reader.GetAttribute(nameof(DisplayName));
					String rawBultIn = reader.GetAttribute(nameof(BuiltIn));
					//would bool.TryParse, but that doesn't cover all Xml Cases
					if (!String.IsNullOrEmpty(rawBultIn))
					{
						try
						{
							BuiltIn = XmlConvert.ToBoolean(rawBultIn);
						}
						catch (FormatException)
						{
							BuiltIn = false;
						}
					}
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
					writer.WriteAttributeString(nameof(BuiltIn), BuiltIn.ToString());
				}
				DataId.dataIdSerializer.Serialize(writer, new DataId.Builder(DataId));
			}
		}
	}
	//non-generic interface for reflection handling
	public interface IDataItem
	{
		public DataId DataId { get; }
		/// <summary>
		/// Get the values of this instance. Faster than reflection, hopefully
		/// </summary>
		(String, Object?)[] Values { get; }
	}
	public interface IDataItem<T> : IDataItem where T : IDataItem<T>
	{
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
		public abstract (String, Object?)[] Values { get; }
	}
	public abstract class MutableBaseDataItem<T> : IDataItem<T> where T : MutableBaseDataItem<T>
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

		protected MutableBaseDataItem(DataId id)
		{
			dataId = id;
		}
		public abstract (String, Object?)[] Values { get; }
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
		/// <summary>
		/// What it the type of item that this builder makes?
		/// </summary>
		Type BuilderType { get; }
		(String, Object?)[] Values { get; }
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

		public Type BuilderType => typeof(T);

		public abstract (string, object?)[] Values { get; }
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
	public partial record EvolutionTree
	{
		/// <summary>
		/// Pokemon we know belong to this tree
		/// </summary>
		public List<ItemReference<DexEntry>> ApplicableMon
		{
			get
			{
				List<ItemReference<DexEntry>> result = new List<ItemReference<DexEntry>>(1 + EvolutionEntries.Count);
				result.Add(Root);
				result.AddRange(EvolutionEntries.Select(entry => entry.To));
				return result;
			}
		}
		public partial class Builder
		{
			/// <summary>
			/// Pokemon we know belong to this tree... Out of what is set so far that is...
			/// </summary>
			public List<ItemReference<DexEntry>> KnownApplicableMon
			{
				get
				{
					List<ItemReference<DexEntry>> result = new List<ItemReference<DexEntry>>(1 + EvolutionEntries.Count);
					if (Root != null)
					{
						result.Add(Root.Value);
					}
					result.AddRange(from entry in EvolutionEntries where entry != null && entry?.To != null select entry.To);
					return result;
				}
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
		[XmlArrayItem("EvolutionTrees")]
		public List<EvolutionTree.Builder> EvolutionTrees { get; set; } = new List<EvolutionTree.Builder>();
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
