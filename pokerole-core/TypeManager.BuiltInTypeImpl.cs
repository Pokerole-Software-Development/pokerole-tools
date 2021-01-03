/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Pokerole.Core
{
	public static partial class TypeManager
	{
		private static readonly IReadOnlyDictionary<BuiltInType, Guid> baseTypeGuids =
			new ReadOnlyDictionary<BuiltInType, Guid>(new Dictionary<BuiltInType, Guid>
			{
				{ BuiltInType.Normal, Guid.Parse("710bfdd6-99e7-46fe-b70b-8974198027d6") },
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
		private class BuiltInTypeImpl : TypeImpl
		{
			private readonly DataId dataId;
			private readonly BuiltInType type;
			internal BuiltInTypeImpl(BuiltInType type)
			{
				this.type = type;
				this.dataId = new DataId((int)type, baseTypeGuids[type]);
			}
			public override DataId DataId => dataId;
			public override bool IsBuiltInType => true;

			public override string Name => type.ToString();

			//colors grabbed from "Dex Elements.png"
			public override Color? BackgroundColor => type switch
			{
				BuiltInType.Normal => Color.FromArgb(0x9b, 0x90, 0x74),
				BuiltInType.Fire => Color.FromArgb(0xe4, 0x56, 0x33),
				BuiltInType.Fighting => Color.FromArgb(0x9a, 0x4e, 0x34),
				BuiltInType.Water => Color.FromArgb(0x49, 0x7e, 0x97),
				BuiltInType.Flying => Color.FromArgb(0x8b, 0x96, 0xad),
				BuiltInType.Grass => Color.FromArgb(0x6e, 0xa9, 0x4d),
				BuiltInType.Poison => Color.FromArgb(0xa3, 0x55, 0x7e),
				BuiltInType.Electric => Color.FromArgb(0xf7, 0xb6, 0x36),
				BuiltInType.Ground => Color.FromArgb(0xc5, 0xa2, 0x4f),
				BuiltInType.Psychic => Color.FromArgb(0xe9, 0x6d, 0x87),
				BuiltInType.Rock => Color.FromArgb(0xa2, 0x87, 0x43),
				BuiltInType.Ice => Color.FromArgb(0x51, 0xba, 0xb8),
				BuiltInType.Bug => Color.FromArgb(0x9f, 0xaf, 0x30),
				BuiltInType.Dragon => Color.FromArgb(0x68, 0x41, 0x81),
				BuiltInType.Ghost => Color.FromArgb(0x58, 0x5c, 0x88),
				BuiltInType.Dark => Color.FromArgb(0x66, 0x4f, 0x3b),
				BuiltInType.Steel => Color.FromArgb(0x97, 0x93, 0x92),
				BuiltInType.Fairy => Color.FromArgb(0xdd, 0x9b, 0xa7),
				_ => throw new InvalidOperationException("Unknown type: " + type),
			};
		}
	}
}
