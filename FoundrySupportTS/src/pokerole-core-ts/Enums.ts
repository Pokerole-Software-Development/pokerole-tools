namespace pokerole {
	export enum GenderType
	{
		Default,
		None,
		MaleOnly,
		FemaleOnly
	}
	export enum ReviveType
	{
		None,
		Minimum,
		Maximum
	}
	export enum Rarity
	{
		Common,
		Uncommon,
		Rare
	}
	export enum ItemKind
	{
		Unknown,
		IncreasedTypeDamage,
		StatBoost,
		TypeChange,
	}
	export enum EvolutionKind
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
	export enum RivalBackground
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
	export enum RivalAttitudes
	{
		Virtuous,
		Jerk,
		Deceitful,
		Envious,
		Evil
	}
}
