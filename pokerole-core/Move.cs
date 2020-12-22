/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokerole.Core
{
	[Flags]
	public enum MoveCategory
	{
		Invalid = 0,
		Physical = 1>>0,
		Special = 1>>1,
		Support = 1>>2
	}
	public enum MoveTarget
	{
		Foe,
		RandomFoe,
		AllFoes,
		User,
		OneAlly,
		UserAndAllies,
		Area,
		Battlefield
	}

	public class Move : IDataBackedItem<Move>
	{
		private readonly IDataBacking<Move> backing;
		public Move(IDataBacking<Move> backing)
		{
			this.backing = backing;
		}
		public IDataBacking<Move> GetDataBacking() => backing;
		public String Name
		{
			get { return backing.GetValue<String>(); }
			set { backing.SetValue(value); }
		}
		public String Description
		{
			get { return backing.GetValue<String>(); }
			set { backing.SetValue(value); }
		}
	}
}
