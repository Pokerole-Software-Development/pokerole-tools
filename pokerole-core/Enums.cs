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
		Invalid
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
}
