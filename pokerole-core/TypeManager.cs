using System;
using System.Collections.Generic;
using System.Drawing;
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
		private static readonly Dictionary<ITypeDefinition, (ITypeDefinition type, TypeEffectiveness effectiveness)>
			offensiveTypeEffectivenessDictionary = new Dictionary<ITypeDefinition, (ITypeDefinition, TypeEffectiveness)>(20);
		private static readonly Dictionary<ITypeDefinition, (ITypeDefinition type, TypeEffectiveness effectiveness)>
			defensiveTypeEffectivenessDictionary = new Dictionary<ITypeDefinition, (ITypeDefinition, TypeEffectiveness)>(20);
		private static readonly Object initLock = new object();
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
			AddTypeEffectivenessEntry(BuiltInType.Electric, BuiltInType., TypeEffectiveness.SuperEffective);


			#endregion

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
				offensiveTypeEffectivenessDictionary.Add(attacker, (defender, effectiveness));
				defensiveTypeEffectivenessDictionary.Add(defender, (attacker, effectiveness));
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
		public static ITypeDefinition GetBuiltInType(BuiltInType type)
		{
			CheckInit();
			if (!builtInTypeImplementations.TryGetValue(type, out BuiltInTypeImpl? item))
			{
				throw new ArgumentException($"'{type}' is not a valid built-in type or has not been registered");
			}
			return item;
		}

		private static EffectivenessCache GetTypeEffectiveness(bool offensive, TypeEffectiveness kind)
		{
			List<ITypeDefinition> resultList = new List<ITypeDefinition>(20);
			int startVersion, endVersion;
			do
			{
				resultList.Clear();
				startVersion = typeEffectivenessModificationCount;
				//code here
#error not implemented
				endVersion = typeEffectivenessModificationCount;
			} while (startVersion != endVersion);//in case things change while we are processing
			return (startVersion, resultList);
		}
		private abstract class TypeImpl : ITypeDefinition
		{
			private readonly Dictionary<TypeEffectiveness, EffectivenessCache>
				defensiveTypeEffectiveness = new Dictionary<TypeEffectiveness, EffectivenessCache>();
			private readonly Dictionary<TypeEffectiveness, EffectivenessCache>
				offensiveTypeEffectiveness = new Dictionary<TypeEffectiveness, EffectivenessCache>();

			public abstract bool IsBuiltInType { get; }
			public abstract string Name { get; }
			public abstract Color? BackgroundColor { get; }

			public IReadOnlyList<ITypeDefinition> GetDefensiveEffectiveness(TypeEffectiveness kind)
			{

			}
			public IReadOnlyList<ITypeDefinition> GetOffensiveEffectiveness(TypeEffectiveness kind)
			{

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
	public interface ITypeDefinition
	{
		bool IsBuiltInType { get; }
		String Name { get; }
		IReadOnlyList<ITypeDefinition> GetOffensiveEffectiveness(TypeEffectiveness kind);
		IReadOnlyList<ITypeDefinition> GetDefensiveEffectiveness(TypeEffectiveness kind);
		Color? BackgroundColor { get; }
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
