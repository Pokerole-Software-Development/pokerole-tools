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
		Burned,
		Poisoned,
		Paralyzed,
		Frozen,
		Asleep,
		Confused,
		Disabled,
		Flinched,
		Infatuated
	}



	public class Move : IDataBackedItem<Move>
	{
		private readonly IDataBacking<Move> backing;
		public Move(IDataBacking<Move> backing)
		{
			ArgCheck.NotNull(backing, nameof(backing));
			this.backing = backing;
		}
		public IDataBacking<Move> GetDataBacking() => backing;
		public int ItemId => backing.ItemId;
		//Note: Using CallerMemberNameAttribute to avoid duplicate nameof() "calls"
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

		public int Power
		{
			get { return backing.GetValue<int>(); }
			set { backing.SetValue(value); }
		}
		public MoveCategory Category
		{
			get { return backing.GetValue<MoveCategory>(); }
			set { backing.SetValue(value); }
		}
		public ITypeDefinition Type
		{
			get { return backing.GetValue<ITypeDefinition>(); }
			set { backing.SetValue(value); }
		}
		public bool Ranged
		{
			get { return backing.GetValue<bool>(); }
			set { backing.SetValue(value); }
		}
		public IList<ISkill> Accuracy
		{
			get { return backing.GetValue<IList<ISkill>>(); }
			set { backing.SetValue(value); }
		}
		public int ReducedAccuracy
		{
			get { return backing.GetValue<int>(); }
			set { backing.SetValue(value); }
		}
		public ISkill DamageSkill
		{
			get { return backing.GetValue<ISkill>(); }
			set { backing.SetValue(value); }
		}
		public int DamageModifier
		{
			get { return backing.GetValue<int>(); }
			set { backing.SetValue(value); }
		}
		public bool HasSpecialAccuracyMod
		{
			get { return backing.GetValue<bool>(); }
			set { backing.SetValue(value); }
		}
		public bool HasSpecialDamageMod
		{
			get { return backing.GetValue<bool>(); }
			set { backing.SetValue(value); }
		}
		public String AdditionalInfo
		{
			get { return backing.GetValue<String>(); }
			set { backing.SetValue(value); }
		}
		public MoveTarget Target
		{
			get { return backing.GetValue<MoveTarget>(); }
			set { backing.SetValue(value); }
		}
		public IList<IMoveEffect> Effects
		{
			get { return backing.GetValue<IList<IMoveEffect>>(); }
			set { backing.SetValue(value); }
		}

	}
}
