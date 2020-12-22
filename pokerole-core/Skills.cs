/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Text;

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
		}

		private static ISkill ConstructSkillInstance(BuiltInSkill skill)
		{

			throw new NotImplementedException();
		}
		private abstract class SkillImpl : ISkill
		{

		}
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
		PrimarySocial,
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
