﻿/* This Source Code Form is subject to the terms of the Mozilla Public
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
				BuiltInType.Typeless => Color.FromArgb(0x76, 0x72, 0x66),
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
