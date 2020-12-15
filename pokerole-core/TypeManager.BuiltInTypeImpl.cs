using System.Collections.Generic;
using System.Drawing;
using System;

namespace Pokerole.Core
{
	public static partial class TypeManager
	{
		private class BuiltInTypeImpl : TypeImpl
		{
			private readonly BuiltInType type;
			internal BuiltInTypeImpl(BuiltInType type)
			{
				this.type = type;
			}

			public override bool IsBuiltInType => true;

			public override string Name => type.ToString();

			public override Color? BackgroundColor
			{
				get
				{
					switch (type)
					{
						case BuiltInType.Normal:
							break;
						case BuiltInType.Fire:
							break;
						case BuiltInType.Fighting:
							break;
						case BuiltInType.Water:
							break;
						case BuiltInType.Flying:
							break;
						case BuiltInType.Grass:
							break;
						case BuiltInType.Poison:
							break;
						case BuiltInType.Electric:
							break;
						case BuiltInType.Ground:
							break;
						case BuiltInType.Psychic:
							break;
						case BuiltInType.Rock:
							break;
						case BuiltInType.Ice:
							break;
						case BuiltInType.Bug:
							break;
						case BuiltInType.Dragon:
							break;
						case BuiltInType.Ghost:
							break;
						case BuiltInType.Dark:
							break;
						case BuiltInType.Steel:
							break;
						case BuiltInType.Fairy:
							break;
						default:
							throw new InvalidOperationException("Unknown type: " + type);
					}
				}
			}
		}
	}
}
