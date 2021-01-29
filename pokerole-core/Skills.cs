/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private static readonly IReadOnlyDictionary<BuiltInSkill, Guid> baseTypeGuids =
			new ReadOnlyDictionary<BuiltInSkill, Guid>(new Dictionary<BuiltInSkill, Guid>
			{
#region Skills
				{ BuiltInSkill.Strength, Guid.Parse("aee02cc8-60ee-4916-8320-282a307103ba") },
				{ BuiltInSkill.Dexterity, Guid.Parse("e4f0fbba-eeea-45e9-9e1e-24c68d3653d5") },
				{ BuiltInSkill.Vitality, Guid.Parse("36800f3e-de5f-4316-bfad-427569707974") },
				{ BuiltInSkill.Special, Guid.Parse("ef3c7e05-d222-489c-9898-f063773787af") },
				{ BuiltInSkill.Insight, Guid.Parse("93ee2cee-e910-401d-bfb1-41c7458b5872") },
				{ BuiltInSkill.Tough, Guid.Parse("c55d3235-baa5-4345-8865-824ef09ca063") },
				{ BuiltInSkill.Cool, Guid.Parse("2c7f0ce1-2dc4-4400-a538-63e7f0beaddb") },
				{ BuiltInSkill.Beauty, Guid.Parse("7a873215-4d94-4e17-8a99-de852a5159db") },
				{ BuiltInSkill.Cute, Guid.Parse("25035da9-c8a2-4bf2-a9ce-e25356f03877") },
				{ BuiltInSkill.Clever, Guid.Parse("c86835a3-f974-40e6-959d-150f6e54f88e") },
				{ BuiltInSkill.Brawl, Guid.Parse("56fd77c7-0aeb-450e-b94e-60ec0a698e04") },
				{ BuiltInSkill.Channel, Guid.Parse("643ef7b7-fe3f-451a-ae8b-263c137c188e") },
				{ BuiltInSkill.Clash, Guid.Parse("4306fa61-44cf-4655-b0a4-b572a4e7abfa") },
				{ BuiltInSkill.Evasion, Guid.Parse("18874376-3dc1-4a3e-9ce4-4f46045b106b") },
				{ BuiltInSkill.Throw, Guid.Parse("7213861f-92c4-4e8f-b8f5-6d24261cddce") },
				{ BuiltInSkill.Weapons, Guid.Parse("4dc76c9d-7ebc-4822-bb9c-c8d0a098f84c") },
				{ BuiltInSkill.Alert, Guid.Parse("1603f97f-a8f2-4f3c-8358-406641a61959") },
				{ BuiltInSkill.Athletic, Guid.Parse("bee1700a-966e-4164-bc78-dc8ac23ff9e3") },
				{ BuiltInSkill.Nature, Guid.Parse("c953af2a-e106-4b55-aeb8-6e8644b7ab70") },
				{ BuiltInSkill.Stealth, Guid.Parse("bf82a4f2-45ce-45df-ad24-afeb691d1378") },
				{ BuiltInSkill.Allure, Guid.Parse("157395de-ae48-43f7-8581-dc6159a161ac") },
				{ BuiltInSkill.Empathy, Guid.Parse("861971d9-7707-4a4c-8117-4d5e42c16289") },
				{ BuiltInSkill.Etiquette, Guid.Parse("84d5d104-b042-4df0-b910-4ddfddc9ed32") },
				{ BuiltInSkill.Intimidate, Guid.Parse("c2edd061-c645-4f45-83df-29a92c3a101c") },
				{ BuiltInSkill.Perform, Guid.Parse("7ec5ccfd-cc2f-4b39-8d69-dca877d65ec9") },
				{ BuiltInSkill.Crafts, Guid.Parse("f76ae46a-d152-4df2-8616-a301ce873823") },
				{ BuiltInSkill.Lore, Guid.Parse("2800112c-8a65-4ef1-b092-5a668c3bf861") },
				{ BuiltInSkill.Medicine, Guid.Parse("404911a0-8d74-4b1b-918c-16a938a38349") },
				{ BuiltInSkill.Science, Guid.Parse("5717accb-d70e-43ea-9a26-fd8da623c497") },
				{ BuiltInSkill.Happiness, Guid.Parse("9d882c06-536a-48ae-b8d4-68cf2175e889") },
				{ BuiltInSkill.Loyalty, Guid.Parse("e44f6385-7cfc-4169-98ee-499ab945febd") }, 
				{ BuiltInSkill.Will, Guid.Parse("82fd7a85-126a-4f73-962e-af0a62edf97d") }, 
				{ BuiltInSkill.None, Guid.Parse("6de5dac7-0b13-4c58-9f6a-892cc71a580c") }, 
	#endregion
			});
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
				BuiltInSkill.Allure or BuiltInSkill.Empathy or BuiltInSkill.Etiquette or BuiltInSkill.Intimidate or
					BuiltInSkill.Perform => SkillCategory.Contest,
				BuiltInSkill.Crafts or BuiltInSkill.Lore or BuiltInSkill.Medicine or BuiltInSkill.Science =>
					SkillCategory.Knowledge,
				BuiltInSkill.Happiness or BuiltInSkill.Loyalty => SkillCategory.HappinesOrLoyalty,
				BuiltInSkill.Will or BuiltInSkill.None => SkillCategory.Other,
				_ => throw new InvalidOperationException($"Unknown base skill: {skill}"),
			};
			return new BuiltInSkillImpl(skill, exclusivity, category);
		}

		private class BuiltInSkillImpl : SkillImpl
		{
			private readonly BuiltInSkill skill;
			private readonly DataId dataId;
			internal BuiltInSkillImpl(BuiltInSkill skill, SkillExclusivity exclusivity, SkillCategory category)
				: base(exclusivity, category)
			{
				this.skill = skill;
				dataId = new DataId((int)skill, baseTypeGuids[skill]);
			}
			public override DataId DataId => dataId;
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
			public abstract DataId DataId { get; }
			public abstract string Name { get; }
			public abstract bool IsBuiltInSkill { get; }
			public SkillCategory SkillCategory => category;
			public SkillExclusivity SkillExclusivity => exclusivity;
		}
	}

	public interface ISkillBuilder
	{
	}

	public interface ISkill : IDataItem
	{
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
		Contest,
		Knowledge,
		HappinesOrLoyalty,
		Other,
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
		//Contest Skills
		Allure,
		Empathy,//apparently this exists but doesn't?
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
		Loyalty,
		Will,
		None

	}
}
