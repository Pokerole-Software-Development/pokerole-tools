/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;

namespace Pokerole.Core
{
	public enum GenderType
	{
		Default,
		None,
		MaleOnly,
		FemaleOnly
	}
	public enum ReviveType
	{
		None,
		Minimum,
		Maximum
	}
	public enum Rarity
	{
		Common,
		Uncommon,
		Rare
	}
	public enum ItemKind
	{
		Unknown,
		IncreasedTypeDamage,
		StatBoost,
		TypeChange,
	}
	public enum EvolutionKind
	{
		None,
		Level,
		Mega,
		Stat,
		Stone,
		Trade,
		Special,
		Item,
		Form
	}
	public enum RivalBackground
	{
		Hax0rus,
		/// <summary>
		/// Rule of Tenta-Cool
		/// </summary>
		Tentacool,
		/// <summary>
		/// Cursed with Awesome
		/// </summary>
		AwesomeCurse,
		Richboy,
		TeamMember,
		DisabledOrphan,
		AntiHero,
		TyrantMentor,
	}
	public enum RivalAttitudes
	{
		Virtuous,
		Jerk,
		Deceitful,
		Envious,
		Evil
	}

}
