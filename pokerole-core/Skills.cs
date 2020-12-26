/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pokerole.Core
{

	public static class SkillManager
	{
		private static bool initted = false;
		private static readonly Object initLock = new object();
		private static readonly List<ISkill> skillList = new List<ISkill>();
		private static readonly IReadOnlyList<ISkill> readonlySkills = skillList.AsReadOnly();
		private static readonly Dictionary<BuiltInSkill, ISkill> builtInSkillImplementations =
			new Dictionary<BuiltInSkill, ISkill>(30);

		public static IReadOnlyList<ISkill> RegisteredSkills
		{
			get
			{
				CheckInit();
				return readonlySkills;
			}
		}
		public static ISkill GetBuiltInSkill(BuiltInSkill skill)
		{
			CheckInit();
			if (!builtInSkillImplementations.TryGetValue(skill, out ISkill? item))
			{
				throw new ArgumentException($"'{skill}' is not a valid built-in skill or has not been registered");
			}
			return item;
		}

		public static ISkillBuilder CreateSkillBuilder()
		{
			throw new NotImplementedException("Support for custom skills is not implemented yet");
		}

		private static void CheckInit()
		{
			if (!initted)
			{
				lock (initLock)
				{
					if (!initted)
					{
						Init();
					}
				}
			}
		}

		private static void Init()
		{
			//just in case....
			skillList.Clear();
			builtInSkillImplementations.Clear();
			BuiltInSkill[] builtInSkills = (BuiltInSkill[])Enum.GetValues(typeof(BuiltInSkill));
			if (skillList.Capacity < builtInSkills.Length)
			{
				skillList.Capacity = builtInSkills.Length;
			}
			foreach (var skill in builtInSkills)
			{
				var instance = ConstructSkillInstance(skill);
				builtInSkillImplementations[skill] = instance;
				skillList.Add(instance);
			}
			initted = true;
		}

		private static BuiltInSkillImpl ConstructSkillInstance(BuiltInSkill skill)
		{
			var exclusivity = skill switch
			{
				BuiltInSkill.Special or BuiltInSkill.Channel or BuiltInSkill.Happiness or BuiltInSkill.Loyalty =>
					SkillExclusivity.Pokemon,
				BuiltInSkill.Throw or BuiltInSkill.Weapons or BuiltInSkill.Crafts or BuiltInSkill.Lore or
					BuiltInSkill.Medicine or BuiltInSkill.Science => SkillExclusivity.Trainer,
				_ => SkillExclusivity.None,
			};
			var category = skill switch
			{
				BuiltInSkill.Strength or BuiltInSkill.Dexterity or BuiltInSkill.Vitality or BuiltInSkill.Special or
					BuiltInSkill.Insight => SkillCategory.Primary,
				BuiltInSkill.Tough or BuiltInSkill.Cool or BuiltInSkill.Beauty or BuiltInSkill.Cute or
					BuiltInSkill.Clever => SkillCategory.SocialAttribute,
				BuiltInSkill.Brawl or BuiltInSkill.Channel or BuiltInSkill.Clash or BuiltInSkill.Evasion or
					BuiltInSkill.Throw or BuiltInSkill.Weapons => SkillCategory.Fight,
				BuiltInSkill.Alert or BuiltInSkill.Athletic or BuiltInSkill.Nature or BuiltInSkill.Stealth =>
					SkillCategory.Survival,
				BuiltInSkill.Allure or BuiltInSkill.Etiquette or BuiltInSkill.Intimidate or BuiltInSkill.Perform =>
					SkillCategory.Social,
				BuiltInSkill.Crafts or BuiltInSkill.Lore or BuiltInSkill.Medicine or BuiltInSkill.Science =>
					SkillCategory.Knowledge,
				BuiltInSkill.Happiness or BuiltInSkill.Loyalty => SkillCategory.HappinesOrLoyalty,
				_ => throw new InvalidOperationException($"Unknown base skill: {skill}"),
			};
			return new BuiltInSkillImpl(skill, exclusivity, category);
		}
		private class BuiltInSkillImpl : SkillImpl
		{
			private readonly BuiltInSkill skill;
			internal BuiltInSkillImpl(BuiltInSkill skill, SkillExclusivity exclusivity, SkillCategory category)
				: base(exclusivity, category)
			{
				this.skill = skill;
			}
			public override int Id => (int)skill;
			public override bool IsBuiltInSkill => true;
			public override string Name => skill.ToString();
		}
		private abstract class SkillImpl : ISkill
		{
			private readonly SkillCategory category;
			private readonly SkillExclusivity exclusivity;
			protected SkillImpl(SkillExclusivity exclusivity, SkillCategory category)
			{
				this.category = category;
				this.exclusivity = exclusivity;
			}
			public abstract int Id { get; }
			public abstract string Name { get; }
			public abstract bool IsBuiltInSkill { get; }
			public SkillCategory SkillCategory => category;
			public SkillExclusivity SkillExclusivity => exclusivity;
		}
	}

	public interface ISkillBuilder
	{
	}

	public interface ISkill
	{
		int Id { get; }
		String Name { get; }
		bool IsBuiltInSkill { get; }
		SkillCategory SkillCategory { get; }
		SkillExclusivity SkillExclusivity { get; }
	}
	public enum SkillExclusivity
	{
		None,
		Pokemon,
		Trainer
	}
	public enum SkillCategory
	{
		Primary,
		//Cool, Cute, etc.
		SocialAttribute,
		Fight,
		Survival,
		Social,
		Knowledge,
		HappinesOrLoyalty,
		Extra
	}
	public enum BuiltInSkill
	{
		//primary attributes
		Strength,
		Dexterity,
		Vitality,
		Special,//pokemon only
		Insight,
		//Primary Social
		Tough,
		Cool,
		Beauty,
		Cute,
		Clever,
		//Fight
		Brawl,
		Channel,//p
		Clash,
		Evasion,
		Throw,//t
		Weapons,//t
		//Survival
		Alert,
		Athletic,
		Nature,
		Stealth,
		//Social
		Allure,
		Etiquette,
		Intimidate,
		Perform,
		//Knowledge
		Crafts,
		Lore,
		Medicine,
		Science,
		//Other?
		Happiness,
		Loyalty

	}
}
