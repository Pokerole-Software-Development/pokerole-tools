/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pokerole.Core
{
	public static partial class TypeManager
	{
		private static bool initted = false;
		private static readonly List<ITypeDefinition> typeList = new List<ITypeDefinition>();
		private static readonly IReadOnlyList<ITypeDefinition> readonlyTypes = typeList.AsReadOnly();
		private static readonly Dictionary<BuiltInType, BuiltInTypeImpl> builtInTypeImplementations =
			new Dictionary<BuiltInType, BuiltInTypeImpl>(20);
		private static volatile int typeEffectivenessModificationCount = 0;
		private static readonly Dictionary<ITypeDefinition, HashSet<EffectivenessNode>>
			offensiveTypeEffectivenessDictionary = new Dictionary<ITypeDefinition, HashSet<EffectivenessNode>>(20);
		private static readonly Dictionary<ITypeDefinition, HashSet<EffectivenessNode>>
			defensiveTypeEffectivenessDictionary = new Dictionary<ITypeDefinition, HashSet<EffectivenessNode>>(20);
		private static readonly Object initLock = new object();
		private static readonly IReadOnlyDictionary<BuiltInType, Guid> baseTypeGuids =
			new ReadOnlyDictionary<BuiltInType, Guid>(new Dictionary<BuiltInType, Guid>
			{
				{ BuiltInType.Normal, Guid.Parse("710bfdd6-99e7-46fe-b70b-8974198027d6") },
				{ BuiltInType.Typeless, Guid.Parse("c684acad-7bc7-48d9-8794-9747ce2fd369") },
				{ BuiltInType.Fire, Guid.Parse("2639b94f-8e09-4677-b8cf-cf582c0c7cf6") },
				{ BuiltInType.Fighting, Guid.Parse("f648ba4f-c048-4cde-9213-af66fd0846dd") },
				{ BuiltInType.Water, Guid.Parse("72b046c4-6656-4b78-964e-bd901eb2cd21") },
				{ BuiltInType.Flying, Guid.Parse("e4b50b65-678d-42de-9481-79777de6a305") },
				{ BuiltInType.Grass, Guid.Parse("da456900-6cb9-44fb-8d38-2765809539d2") },
				{ BuiltInType.Poison, Guid.Parse("a7560579-4d50-49c0-bf3f-240cef481463") },
				{ BuiltInType.Electric, Guid.Parse("402bd9f6-3d3d-46ce-bc35-3874fad7c644") },
				{ BuiltInType.Ground, Guid.Parse("2659fc24-54a6-498e-b0a2-dda4677246d7") },
				{ BuiltInType.Psychic, Guid.Parse("7209d2a4-e796-4b15-a7ad-569670390ea2") },
				{ BuiltInType.Rock, Guid.Parse("555ced86-dda5-4361-b31f-ba917f2b3ecf") },
				{ BuiltInType.Ice, Guid.Parse("34a04bf1-c17a-4345-ac37-f54bd89535f0") },
				{ BuiltInType.Bug, Guid.Parse("04a95a1a-ff84-4906-bbff-9e8e6d2c7ac5") },
				{ BuiltInType.Dragon, Guid.Parse("f15029fa-b047-46ad-b388-12112b98253c") },
				{ BuiltInType.Ghost, Guid.Parse("17b137d5-7509-4ac7-aa60-6d99b6e3b0c9") },
				{ BuiltInType.Dark, Guid.Parse("5a76513e-176b-4687-8fdf-604882fef10a") },
				{ BuiltInType.Steel, Guid.Parse("5223656b-9629-4728-b82e-92eddd4b74e8") },
				{ BuiltInType.Fairy, Guid.Parse("9d5f2e81-b827-4cb7-a81e-b7b71a551d86") },

			});
		public static IReadOnlyList<ITypeDefinition> RegisteredTypes
		{
			get
			{
				CheckInit();
				return readonlyTypes;
			}
		}

		private static void CheckInit()
		{
			if (!initted)
			{
				lock (initLock)
				{
					if (!initted)
					{
						Init();
					}
				}
			}
		}

		private static void Init()
		{
			//just in case...
			typeList.Clear();
			builtInTypeImplementations.Clear();
			BuiltInType[] builtInTypes = (BuiltInType[])Enum.GetValues(typeof(BuiltInType));
			if (typeList.Capacity < builtInTypes.Length)
			{
				typeList.Capacity = builtInTypes.Length;
			}
			foreach (var type in builtInTypes)
			{
				var instance = new BuiltInTypeImpl(type);
				builtInTypeImplementations[type] = instance;
				typeList.Add(instance);
			}
			//populate built-in type effectiveness in bulbapedia order by offense (for simplicity)
			#region Default Type Effectiveness Table
			//Normal
			AddTypeEffectivenessEntry(BuiltInType.Normal, BuiltInType.Rock, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Normal, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Normal, BuiltInType.Ghost, TypeEffectiveness.NoEffect);
			//Fire
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Bug, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Grass, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Ice, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Steel, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Dragon, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Rock, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fire, BuiltInType.Water, TypeEffectiveness.Ineffective);
			//Fighting
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Dark, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Ice, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Normal, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Rock, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Steel, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Bug, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Fairy, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Flying, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Poison, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Psychic, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fighting, BuiltInType.Ghost, TypeEffectiveness.NoEffect);
			//Water
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Fire, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Ground, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Rock, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Dragon, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Grass, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Water, BuiltInType.Water, TypeEffectiveness.Ineffective);
			//Flying
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Bug, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Fighting, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Grass, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Electric, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Rock, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Flying, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			//Grass
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Ground, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Rock, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Water, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Bug, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Dragon, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Flying, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Grass, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Poison, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Grass, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			//Poison
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Fairy, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Grass, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Poison, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Ground, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Rock, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Ghost, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Poison, BuiltInType.Steel, TypeEffectiveness.NoEffect);
			//Electric
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Flying, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Water, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Dragon, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Electric, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Grass, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType.Ground, TypeEffectiveness.NoEffect);
			//Ground
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Electric, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Fire, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Poison, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Rock, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Steel, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Bug, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Grass, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ground, BuiltInType.Flying, TypeEffectiveness.NoEffect);
			//Psychic
			AddTypeEffectivenessEntry(BuiltInType.Psychic, BuiltInType.Fighting, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Psychic, BuiltInType.Poison, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Psychic, BuiltInType.Psychic, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Psychic, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Psychic, BuiltInType.Dark, TypeEffectiveness.NoEffect);
			//Rock
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Bug, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Fire, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Flying, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Ice, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Fighting, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Ground, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Rock, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			//Ice
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Dragon, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Flying, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Ground, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Ground, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Ice, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ice, BuiltInType.Water, TypeEffectiveness.Ineffective);
			//Bug
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Dark, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Grass, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Psychic, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Fairy, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Fighting, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Flying, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Ghost, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Poison, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Bug, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			//Dragon
			AddTypeEffectivenessEntry(BuiltInType.Dragon, BuiltInType.Dragon, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Dragon, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Dragon, BuiltInType.Fairy, TypeEffectiveness.NoEffect);
			//Ghost
			AddTypeEffectivenessEntry(BuiltInType.Ghost, BuiltInType.Ghost, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ghost, BuiltInType.Psychic, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Ghost, BuiltInType.Dark, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Ghost, BuiltInType.Normal, TypeEffectiveness.NoEffect);
			//Dark
			AddTypeEffectivenessEntry(BuiltInType.Dark, BuiltInType.Ghost, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Dark, BuiltInType.Psychic, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Dark, BuiltInType.Dark, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Dark, BuiltInType.Fairy, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Dark, BuiltInType.Fighting, TypeEffectiveness.Ineffective);
			//Steel
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Fairy, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Ice, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Rock, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Electric, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Steel, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Steel, BuiltInType.Water, TypeEffectiveness.Ineffective);
			//Fairy
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Dark, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Dragon, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Fighting, TypeEffectiveness.SuperEffective);
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Fire, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Poison, TypeEffectiveness.Ineffective);
			AddTypeEffectivenessEntry(BuiltInType.Fairy, BuiltInType.Steel, TypeEffectiveness.Ineffective);

			#endregion
			initted = true;
		}
		private static void AddTypeEffectivenessEntry(BuiltInType attacker, BuiltInType defender,
			TypeEffectiveness effectiveness)
		{
			AddTypeEffectivenessEntry(builtInTypeImplementations[attacker], builtInTypeImplementations[defender],
				effectiveness);
		}
		private static void AddTypeEffectivenessEntry(ITypeDefinition attacker, ITypeDefinition defender,
			TypeEffectiveness effectiveness)
		{
			void AddItem()
			{
				if (!offensiveTypeEffectivenessDictionary.TryGetValue(attacker, out HashSet<EffectivenessNode>? set))
				{
					set = new HashSet<EffectivenessNode>(20);
					offensiveTypeEffectivenessDictionary.Add(attacker, set);
				}
				set.Add((defender, effectiveness));
				if (!defensiveTypeEffectivenessDictionary.TryGetValue(defender, out set))
				{
					set = new HashSet<EffectivenessNode>(20);
					defensiveTypeEffectivenessDictionary.Add(defender, set);
				}
				set.Add((attacker, effectiveness));
				typeEffectivenessModificationCount++;//this should be the only place this number is modified!
			}
			if (!initted)
			{
				//no lock needed since we are doing initialization
				AddItem();
			}
			else
			{
				lock (offensiveTypeEffectivenessDictionary)
				{
					AddItem();
				}
			}
		}

		public static ITypeBuilder CreateTypeBuilder()
		{
			throw new NotImplementedException("Support for custom types is not implemented yet");
		}
		public static ItemReference<ITypeDefinition> GetBuiltInTypeReference(BuiltInType type)
		{
			if (!baseTypeGuids.TryGetValue(type, out Guid guid))
			{
				throw new ArgumentException($"'{type}' is not a valid built-in type or has not been registered");
			}
			//true for now???
			return new ItemReference<ITypeDefinition>(new DataId(null, guid), type.ToString(), true);
		}
		public static ITypeDefinition GetBuiltInType(BuiltInType type)
		{
			CheckInit();
			if (!builtInTypeImplementations.TryGetValue(type, out BuiltInTypeImpl? item))
			{
				throw new ArgumentException($"'{type}' is not a valid built-in type or has not been registered");
			}
			return item;
		}
		public static (List<ITypeDefinition> resistances, List<ITypeDefinition> weaknesses,
			List<ITypeDefinition> immunities) CalculateEffectiveness(bool offensive, params ITypeDefinition[] definitions)
		{
			return CalculateEffectiveness(offensive, (IEnumerable<ITypeDefinition>)definitions);
		}
		public static (List<ITypeDefinition> resistances, List<ITypeDefinition> weaknesses,
			List<ITypeDefinition> immunities) CalculateEffectiveness(bool offensive, IEnumerable<ITypeDefinition> definitions)
		{
			//null value indicates immunity. 0 indicated a canceled value
			Dictionary<ITypeDefinition, int?> results = new Dictionary<ITypeDefinition, int?>(10);
			foreach (var def in definitions)
			{
				foreach (TypeEffectiveness kind in (TypeEffectiveness[])Enum.GetValues(typeof(TypeEffectiveness)))
				{
					var entries = offensive ? def.GetOffensiveEffectiveness(kind) : def.GetDefensiveEffectiveness(kind);
					foreach (var entry in entries)
					{
						if (!results.TryGetValue(entry, out int? count))
						{
							count = 0;
						}
						//if (count == null)trivia: null + <any> = null
						//{use built-in null handling
						//	//immune
						//	continue;
						//}
						switch (kind)
						{
							case TypeEffectiveness.Normal:
								//should not be here....
								continue;
							case TypeEffectiveness.SuperEffective:
								count++;
								break;
							case TypeEffectiveness.Ineffective:
								count--;
								break;
							case TypeEffectiveness.NoEffect:
								count = null;
								break;
							default:
								throw new InvalidOperationException();
						}
						results[entry] = count;
					}
				}
			}
			List<ITypeDefinition> resistances = new List<ITypeDefinition>(results.Count);
			List<ITypeDefinition> weaknesses = new List<ITypeDefinition>(results.Count);
			List<ITypeDefinition> immunities = new List<ITypeDefinition>(results.Count);
			foreach (var pair in results)
			{
				//note: do nothing if Value == 0
				if (pair.Value == null)
				{
					immunities.Add(pair.Key);
				}
				else if (pair.Value > 0)
				{
					weaknesses.AddRange(Enumerable.Repeat(pair.Key, pair.Value.Value));
				}
				else if (pair.Value < 0)
				{
					resistances.AddRange(Enumerable.Repeat(pair.Key, -pair.Value.Value));
				}
			}
			return (resistances, weaknesses, immunities);
		}

		private static EffectivenessCache GetTypeEffectiveness(ITypeDefinition type, bool offensive,
			TypeEffectiveness kind)
		{
			ArgCheck.NotNull(type, nameof(type));
			CheckInit();
			List<ITypeDefinition> resultList = new List<ITypeDefinition>(20);
			int startVersion, endVersion;
			do
			{
				resultList.Clear();
				startVersion = typeEffectivenessModificationCount;
				//code here
				var targetDict = offensive ? offensiveTypeEffectivenessDictionary : defensiveTypeEffectivenessDictionary;
				if (!targetDict.TryGetValue(type, out HashSet<EffectivenessNode>? entries))
				{
					throw new ArgumentException($"Unknown type: {type.Name}");
				}
				if (kind == TypeEffectiveness.Normal)
				{
					resultList.AddRange(typeList);
					//remove all of the ones we find
					foreach (var item in entries)
					{
						resultList.Remove(item.type);
					}
				}
				else
				{
					foreach (var item in entries)
					{
						if (item.effectiveness == kind)
						{
							resultList.Add(item.type);
						}
					}
				}
				endVersion = typeEffectivenessModificationCount;
			} while (startVersion != endVersion);//in case things change while we are processing
			return (startVersion, resultList.AsReadOnly());
		}
		private abstract class TypeImpl : ITypeDefinition
		{
			private readonly Dictionary<TypeEffectiveness, EffectivenessCache>
				defensiveTypeEffectiveness = new Dictionary<TypeEffectiveness, EffectivenessCache>();
			private readonly Dictionary<TypeEffectiveness, EffectivenessCache>
				offensiveTypeEffectiveness = new Dictionary<TypeEffectiveness, EffectivenessCache>();
			public abstract DataId DataId { get; }
			public abstract bool IsBuiltInType { get; }
			public abstract string Name { get; }
			public abstract Color? BackgroundColor { get; }
			//anything more makes things complicated....

			public (String, Object?)[] Values => new (string, object?)[]{
					(nameof(DataId), DataId),
					(nameof(Name), Name),
					(nameof(BackgroundColor), BackgroundColor)
				};
			public bool Mutable => false;//you need something more than just a builder
			public bool HasBuilder => false;
			public DataKind Kind => DataKind.TypeDefinition;


			public ItemReference<ITypeDefinition> ItemReference => new ItemReference<ITypeDefinition>(DataId, Name);

			private IReadOnlyList<ITypeDefinition> GetEffectiveness(TypeEffectiveness kind,
				Dictionary<TypeEffectiveness, EffectivenessCache> cache, bool offensive)
			{
				if (!cache.TryGetValue(kind, out EffectivenessCache entry))
				{
					return AddNewEntry();
				}
				if (entry.version == typeEffectivenessModificationCount)
				{
					return entry.list;
				}
				cache.Remove(kind);
				return AddNewEntry();

				IReadOnlyList<ITypeDefinition> AddNewEntry()
				{
					entry = GetTypeEffectiveness(this, offensive, kind);
					cache[kind] = entry;
					return entry.list;
				}
			}
			public IReadOnlyList<ITypeDefinition> GetDefensiveEffectiveness(TypeEffectiveness kind)
			{
				return GetEffectiveness(kind, defensiveTypeEffectiveness, false);
			}
			public IReadOnlyList<ITypeDefinition> GetOffensiveEffectiveness(TypeEffectiveness kind)
			{
				return GetEffectiveness(kind, offensiveTypeEffectiveness, true);
			}
		}
		internal readonly struct EffectivenessNode
		{
			public readonly ITypeDefinition type;
			public readonly TypeEffectiveness effectiveness;

			public EffectivenessNode(ITypeDefinition type, TypeEffectiveness effectiveness)
			{
				this.type = type;
				this.effectiveness = effectiveness;
			}

			public override bool Equals(object? obj)
			{
				return obj is EffectivenessNode other &&
					   EqualityComparer<ITypeDefinition>.Default.Equals(type, other.type) &&
					   effectiveness == other.effectiveness;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(type, effectiveness);
			}

			public void Deconstruct(out ITypeDefinition type, out TypeEffectiveness effectiveness)
			{
				type = this.type;
				effectiveness = this.effectiveness;
			}

			public static implicit operator (ITypeDefinition type, TypeEffectiveness effectiveness)(EffectivenessNode value)
			{
				return (value.type, value.effectiveness);
			}

			public static implicit operator EffectivenessNode((ITypeDefinition type, TypeEffectiveness effectiveness) value)
			{
				return new EffectivenessNode(value.type, value.effectiveness);
			}
		}
		internal readonly struct EffectivenessCache
		{
			public readonly int version;
			public readonly IReadOnlyList<ITypeDefinition> list;

			public EffectivenessCache(int version, IReadOnlyList<ITypeDefinition> list)
			{
				this.version = version;
				this.list = list;
			}

			public override bool Equals(object? obj) => obj is EffectivenessCache other && version == other.version && EqualityComparer<IReadOnlyList<ITypeDefinition>>.Default.Equals(list, other.list);
			public override int GetHashCode() => HashCode.Combine(version, list);

			public void Deconstruct(out int version, out IReadOnlyList<ITypeDefinition> list)
			{
				version = this.version;
				list = this.list;
			}

			public static implicit operator (int version, IReadOnlyList<ITypeDefinition> list)(EffectivenessCache value)
			{
				return (value.version, value.list);
			}

			public static implicit operator EffectivenessCache((int version, IReadOnlyList<ITypeDefinition> list) value)
			{
				return new EffectivenessCache(value.version, value.list);
			}
		}
	}
	public interface ITypeBuilder { }
	public interface ITypeDefinition : IDataItem<ITypeDefinition>
	{
		bool IsBuiltInType { get; }
		String Name { get; }
		IReadOnlyList<ITypeDefinition> GetOffensiveEffectiveness(TypeEffectiveness kind);
		IReadOnlyList<ITypeDefinition> GetDefensiveEffectiveness(TypeEffectiveness kind);
		Color? BackgroundColor { get; }
		//ItemReference<ITypeDefinition> ItemReference { get; }
	}
	public enum TypeEffectiveness
	{
		Normal,
		SuperEffective,
		Ineffective,
		NoEffect
	}
	public enum BuiltInType {
		Normal,//normal is first so it will be the default
		Typeless,
		Fire,
		Fighting,
		Water,
		Flying,
		Grass,
		Poison,
		Electric,
		Ground,
		Psychic,
		Rock,
		Ice,
		Bug,
		Dragon,
		Ghost,
		Dark,
		Steel,
		Fairy
	}

}
