using System.Collections.Generic;

namespace Pokerole.Core
{
	public static partial class TypeManager
	{
		private class BuiltInTypeImpl : ITypeDefinition
		{
			private readonly Dictionary<TypeEffectiveness, IReadOnlyList<ITypeDefinition>> defensiveTypeEffectiveness
				= new Dictionary<TypeEffectiveness, IReadOnlyList<ITypeDefinition>>();
			private readonly Dictionary<TypeEffectiveness, IReadOnlyList<ITypeDefinition>> offensiveTypeEffectiveness
				 = new Dictionary<TypeEffectiveness, IReadOnlyList<ITypeDefinition>>();
			private readonly BuiltInType type;
			internal BuiltInTypeImpl(BuiltInType type)
			{
				this.type = type;
			}
		}
	}
}
