using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pokerole.Core
{
	public static partial class TypeManager
	{
		private static readonly List<ITypeDefinition> typeList = new List<ITypeDefinition>();
		private static readonly IReadOnlyList<ITypeDefinition> readonlyTypes = typeList.AsReadOnly();
		public static IReadOnlyList<ITypeDefinition> RegisteredTypes
		{
			get
			{
				if (typeList.Count < 1)
				{

				}
			}
		}
		public static ITypeBuilder CreateTypeBuilder()
		{
			throw new NotImplementedException();
		}
		public static ITypeDefinition GetBuiltInType(BuiltInType type)
		{
			throw new NotImplementedException();
		}

		private IReadOnlyList<ITypeDefinition> GetTypeEffectiveness(bool offensive, TypeEffectiveness kind)
		{

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
