using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using Pokerole.Core.TypeGeneration;

namespace Pokerole.Core
{
	[XmlRoot]
	public class TypeTableData
	{
		public List<TypeTableEntry> Types { get; set; } = new List<TypeTableEntry>();
		public static TypeTableData CreateDefaultTypeTable()
		{
			//populate built-in type effectiveness in bulbapedia order by defense (for simplicity)
			//colors grabbed from "Dex Elements.png"
			(BuiltInType type, Color backgroundColor, BuiltInType[]? weaknesses, BuiltInType[]? resistances, BuiltInType[]? immunities)[] payload =
			#region Data
			{
								(BuiltInType.Normal, Color.FromArgb(0x9b, 0x90, 0x74),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fighting
					},
					//resistances
					null,
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Ghost
					}),
				(BuiltInType.Typeless, Color.FromArgb(0x76, 0x72, 0x66), null, null, null),
				(BuiltInType.Fire, Color.FromArgb(0xe4, 0x56, 0x33),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Ground,
						BuiltInType.Rock,
						BuiltInType.Water,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Fairy,
						BuiltInType.Fire,
						BuiltInType.Grass,
						BuiltInType.Ice,
						BuiltInType.Steel,
					},
					//immunities
					null),
				(BuiltInType.Fighting, Color.FromArgb(0x9a, 0x4e, 0x34),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fairy,
						BuiltInType.Flying,
						BuiltInType.Psychic,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Dark,
						BuiltInType.Rock,
					},
					//immunities
					null),
				(BuiltInType.Water, Color.FromArgb(0x49, 0x7e, 0x97),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Electric,
						BuiltInType.Grass,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Fire,
						BuiltInType.Ice,
						BuiltInType.Steel,
						BuiltInType.Water,
					},
					//immunities
					null),
				(BuiltInType.Flying, Color.FromArgb(0x8b, 0x96, 0xad),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Electric,
						BuiltInType.Ice,
						BuiltInType.Rock,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Fighting,
						BuiltInType.Grass,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Ground
					}),
				(BuiltInType.Grass, Color.FromArgb(0x6e, 0xa9, 0x4d),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Fire,
						BuiltInType.Flying,
						BuiltInType.Ice,
						BuiltInType.Poison,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Electric,
						BuiltInType.Grass,
						BuiltInType.Ground,
						BuiltInType.Water,
					},
					//immunities
					null),
				(BuiltInType.Poison, Color.FromArgb(0xa3, 0x55, 0x7e),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Ground,
						BuiltInType.Psychic,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Poison,
						BuiltInType.Bug,
						BuiltInType.Grass,
						BuiltInType.Fairy,
					},
					//immunities
					null),
				(BuiltInType.Electric, Color.FromArgb(0xf7, 0xb6, 0x36),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Ground
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Electric,
						BuiltInType.Flying,
						BuiltInType.Steel,
					},
					//immunities
					null),
				(BuiltInType.Ground, Color.FromArgb(0xc5, 0xa2, 0x4f),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Grass,
						BuiltInType.Ice,
						BuiltInType.Water,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Poison,
						BuiltInType.Rock,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Electric
					}),
				(BuiltInType.Psychic, Color.FromArgb(0xe9, 0x6d, 0x87),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Dark,
						BuiltInType.Ghost,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Psychic,
					},
					//immunities
					null),
				(BuiltInType.Rock, Color.FromArgb(0xa2, 0x87, 0x43),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Grass,
						BuiltInType.Ground,
						BuiltInType.Steel,
						BuiltInType.Water,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Fire,
						BuiltInType.Flying,
						BuiltInType.Normal,
						BuiltInType.Poison,
					},
					//immunities
					null),
				(BuiltInType.Ice, Color.FromArgb(0x51, 0xba, 0xb8),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Fire,
						BuiltInType.Rock,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Ice
					},
					//immunities
					null),
				(BuiltInType.Bug, Color.FromArgb(0x9f, 0xaf, 0x30),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fire,
						BuiltInType.Flying,
						BuiltInType.Rock,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Grass,
						BuiltInType.Ground,
					},
					//immunities
					null),
				(BuiltInType.Dragon, Color.FromArgb(0x68, 0x41, 0x81),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Dragon,
						BuiltInType.Fairy,
						BuiltInType.Ice,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Electric,
						BuiltInType.Fire,
						BuiltInType.Grass,
						BuiltInType.Water,
					},
					//immunities
					null),
				(BuiltInType.Ghost, Color.FromArgb(0x58, 0x5c, 0x88),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Dark,
						BuiltInType.Ghost,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Poison,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Normal,
						BuiltInType.Fighting,
					}),
				(BuiltInType.Dark, Color.FromArgb(0x66, 0x4f, 0x3b),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Fairy,
						BuiltInType.Fighting,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Dark,
						BuiltInType.Ghost,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Psychic
					}),
				(BuiltInType.Steel, Color.FromArgb(0x97, 0x93, 0x92),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Fighting,
						BuiltInType.Fire,
						BuiltInType.Ground,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Dragon,
						BuiltInType.Fairy,
						BuiltInType.Flying,
						BuiltInType.Grass,
						BuiltInType.Ice,
						BuiltInType.Normal,
						BuiltInType.Psychic,
						BuiltInType.Rock,
						BuiltInType.Steel,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Poison
					}),
				(BuiltInType.Fairy, Color.FromArgb(0xdd, 0x9b, 0xa7),
					//weaknesses
					new BuiltInType[]
					{
						BuiltInType.Poison,
						BuiltInType.Steel,
					},
					//resistances
					new BuiltInType[]
					{
						BuiltInType.Bug,
						BuiltInType.Dark,
						BuiltInType.Fighting,
					},
					//immunities
					new BuiltInType[]
					{
						BuiltInType.Dragon
					}),
			};
			#endregion
			TypeTableData data = new TypeTableData();
			foreach (var (type, color, weakness, resistances, immunities) in payload)
			{
				TypeTableEntry entry = new TypeTableEntry
				{
					Name = type.ToString(),
					BackgroundColor = color
				};
				//grabbbing uuids from this to avoid having to hard code them in the payload above
				entry.DataId = TypeManager.GetBuiltInTypeReference(type).DataId;
				(BuiltInType[]?, List<ItemReference<ITypeDefinition>>)[] lazyList =
				{
					(weakness, entry.Weaknesses),
					(resistances, entry.Resistances),
					(immunities, entry.Immunities)
				};
				foreach (var (array, list) in lazyList)
				{
					if (array == null)
					{
						continue;
					}
					foreach (var item in array)
					{
						list.Add(TypeManager.GetBuiltInTypeReference(item));
					}
				}
				data.Types.Add(entry);
			}
			return data;
		}
	}
	public class TypeTableEntry
	{
		public String Name { get; set; } = "";
		[XmlElement(type: typeof(HtmlColor))]
		public Color? BackgroundColor { get; set; }
		public DataId DataId { get; set; } = default;
		public List<ItemReference<ITypeDefinition>> Resistances { get; set; } = new List<ItemReference<ITypeDefinition>>();
		public List<ItemReference<ITypeDefinition>> Weaknesses { get; set; } = new List<ItemReference<ITypeDefinition>>();
		public List<ItemReference<ITypeDefinition>> Immunities { get; set; } = new List<ItemReference<ITypeDefinition>>();
	}
}
