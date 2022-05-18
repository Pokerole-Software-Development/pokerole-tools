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

	public static class StatManager
	{
		private static bool initted = false;
		private static readonly Object initLock = new object();
		private static readonly List<Stat> statList = new List<Stat>();
		private static readonly IReadOnlyList<Stat> readonlyStats = statList.AsReadOnly();
		private static readonly Dictionary<BuiltInStat, BuiltInStatImpl> builtInStatImplementations =
			new Dictionary<BuiltInStat, BuiltInStatImpl>(30);
		private static readonly IReadOnlyDictionary<BuiltInStat, Guid> baseTypeGuids =
			new ReadOnlyDictionary<BuiltInStat, Guid>(new Dictionary<BuiltInStat, Guid>
			{
#region Stats
				{ BuiltInStat.Strength, Guid.Parse("aee02cc8-60ee-4916-8320-282a307103ba") },
				{ BuiltInStat.Dexterity, Guid.Parse("e4f0fbba-eeea-45e9-9e1e-24c68d3653d5") },
				{ BuiltInStat.Vitality, Guid.Parse("36800f3e-de5f-4316-bfad-427569707974") },
				{ BuiltInStat.Special, Guid.Parse("ef3c7e05-d222-489c-9898-f063773787af") },
				{ BuiltInStat.Insight, Guid.Parse("93ee2cee-e910-401d-bfb1-41c7458b5872") },
				{ BuiltInStat.Tough, Guid.Parse("c55d3235-baa5-4345-8865-824ef09ca063") },
				{ BuiltInStat.Cool, Guid.Parse("2c7f0ce1-2dc4-4400-a538-63e7f0beaddb") },
				{ BuiltInStat.Beauty, Guid.Parse("7a873215-4d94-4e17-8a99-de852a5159db") },
				{ BuiltInStat.Cute, Guid.Parse("25035da9-c8a2-4bf2-a9ce-e25356f03877") },
				{ BuiltInStat.Clever, Guid.Parse("c86835a3-f974-40e6-959d-150f6e54f88e") },
				{ BuiltInStat.Brawl, Guid.Parse("56fd77c7-0aeb-450e-b94e-60ec0a698e04") },
				{ BuiltInStat.Channel, Guid.Parse("643ef7b7-fe3f-451a-ae8b-263c137c188e") },
				{ BuiltInStat.Clash, Guid.Parse("4306fa61-44cf-4655-b0a4-b572a4e7abfa") },
				{ BuiltInStat.Evasion, Guid.Parse("18874376-3dc1-4a3e-9ce4-4f46045b106b") },
				{ BuiltInStat.Throw, Guid.Parse("7213861f-92c4-4e8f-b8f5-6d24261cddce") },
				{ BuiltInStat.Weapons, Guid.Parse("4dc76c9d-7ebc-4822-bb9c-c8d0a098f84c") },
				{ BuiltInStat.Alert, Guid.Parse("1603f97f-a8f2-4f3c-8358-406641a61959") },
				{ BuiltInStat.Athletic, Guid.Parse("bee1700a-966e-4164-bc78-dc8ac23ff9e3") },
				{ BuiltInStat.Nature, Guid.Parse("c953af2a-e106-4b55-aeb8-6e8644b7ab70") },
				{ BuiltInStat.Stealth, Guid.Parse("bf82a4f2-45ce-45df-ad24-afeb691d1378") },
				{ BuiltInStat.Allure, Guid.Parse("157395de-ae48-43f7-8581-dc6159a161ac") },
				{ BuiltInStat.Empathy, Guid.Parse("861971d9-7707-4a4c-8117-4d5e42c16289") },
				{ BuiltInStat.Etiquette, Guid.Parse("84d5d104-b042-4df0-b910-4ddfddc9ed32") },
				{ BuiltInStat.Intimidate, Guid.Parse("c2edd061-c645-4f45-83df-29a92c3a101c") },
				{ BuiltInStat.Perform, Guid.Parse("7ec5ccfd-cc2f-4b39-8d69-dca877d65ec9") },
				{ BuiltInStat.Crafts, Guid.Parse("f76ae46a-d152-4df2-8616-a301ce873823") },
				{ BuiltInStat.Lore, Guid.Parse("2800112c-8a65-4ef1-b092-5a668c3bf861") },
				{ BuiltInStat.Medicine, Guid.Parse("404911a0-8d74-4b1b-918c-16a938a38349") },
				{ BuiltInStat.Science, Guid.Parse("5717accb-d70e-43ea-9a26-fd8da623c497") },
				{ BuiltInStat.Happiness, Guid.Parse("9d882c06-536a-48ae-b8d4-68cf2175e889") },
				{ BuiltInStat.Loyalty, Guid.Parse("e44f6385-7cfc-4169-98ee-499ab945febd") }, 
				{ BuiltInStat.Will, Guid.Parse("82fd7a85-126a-4f73-962e-af0a62edf97d") }, 
				{ BuiltInStat.None, Guid.Parse("6de5dac7-0b13-4c58-9f6a-892cc71a580c") },
				{ BuiltInStat.Varies, Guid.Parse("f5c6817e-2dc4-4494-8dfb-aeddef0fd80c") },
				{ BuiltInStat.SameAsTheCopiedMove, Guid.Parse("d6557cfa-c0d5-406c-a8f3-d492a2fd5ef3") },
	#endregion
			});
		public static IReadOnlyList<Stat> RegisteredStats
		{
			get
			{
				CheckInit();
				return readonlyStats;
			}
		}
		public static Stat GetBuiltInStat(BuiltInStat stat)
		{
			CheckInit();
			if (!builtInStatImplementations.TryGetValue(stat, out BuiltInStatImpl? item))
			{
				throw new ArgumentException($"'{stat}' is not a valid built-in stat or has not been registered");
			}
			return item;
		}

		public static IStatBuilder CreateStatBuilder()
		{
			throw new NotImplementedException("Support for custom stats is not implemented yet");
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
			statList.Clear();
			builtInStatImplementations.Clear();
			BuiltInStat[] builtInStats = (BuiltInStat[])Enum.GetValues(typeof(BuiltInStat));
			if (statList.Capacity < builtInStats.Length)
			{
				statList.Capacity = builtInStats.Length;
			}
			foreach (var stat in builtInStats)
			{
				var instance = ConstructStatInstance(stat);
				builtInStatImplementations[stat] = instance;
				statList.Add(instance);
			}
			initted = true;
		}

		private static BuiltInStatImpl ConstructStatInstance(BuiltInStat stat)
		{
			var exclusivity = stat switch
			{
				BuiltInStat.Special or BuiltInStat.Channel or BuiltInStat.Happiness or BuiltInStat.Loyalty =>
					StatExclusivity.Pokemon,
				BuiltInStat.Throw or BuiltInStat.Weapons or BuiltInStat.Crafts or BuiltInStat.Lore or
					BuiltInStat.Medicine or BuiltInStat.Science => StatExclusivity.Trainer,
				_ => StatExclusivity.None,
			};
			var category = stat switch
			{
				BuiltInStat.Strength or BuiltInStat.Dexterity or BuiltInStat.Vitality or BuiltInStat.Special or
					BuiltInStat.Insight => StatCategory.Primary,
				BuiltInStat.Tough or BuiltInStat.Cool or BuiltInStat.Beauty or BuiltInStat.Cute or
					BuiltInStat.Clever => StatCategory.SocialAttribute,
				BuiltInStat.Brawl or BuiltInStat.Channel or BuiltInStat.Clash or BuiltInStat.Evasion or
					BuiltInStat.Throw or BuiltInStat.Weapons => StatCategory.Fight,
				BuiltInStat.Alert or BuiltInStat.Athletic or BuiltInStat.Nature or BuiltInStat.Stealth =>
					StatCategory.Survival,
				BuiltInStat.Allure or BuiltInStat.Empathy or BuiltInStat.Etiquette or BuiltInStat.Intimidate or
					BuiltInStat.Perform => StatCategory.Contest,
				BuiltInStat.Crafts or BuiltInStat.Lore or BuiltInStat.Medicine or BuiltInStat.Science =>
					StatCategory.Knowledge,
				BuiltInStat.Happiness or BuiltInStat.Loyalty => StatCategory.HappinesOrLoyalty,
				BuiltInStat.Will or BuiltInStat.None or BuiltInStat.Varies or BuiltInStat.SameAsTheCopiedMove => 
					StatCategory.Other,
				_ => throw new InvalidOperationException($"Unknown base stat: {stat}"),
			};
			return new BuiltInStatImpl(stat, exclusivity, category);
		}

		private record BuiltInStatImpl : Stat
		{
			internal BuiltInStatImpl(BuiltInStat stat, StatExclusivity exclusivity, StatCategory category)
				: base(new DataId((int)stat, baseTypeGuids[stat]), stat.ToString(), exclusivity, category)
			{
			}
			public override bool IsBuiltIn => true;
		}
		private abstract class StatImpl : IStat
		{
			private readonly StatCategory category;
			private readonly StatExclusivity exclusivity;
			protected StatImpl(StatExclusivity exclusivity, StatCategory category)
			{
				this.category = category;
				this.exclusivity = exclusivity;
			}
			public abstract DataId DataId { get; }
			public abstract string Name { get; }
			public abstract bool IsBuiltInStat { get; }
			public StatCategory StatCategory => category;
			public StatExclusivity StatExclusivity => exclusivity;
			public ItemReference<IStat> ItemReference => new ItemReference<IStat>(DataId, Name);

			public (string, object?)[] Values => new(String, object?)[]{
				(nameof(DataId), DataId),
				(nameof(Name), Name),
				(nameof(IsBuiltInStat), IsBuiltInStat),
				(nameof(StatCategory), StatCategory),
				(nameof(StatExclusivity), StatExclusivity)
			};

			public DataKind Kind => DataKind.Stat;

			public bool Mutable => false;

			public bool HasBuilder => false;
		}
	}

	public interface IStatBuilder
	{
	}

	public interface IStat : IDataItem<IStat>
	{
		String Name { get; }
		bool IsBuiltInStat { get; }
		StatCategory StatCategory { get; }
		StatExclusivity StatExclusivity { get; }
		//ItemReference<IStat> ItemReference { get; }
	}
	public enum StatExclusivity
	{
		None,
		Pokemon,
		Trainer
	}
	public enum StatCategory
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
	public enum BuiltInStat
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
		//Contest Stats
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
		Varies,
		None,
		SameAsTheCopiedMove

	}
}
