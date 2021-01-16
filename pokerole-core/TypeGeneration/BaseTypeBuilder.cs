/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Pokerole.Core{
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record Move : BaseDataItem
	{
		public Move(DataId dataId,
			string name,
			string description,
			int power,
			MoveCategory moveCategory,
			ITypeDefinition type,
			MoveTarget moveTarget,
			bool ranged,
			IList<ISkill> accuracy,
			int reducedAccuracy,
			ISkill? damageSkill,
			int damageModifier,
			bool hasSpecialAccuracyMod,
			bool hasSpecialDamageMod,
			string additionalInfo,
			IList<IEffect> effects) : base(dataId)
		{
			Name = name;
			Description = description;
			Power = power;
			MoveCategory = moveCategory;
			Type = type;
			MoveTarget = moveTarget;
			Ranged = ranged;
			Accuracy = new List<ISkill>(accuracy).AsReadOnly();
			ReducedAccuracy = reducedAccuracy;
			DamageSkill = damageSkill;
			DamageModifier = damageModifier;
			HasSpecialAccuracyMod = hasSpecialAccuracyMod;
			HasSpecialDamageMod = hasSpecialDamageMod;
			AdditionalInfo = additionalInfo;
			Effects = new List<IEffect>(effects).AsReadOnly();
		}
		/// <summary>
		/// Name of the move
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Move's description
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// The power of the move
		/// </summary>
		public int Power { get; }
		/// <summary>
		/// Category of the move
		/// </summary>
		public MoveCategory MoveCategory { get; }
		/// <summary>
		/// Move Type
		/// </summary>
		public ITypeDefinition Type { get; }
		/// <summary>
		/// What this move targets
		/// </summary>
		public MoveTarget MoveTarget { get; }
		/// <summary>
		/// Whether or not this move is ranged
		/// </summary>
		public bool Ranged { get; }
		/// <summary>
		/// Skills used to roll accuracy for this move
		/// </summary>
		public IReadOnlyList<ISkill> Accuracy { get; }
		/// <summary>
		/// How many more successes are needed for this attack to hit
		/// </summary>
		public int ReducedAccuracy { get; }
		/// <summary>
		/// Skill used to roll damage for this move if any
		/// </summary>
		public ISkill? DamageSkill { get; }
		/// <summary>
		/// How many more dice to add to the damage roll pool
		/// </summary>
		public int DamageModifier { get; }
		/// <summary>
		/// Refer to AdditionalInfo if this is true
		/// </summary>
		public bool HasSpecialAccuracyMod { get; }
		/// <summary>
		/// Refer to AdditionalInfo if this is true
		/// </summary>
		public bool HasSpecialDamageMod { get; }
		/// <summary>
		/// More information about this move that could not be contained in the other variables
		/// </summary>
		public string AdditionalInfo { get; }
		/// <summary>
		/// List of effects this move causes when it hits
		/// </summary>
		public IReadOnlyList<IEffect> Effects { get; }
		public class Builder
		{
			public Builder() { }
			public Builder(Move move)
			{
				DataId = move.DataId;
				Name = move.Name;
				Description = move.Description;
				Power = move.Power;
				MoveCategory = move.MoveCategory;
				Type = move.Type;
				MoveTarget = move.MoveTarget;
				Ranged = move.Ranged;
				Accuracy = new List<ISkill>(move.Accuracy);
				ReducedAccuracy = move.ReducedAccuracy;
				DamageSkill = move.DamageSkill;
				DamageModifier = move.DamageModifier;
				HasSpecialAccuracyMod = move.HasSpecialAccuracyMod;
				HasSpecialDamageMod = move.HasSpecialDamageMod;
				AdditionalInfo = move.AdditionalInfo;
				Effects = new List<IEffect>(move.Effects);
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// Name of the move
			/// </summary>
			public string? Name { get; set; }
			/// <summary>
			/// Move's description
			/// </summary>
			public string? Description { get; set; }
			/// <summary>
			/// The power of the move
			/// </summary>
			public int? Power { get; set; }
			/// <summary>
			/// Category of the move
			/// </summary>
			public MoveCategory? MoveCategory { get; set; }
			/// <summary>
			/// Move Type
			/// </summary>
			public ITypeDefinition? Type { get; set; }
			/// <summary>
			/// What this move targets
			/// </summary>
			public MoveTarget? MoveTarget { get; set; }
			/// <summary>
			/// Whether or not this move is ranged
			/// </summary>
			public bool? Ranged { get; set; }
			/// <summary>
			/// Skills used to roll accuracy for this move
			/// </summary>
			public IList<ISkill>? Accuracy { get; set; }
			/// <summary>
			/// How many more successes are needed for this attack to hit
			/// </summary>
			public int? ReducedAccuracy { get; set; }
			/// <summary>
			/// Skill used to roll damage for this move if any
			/// </summary>
			public ISkill? DamageSkill { get; set; }
			/// <summary>
			/// How many more dice to add to the damage roll pool
			/// </summary>
			public int? DamageModifier { get; set; }
			/// <summary>
			/// Refer to AdditionalInfo if this is true
			/// </summary>
			public bool? HasSpecialAccuracyMod { get; set; }
			/// <summary>
			/// Refer to AdditionalInfo if this is true
			/// </summary>
			public bool? HasSpecialDamageMod { get; set; }
			/// <summary>
			/// More information about this move that could not be contained in the other variables
			/// </summary>
			public string? AdditionalInfo { get; set; }
			/// <summary>
			/// List of effects this move causes when it hits
			/// </summary>
			public IList<IEffect>? Effects { get; set; }
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="Move"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					if (Name is null)
					{
						return false;
					}
					if (Description is null)
					{
						return false;
					}
					if (Power is null)
					{
						return false;
					}
					if (MoveCategory is null)
					{
						return false;
					}
					if (Type is null)
					{
						return false;
					}
					if (MoveTarget is null)
					{
						return false;
					}
					if (Ranged is null)
					{
						return false;
					}
					if (Accuracy is null)
					{
						return false;
					}
					if (ReducedAccuracy is null)
					{
						return false;
					}
					if (DamageModifier is null)
					{
						return false;
					}
					if (HasSpecialAccuracyMod is null)
					{
						return false;
					}
					if (HasSpecialDamageMod is null)
					{
						return false;
					}
					if (AdditionalInfo is null)
					{
						return false;
					}
					if (Effects is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="Move"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="Move"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public Move Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new Move(DataId!.Value,
					Name!,
					Description!,
					Power!.Value,
					MoveCategory!.Value,
					Type!,
					MoveTarget!.Value,
					Ranged!.Value,
					Accuracy!,
					ReducedAccuracy!.Value,
					DamageSkill,
					DamageModifier!.Value,
					HasSpecialAccuracyMod!.Value,
					HasSpecialDamageMod!.Value,
					AdditionalInfo!,
					Effects!);
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record Item : BaseDataItem
	{
		public Item(DataId dataId,
			string name,
			string description) : base(dataId)
		{
			Name = name;
			Description = description;
		}
		/// <summary>
		/// Item Name
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Item Description
		/// </summary>
		public string Description { get; }
		public class Builder
		{
			public Builder() { }
			public Builder(Item item)
			{
				DataId = item.DataId;
				Name = item.Name;
				Description = item.Description;
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// Item Name
			/// </summary>
			public string? Name { get; set; }
			/// <summary>
			/// Item Description
			/// </summary>
			public string? Description { get; set; }
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="Item"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					if (Name is null)
					{
						return false;
					}
					if (Description is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="Item"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="Item"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public Item Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new Item(DataId!.Value,
					Name!,
					Description!);
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public record MegaEvolutionEntry
	{
		public MegaEvolutionEntry(ItemReference<Item> item,
			ItemReference<DexEntry> targetEvolution)
		{
			Item = item;
			TargetEvolution = targetEvolution;
		}
		/// <summary>
		/// Item the Pokémon must be holding to perform this megaevolution
		/// </summary>
		public ItemReference<Item> Item { get; }
		/// <summary>
		/// DexEntry to use when mega-evolved
		/// </summary>
		public ItemReference<DexEntry> TargetEvolution { get; }
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public record MoveEntry
	{
		public MoveEntry(Rank rank,
			ItemReference<Move> move)
		{
			Rank = rank;
			Move = move;
		}
		/// <summary>
		/// Required rank to learn the given move
		/// </summary>
		public Rank Rank { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public ItemReference<Move> Move { get; }
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public record AbilityEntry
	{
		public AbilityEntry(bool hidden,
			ItemReference<Ability> ability)
		{
			Hidden = hidden;
			Ability = ability;
		}
		/// <summary>
		/// Whether or not this is a hidden ability
		/// </summary>
		public bool Hidden { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public ItemReference<Ability> Ability { get; }
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record DexEntry : BaseDataItem
	{
		public DexEntry(DataId dataId,
			int dexNum,
			bool suggestedStarer,
			ITypeDefinition primaryType,
			ITypeDefinition? secondaryType,
			string name,
			string? variant,
			Height averageHeight,
			Weight averageWeight,
			string category,
			string dexDescription,
			Rank suggestedRank,
			int baseHp,
			ImageRef primaryImage,
			ImageRef smallImage,
			ImageRef? shinyImage,
			ImageRef? smallShinyImage,
			IList<AbilityEntry> abilities,
			ItemReference<EvolutionList>? evolutionList,
			ItemReference<DexEntry>? megaEvolutionBaseEntry,
			IList<MegaEvolutionEntry> megaEvolutions,
			int maxStrength,
			int startingStrength,
			int maxDexterity,
			int startingDexterity,
			int maxVitality,
			int startingVitality,
			int maxSpecial,
			int startingSpecial,
			int maxInsight,
			int startingInsight,
			IList<MoveEntry> moveSet) : base(dataId)
		{
			DexNum = dexNum;
			SuggestedStarer = suggestedStarer;
			PrimaryType = primaryType;
			SecondaryType = secondaryType;
			Name = name;
			Variant = variant;
			AverageHeight = averageHeight;
			AverageWeight = averageWeight;
			Category = category;
			DexDescription = dexDescription;
			SuggestedRank = suggestedRank;
			BaseHp = baseHp;
			PrimaryImage = primaryImage;
			SmallImage = smallImage;
			ShinyImage = shinyImage;
			SmallShinyImage = smallShinyImage;
			Abilities = new List<AbilityEntry>(abilities).AsReadOnly();
			EvolutionList = evolutionList;
			MegaEvolutionBaseEntry = megaEvolutionBaseEntry;
			MegaEvolutions = new List<MegaEvolutionEntry>(megaEvolutions).AsReadOnly();
			MaxStrength = maxStrength;
			StartingStrength = startingStrength;
			MaxDexterity = maxDexterity;
			StartingDexterity = startingDexterity;
			MaxVitality = maxVitality;
			StartingVitality = startingVitality;
			MaxSpecial = maxSpecial;
			StartingSpecial = startingSpecial;
			MaxInsight = maxInsight;
			StartingInsight = startingInsight;
			MoveSet = new List<MoveEntry>(moveSet).AsReadOnly();
		}
		/// <summary>
		/// The international dex number of this Pokémon
		/// </summary>
		public int DexNum { get; }
		/// <summary>
		/// Whether or not this Pokémon is recommended as a starter
		/// </summary>
		public bool SuggestedStarer { get; }
		/// <summary>
		/// The primary type of this Pokémon
		/// </summary>
		public ITypeDefinition PrimaryType { get; }
		/// <summary>
		/// The secondary type of this Pokémon if applicable
		/// </summary>
		public ITypeDefinition? SecondaryType { get; }
		/// <summary>
		/// The name of this Pokémon
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Regional variant of this Pokémon (if applicable) such as "Galaran"
		/// </summary>
		public string? Variant { get; }
		/// <summary>
		/// The average height of this Pokémon
		/// </summary>
		public Height AverageHeight { get; }
		/// <summary>
		/// The average weight of this Pokémon
		/// </summary>
		public Weight AverageWeight { get; }
		/// <summary>
		/// The descriptive category for this Pokémon
		/// </summary>
		public string Category { get; }
		/// <summary>
		/// The description of this Pokémon
		/// </summary>
		public string DexDescription { get; }
		/// <summary>
		/// Suggested starting rank of this Pokémon
		/// </summary>
		public Rank SuggestedRank { get; }
		/// <summary>
		/// The base hp of this Pokémon
		/// </summary>
		public int BaseHp { get; }
		/// <summary>
		/// Primary display image of this Pokémon
		/// </summary>
		public ImageRef PrimaryImage { get; }
		/// <summary>
		/// Smaller display image of this Pokémon
		/// </summary>
		public ImageRef SmallImage { get; }
		/// <summary>
		/// Primary display image of a shiny instance of this Pokémon
		/// </summary>
		public ImageRef? ShinyImage { get; }
		/// <summary>
		/// Smaller display image of a shiny instance of this Pokémon
		/// </summary>
		public ImageRef? SmallShinyImage { get; }
		/// <summary>
		/// List of possible abilities this Pokémon can have
		/// </summary>
		public IReadOnlyList<AbilityEntry> Abilities { get; }
		/// <summary>
		/// Evolution line of this Pokémon if applicable
		/// </summary>
		public ItemReference<EvolutionList>? EvolutionList { get; }
		/// <summary>
		/// If this is a mega evolution, then what it evolved from, otherwise null
		/// </summary>
		public ItemReference<DexEntry>? MegaEvolutionBaseEntry { get; }
		/// <summary>
		/// List of possible mega evolutions of this Pokémon and their required items if any
		/// </summary>
		public IReadOnlyList<MegaEvolutionEntry> MegaEvolutions { get; }
		/// <summary>
		/// The maximum strength score this Pokémon can have
		/// </summary>
		public int MaxStrength { get; }
		/// <summary>
		/// The initial strength score this Pokémon has
		/// </summary>
		public int StartingStrength { get; }
		/// <summary>
		/// The maximum dexterity score this Pokémon can have
		/// </summary>
		public int MaxDexterity { get; }
		/// <summary>
		/// The initial dexterity score this Pokémon has
		/// </summary>
		public int StartingDexterity { get; }
		/// <summary>
		/// The maximum vitality score this Pokémon can have
		/// </summary>
		public int MaxVitality { get; }
		/// <summary>
		/// The initial vitality score this Pokémon has
		/// </summary>
		public int StartingVitality { get; }
		/// <summary>
		/// The maximum special score this Pokémon can have
		/// </summary>
		public int MaxSpecial { get; }
		/// <summary>
		/// The initial special score this Pokémon has
		/// </summary>
		public int StartingSpecial { get; }
		/// <summary>
		/// The maximum insight score this Pokémon can have
		/// </summary>
		public int MaxInsight { get; }
		/// <summary>
		/// The initial insight score this Pokémon has
		/// </summary>
		public int StartingInsight { get; }
		/// <summary>
		/// List of moves that this Pokémon can learn
		/// </summary>
		public IReadOnlyList<MoveEntry> MoveSet { get; }
		public class Builder
		{
			public Builder() { }
			public Builder(DexEntry dexEntry)
			{
				DataId = dexEntry.DataId;
				DexNum = dexEntry.DexNum;
				SuggestedStarer = dexEntry.SuggestedStarer;
				PrimaryType = dexEntry.PrimaryType;
				SecondaryType = dexEntry.SecondaryType;
				Name = dexEntry.Name;
				Variant = dexEntry.Variant;
				AverageHeight = dexEntry.AverageHeight;
				AverageWeight = dexEntry.AverageWeight;
				Category = dexEntry.Category;
				DexDescription = dexEntry.DexDescription;
				SuggestedRank = dexEntry.SuggestedRank;
				BaseHp = dexEntry.BaseHp;
				PrimaryImage = dexEntry.PrimaryImage;
				SmallImage = dexEntry.SmallImage;
				ShinyImage = dexEntry.ShinyImage;
				SmallShinyImage = dexEntry.SmallShinyImage;
				Abilities = new List<AbilityEntry>(dexEntry.Abilities);
				EvolutionList = dexEntry.EvolutionList;
				MegaEvolutionBaseEntry = dexEntry.MegaEvolutionBaseEntry;
				MegaEvolutions = new List<MegaEvolutionEntry>(dexEntry.MegaEvolutions);
				MaxStrength = dexEntry.MaxStrength;
				StartingStrength = dexEntry.StartingStrength;
				MaxDexterity = dexEntry.MaxDexterity;
				StartingDexterity = dexEntry.StartingDexterity;
				MaxVitality = dexEntry.MaxVitality;
				StartingVitality = dexEntry.StartingVitality;
				MaxSpecial = dexEntry.MaxSpecial;
				StartingSpecial = dexEntry.StartingSpecial;
				MaxInsight = dexEntry.MaxInsight;
				StartingInsight = dexEntry.StartingInsight;
				MoveSet = new List<MoveEntry>(dexEntry.MoveSet);
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// The international dex number of this Pokémon
			/// </summary>
			public int? DexNum { get; set; }
			/// <summary>
			/// Whether or not this Pokémon is recommended as a starter
			/// </summary>
			public bool? SuggestedStarer { get; set; }
			/// <summary>
			/// The primary type of this Pokémon
			/// </summary>
			public ITypeDefinition? PrimaryType { get; set; }
			/// <summary>
			/// The secondary type of this Pokémon if applicable
			/// </summary>
			public ITypeDefinition? SecondaryType { get; set; }
			/// <summary>
			/// The name of this Pokémon
			/// </summary>
			public string? Name { get; set; }
			/// <summary>
			/// Regional variant of this Pokémon (if applicable) such as "Galaran"
			/// </summary>
			public string? Variant { get; set; }
			/// <summary>
			/// The average height of this Pokémon
			/// </summary>
			public Height? AverageHeight { get; set; }
			/// <summary>
			/// The average weight of this Pokémon
			/// </summary>
			public Weight? AverageWeight { get; set; }
			/// <summary>
			/// The descriptive category for this Pokémon
			/// </summary>
			public string? Category { get; set; }
			/// <summary>
			/// The description of this Pokémon
			/// </summary>
			public string? DexDescription { get; set; }
			/// <summary>
			/// Suggested starting rank of this Pokémon
			/// </summary>
			public Rank? SuggestedRank { get; set; }
			/// <summary>
			/// The base hp of this Pokémon
			/// </summary>
			public int? BaseHp { get; set; }
			/// <summary>
			/// Primary display image of this Pokémon
			/// </summary>
			public ImageRef? PrimaryImage { get; set; }
			/// <summary>
			/// Smaller display image of this Pokémon
			/// </summary>
			public ImageRef? SmallImage { get; set; }
			/// <summary>
			/// Primary display image of a shiny instance of this Pokémon
			/// </summary>
			public ImageRef? ShinyImage { get; set; }
			/// <summary>
			/// Smaller display image of a shiny instance of this Pokémon
			/// </summary>
			public ImageRef? SmallShinyImage { get; set; }
			/// <summary>
			/// List of possible abilities this Pokémon can have
			/// </summary>
			public IList<AbilityEntry>? Abilities { get; set; }
			/// <summary>
			/// Evolution line of this Pokémon if applicable
			/// </summary>
			public ItemReference<EvolutionList>? EvolutionList { get; set; }
			/// <summary>
			/// If this is a mega evolution, then what it evolved from, otherwise null
			/// </summary>
			public ItemReference<DexEntry>? MegaEvolutionBaseEntry { get; set; }
			/// <summary>
			/// List of possible mega evolutions of this Pokémon and their required items if any
			/// </summary>
			public IList<MegaEvolutionEntry>? MegaEvolutions { get; set; }
			/// <summary>
			/// The maximum strength score this Pokémon can have
			/// </summary>
			public int? MaxStrength { get; set; }
			/// <summary>
			/// The initial strength score this Pokémon has
			/// </summary>
			public int? StartingStrength { get; set; }
			/// <summary>
			/// The maximum dexterity score this Pokémon can have
			/// </summary>
			public int? MaxDexterity { get; set; }
			/// <summary>
			/// The initial dexterity score this Pokémon has
			/// </summary>
			public int? StartingDexterity { get; set; }
			/// <summary>
			/// The maximum vitality score this Pokémon can have
			/// </summary>
			public int? MaxVitality { get; set; }
			/// <summary>
			/// The initial vitality score this Pokémon has
			/// </summary>
			public int? StartingVitality { get; set; }
			/// <summary>
			/// The maximum special score this Pokémon can have
			/// </summary>
			public int? MaxSpecial { get; set; }
			/// <summary>
			/// The initial special score this Pokémon has
			/// </summary>
			public int? StartingSpecial { get; set; }
			/// <summary>
			/// The maximum insight score this Pokémon can have
			/// </summary>
			public int? MaxInsight { get; set; }
			/// <summary>
			/// The initial insight score this Pokémon has
			/// </summary>
			public int? StartingInsight { get; set; }
			/// <summary>
			/// List of moves that this Pokémon can learn
			/// </summary>
			public IList<MoveEntry>? MoveSet { get; set; }
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="DexEntry"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					if (DexNum is null)
					{
						return false;
					}
					if (SuggestedStarer is null)
					{
						return false;
					}
					if (PrimaryType is null)
					{
						return false;
					}
					if (Name is null)
					{
						return false;
					}
					if (AverageHeight is null)
					{
						return false;
					}
					if (AverageWeight is null)
					{
						return false;
					}
					if (Category is null)
					{
						return false;
					}
					if (DexDescription is null)
					{
						return false;
					}
					if (SuggestedRank is null)
					{
						return false;
					}
					if (BaseHp is null)
					{
						return false;
					}
					if (PrimaryImage is null)
					{
						return false;
					}
					if (SmallImage is null)
					{
						return false;
					}
					if (Abilities is null)
					{
						return false;
					}
					if (MegaEvolutions is null)
					{
						return false;
					}
					if (MaxStrength is null)
					{
						return false;
					}
					if (StartingStrength is null)
					{
						return false;
					}
					if (MaxDexterity is null)
					{
						return false;
					}
					if (StartingDexterity is null)
					{
						return false;
					}
					if (MaxVitality is null)
					{
						return false;
					}
					if (StartingVitality is null)
					{
						return false;
					}
					if (MaxSpecial is null)
					{
						return false;
					}
					if (StartingSpecial is null)
					{
						return false;
					}
					if (MaxInsight is null)
					{
						return false;
					}
					if (StartingInsight is null)
					{
						return false;
					}
					if (MoveSet is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="DexEntry"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="DexEntry"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public DexEntry Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new DexEntry(DataId!.Value,
					DexNum!.Value,
					SuggestedStarer!.Value,
					PrimaryType!,
					SecondaryType,
					Name!,
					Variant,
					AverageHeight!,
					AverageWeight!,
					Category!,
					DexDescription!,
					SuggestedRank!.Value,
					BaseHp!.Value,
					PrimaryImage!,
					SmallImage!,
					ShinyImage,
					SmallShinyImage,
					Abilities!,
					EvolutionList,
					MegaEvolutionBaseEntry,
					MegaEvolutions!,
					MaxStrength!.Value,
					StartingStrength!.Value,
					MaxDexterity!.Value,
					StartingDexterity!.Value,
					MaxVitality!.Value,
					StartingVitality!.Value,
					MaxSpecial!.Value,
					StartingSpecial!.Value,
					MaxInsight!.Value,
					StartingInsight!.Value,
					MoveSet!);
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record MonInstance : BaseDataItem
	{
		public MonInstance(DataId dataId,
			ImageRef picture,
			ItemReference<DexEntry> definition,
			string name,
			ItemReference<Ability> ability,
			ItemReference<Ability>? overiddenAblity,
			int hP,
			int willPoints,
			ItemReference<Item> heldItem,
			IList<MonStatus> status,
			int evasionDice,
			int clashDice,
			int defence,
			int specialDefence,
			Rank rank,
			IList<MoveEntry> moves,
			Height height,
			Weight weight,
			int strength,
			int dexterity,
			int vitality,
			int special,
			int insight,
			int brawl,
			int channel,
			int clash,
			int evasion,
			int alert,
			int athletic,
			int nature,
			int stealth,
			int allure,
			int etiquette,
			int intimidate,
			int perform,
			IDictionary<string, int> customSkills,
			int tough,
			int cool,
			int beauty,
			int clever,
			int cute,
			Nature monNature,
			int happiness,
			int loyalty,
			int battleCount,
			int vicoryCount,
			IList<string> accessories,
			IList<string> ribbons) : base(dataId)
		{
			Picture = picture;
			Definition = definition;
			Name = name;
			Ability = ability;
			OveriddenAblity = overiddenAblity;
			HP = hP;
			WillPoints = willPoints;
			HeldItem = heldItem;
			Status = new List<MonStatus>(status).AsReadOnly();
			EvasionDice = evasionDice;
			ClashDice = clashDice;
			Defence = defence;
			SpecialDefence = specialDefence;
			Rank = rank;
			Moves = new List<MoveEntry>(moves).AsReadOnly();
			Height = height;
			Weight = weight;
			Strength = strength;
			Dexterity = dexterity;
			Vitality = vitality;
			Special = special;
			Insight = insight;
			Brawl = brawl;
			Channel = channel;
			Clash = clash;
			Evasion = evasion;
			Alert = alert;
			Athletic = athletic;
			Nature = nature;
			Stealth = stealth;
			Allure = allure;
			Etiquette = etiquette;
			Intimidate = intimidate;
			Perform = perform;
			CustomSkills = new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(customSkills));
			Tough = tough;
			Cool = cool;
			Beauty = beauty;
			Clever = clever;
			Cute = cute;
			MonNature = monNature;
			Happiness = happiness;
			Loyalty = loyalty;
			BattleCount = battleCount;
			VicoryCount = vicoryCount;
			Accessories = new List<string>(accessories).AsReadOnly();
			Ribbons = new List<string>(ribbons).AsReadOnly();
		}
		/// <summary>
		/// Picture of this Pokémon
		/// </summary>
		public ImageRef Picture { get; }
		/// <summary>
		/// The DexEntry that currently defines this Pokémon
		/// </summary>
		public ItemReference<DexEntry> Definition { get; }
		/// <summary>
		/// The name of this Pokémon
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// This Pokémon's usual ability
		/// </summary>
		public ItemReference<Ability> Ability { get; }
		/// <summary>
		/// This Pokémon's current ability if it isn't the usual ability, such as what happens when one gets hit by simple beam
		/// </summary>
		public ItemReference<Ability>? OveriddenAblity { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int HP { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int WillPoints { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public ItemReference<Item> HeldItem { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public IReadOnlyList<MonStatus> Status { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int EvasionDice { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int ClashDice { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Defence { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int SpecialDefence { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public Rank Rank { get; }
		/// <summary>
		/// List of moves this Pokémon knows
		/// </summary>
		public IReadOnlyList<MoveEntry> Moves { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public Height Height { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public Weight Weight { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Strength { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Dexterity { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Vitality { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Special { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Insight { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Brawl { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Channel { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Clash { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Evasion { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Alert { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Athletic { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Nature { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Stealth { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Allure { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Etiquette { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Intimidate { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Perform { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public IReadOnlyDictionary<string, int> CustomSkills { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Tough { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Cool { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Beauty { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Clever { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Cute { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public Nature MonNature { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Happiness { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int Loyalty { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int BattleCount { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public int VicoryCount { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public IReadOnlyList<string> Accessories { get; }
		/// <summary>
		/// Someone didn't document this item...
		/// </summary>
		public IReadOnlyList<string> Ribbons { get; }
		public class Builder
		{
			public Builder() { }
			public Builder(MonInstance monInstance)
			{
				DataId = monInstance.DataId;
				Picture = monInstance.Picture;
				Definition = monInstance.Definition;
				Name = monInstance.Name;
				Ability = monInstance.Ability;
				OveriddenAblity = monInstance.OveriddenAblity;
				HP = monInstance.HP;
				WillPoints = monInstance.WillPoints;
				HeldItem = monInstance.HeldItem;
				Status = new List<MonStatus>(monInstance.Status);
				EvasionDice = monInstance.EvasionDice;
				ClashDice = monInstance.ClashDice;
				Defence = monInstance.Defence;
				SpecialDefence = monInstance.SpecialDefence;
				Rank = monInstance.Rank;
				Moves = new List<MoveEntry>(monInstance.Moves);
				Height = monInstance.Height;
				Weight = monInstance.Weight;
				Strength = monInstance.Strength;
				Dexterity = monInstance.Dexterity;
				Vitality = monInstance.Vitality;
				Special = monInstance.Special;
				Insight = monInstance.Insight;
				Brawl = monInstance.Brawl;
				Channel = monInstance.Channel;
				Clash = monInstance.Clash;
				Evasion = monInstance.Evasion;
				Alert = monInstance.Alert;
				Athletic = monInstance.Athletic;
				Nature = monInstance.Nature;
				Stealth = monInstance.Stealth;
				Allure = monInstance.Allure;
				Etiquette = monInstance.Etiquette;
				Intimidate = monInstance.Intimidate;
				Perform = monInstance.Perform;
				CustomSkills = new Dictionary<string, int>(monInstance.CustomSkills);
				Tough = monInstance.Tough;
				Cool = monInstance.Cool;
				Beauty = monInstance.Beauty;
				Clever = monInstance.Clever;
				Cute = monInstance.Cute;
				MonNature = monInstance.MonNature;
				Happiness = monInstance.Happiness;
				Loyalty = monInstance.Loyalty;
				BattleCount = monInstance.BattleCount;
				VicoryCount = monInstance.VicoryCount;
				Accessories = new List<string>(monInstance.Accessories);
				Ribbons = new List<string>(monInstance.Ribbons);
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// Picture of this Pokémon
			/// </summary>
			public ImageRef? Picture { get; set; }
			/// <summary>
			/// The DexEntry that currently defines this Pokémon
			/// </summary>
			public ItemReference<DexEntry>? Definition { get; set; }
			/// <summary>
			/// The name of this Pokémon
			/// </summary>
			public string? Name { get; set; }
			/// <summary>
			/// This Pokémon's usual ability
			/// </summary>
			public ItemReference<Ability>? Ability { get; set; }
			/// <summary>
			/// This Pokémon's current ability if it isn't the usual ability, such as what happens when one gets hit by simple beam
			/// </summary>
			public ItemReference<Ability>? OveriddenAblity { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? HP { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? WillPoints { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public ItemReference<Item>? HeldItem { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public IList<MonStatus>? Status { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? EvasionDice { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? ClashDice { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Defence { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? SpecialDefence { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public Rank? Rank { get; set; }
			/// <summary>
			/// List of moves this Pokémon knows
			/// </summary>
			public IList<MoveEntry>? Moves { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public Height? Height { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public Weight? Weight { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Strength { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Dexterity { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Vitality { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Special { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Insight { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Brawl { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Channel { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Clash { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Evasion { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Alert { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Athletic { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Nature { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Stealth { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Allure { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Etiquette { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Intimidate { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Perform { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public IDictionary<string, int>? CustomSkills { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Tough { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Cool { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Beauty { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Clever { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Cute { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public Nature? MonNature { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Happiness { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? Loyalty { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? BattleCount { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public int? VicoryCount { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public IList<string>? Accessories { get; set; }
			/// <summary>
			/// Someone didn't document this item...
			/// </summary>
			public IList<string>? Ribbons { get; set; }
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="MonInstance"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					if (Picture is null)
					{
						return false;
					}
					if (Definition is null)
					{
						return false;
					}
					if (Name is null)
					{
						return false;
					}
					if (Ability is null)
					{
						return false;
					}
					if (HP is null)
					{
						return false;
					}
					if (WillPoints is null)
					{
						return false;
					}
					if (HeldItem is null)
					{
						return false;
					}
					if (Status is null)
					{
						return false;
					}
					if (EvasionDice is null)
					{
						return false;
					}
					if (ClashDice is null)
					{
						return false;
					}
					if (Defence is null)
					{
						return false;
					}
					if (SpecialDefence is null)
					{
						return false;
					}
					if (Rank is null)
					{
						return false;
					}
					if (Moves is null)
					{
						return false;
					}
					if (Height is null)
					{
						return false;
					}
					if (Weight is null)
					{
						return false;
					}
					if (Strength is null)
					{
						return false;
					}
					if (Dexterity is null)
					{
						return false;
					}
					if (Vitality is null)
					{
						return false;
					}
					if (Special is null)
					{
						return false;
					}
					if (Insight is null)
					{
						return false;
					}
					if (Brawl is null)
					{
						return false;
					}
					if (Channel is null)
					{
						return false;
					}
					if (Clash is null)
					{
						return false;
					}
					if (Evasion is null)
					{
						return false;
					}
					if (Alert is null)
					{
						return false;
					}
					if (Athletic is null)
					{
						return false;
					}
					if (Nature is null)
					{
						return false;
					}
					if (Stealth is null)
					{
						return false;
					}
					if (Allure is null)
					{
						return false;
					}
					if (Etiquette is null)
					{
						return false;
					}
					if (Intimidate is null)
					{
						return false;
					}
					if (Perform is null)
					{
						return false;
					}
					if (CustomSkills is null)
					{
						return false;
					}
					if (Tough is null)
					{
						return false;
					}
					if (Cool is null)
					{
						return false;
					}
					if (Beauty is null)
					{
						return false;
					}
					if (Clever is null)
					{
						return false;
					}
					if (Cute is null)
					{
						return false;
					}
					if (MonNature is null)
					{
						return false;
					}
					if (Happiness is null)
					{
						return false;
					}
					if (Loyalty is null)
					{
						return false;
					}
					if (BattleCount is null)
					{
						return false;
					}
					if (VicoryCount is null)
					{
						return false;
					}
					if (Accessories is null)
					{
						return false;
					}
					if (Ribbons is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="MonInstance"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="MonInstance"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public MonInstance Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new MonInstance(DataId!.Value,
					Picture!,
					Definition!,
					Name!,
					Ability!,
					OveriddenAblity,
					HP!.Value,
					WillPoints!.Value,
					HeldItem!,
					Status!,
					EvasionDice!.Value,
					ClashDice!.Value,
					Defence!.Value,
					SpecialDefence!.Value,
					Rank!.Value,
					Moves!,
					Height!,
					Weight!,
					Strength!.Value,
					Dexterity!.Value,
					Vitality!.Value,
					Special!.Value,
					Insight!.Value,
					Brawl!.Value,
					Channel!.Value,
					Clash!.Value,
					Evasion!.Value,
					Alert!.Value,
					Athletic!.Value,
					Nature!.Value,
					Stealth!.Value,
					Allure!.Value,
					Etiquette!.Value,
					Intimidate!.Value,
					Perform!.Value,
					CustomSkills!,
					Tough!.Value,
					Cool!.Value,
					Beauty!.Value,
					Clever!.Value,
					Cute!.Value,
					MonNature!.Value,
					Happiness!.Value,
					Loyalty!.Value,
					BattleCount!.Value,
					VicoryCount!.Value,
					Accessories!,
					Ribbons!);
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record Ability : BaseDataItem
	{
		public Ability(DataId dataId) : base(dataId)
		{
		}
		public class Builder
		{
			public Builder() { }
			public Builder(Ability ability)
			{
				DataId = ability.DataId;
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="Ability"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="Ability"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="Ability"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public Ability Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new Ability(DataId!.Value);
			}
		}
	}
	[System.CodeDom.Compiler.GeneratedCode("BaseTypeBuilder.tt", "??")]
	public partial record EvolutionList : BaseDataItem
	{
		public EvolutionList(DataId dataId) : base(dataId)
		{
		}
		public class Builder
		{
			public Builder() { }
			public Builder(EvolutionList evolutionList)
			{
				DataId = evolutionList.DataId;
			}
			DataId? DataId {get; set;}
			/// <summary>
			/// Whether or not all of the required Properites of this instance are set to build a new
			/// <see cref="EvolutionList"/>. <see cref="Build"/> will throw an exception if this returns false.
			/// </summary>
			public bool IsValid
			{
				get
				{
					if (DataId is null)
					{
						return false;
					}
					return true;
				}
			}
			/// <summary>
			/// Build and instance of <see cref="EvolutionList"/> from this Builder
			/// </summary>
			/// <returns>A new instance of <see cref="EvolutionList"/></returns>
			/// <exception cref="InvalidOperationException">If this method is called when not all required properties
			/// have been set</exception>
			public EvolutionList Build(){
				if (!IsValid)
				{
					throw new InvalidOperationException("Not all required fields were set");
				}
				return new EvolutionList(DataId!.Value);
			}
		}
	}
}
