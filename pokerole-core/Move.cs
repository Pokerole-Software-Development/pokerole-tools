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

	public enum MonStatus
	{
		None,
		Burn1,
		Burn2,
		Burn3,
		Poisoned,
		Paralyzed,
		Frozen,
		Asleep,
		Confused,
		Disabled,
		Flinched,
		Infatuated,
		/// <summary>
		/// Special value representing all status conditions. Used for healing items
		/// </summary>
		All
	}
}
