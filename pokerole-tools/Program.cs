using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Pokerole.Core;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;

namespace Pokerole.Tools
{
	class Program
	{
		private const String csvFetchUrl = "https://raw.githubusercontent.com/XShadeSlayerXx/PokeRole-Discord.py-Base/master/";
		private static readonly HashSet<Guid> unevolvedEntries = new HashSet<Guid>();
		private static readonly HashSet<Guid> hasMegaEvolution = new HashSet<Guid>();
		static void Main(string[] args)
		{
			PokeroleXmlData data = new PokeroleXmlData();
			//read them datas!!!
			var movesByName = ReadMoves(data);
			var monByName = ReadDexEntries(data);
			ReadAbilities(data);
			ReadMoveLists(data, monByName, movesByName);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PokeroleXmlData));

			StringWriter writer = new StringWriter();
			xmlSerializer.Serialize(writer, data);
			File.WriteAllText("output.xml", writer.ToString());
			Console.WriteLine(writer.ToString());

			//for the sake of testing
			PokeroleXmlData data2 = (PokeroleXmlData)xmlSerializer.Deserialize(new StringReader(writer.ToString()));

			//Console.WriteLine("Hello World!");
		}

		private static String FetchFileIfNeeded(String file)
		{
			String path = Path.Combine(Directory.GetCurrentDirectory(), file);
			if (!File.Exists(path))
			{
				//download it!
				using var client = new WebClient();
				client.DownloadFile(csvFetchUrl + file, path);
			}
			return path;
		}
		private static T ParseEnum<T>(string val) where T : Enum{
			return (T)Enum.Parse(typeof(T), val.Replace(" ", ""), true);
		}
		private static ItemReference<ITypeDefinition>? ReadType(string val)
		{
			if (String.IsNullOrEmpty(val))
			{
				return null;
			}
			BuiltInType type;
			if (val == "any")
			{
				//"Any Move" support
				type = BuiltInType.Normal;
			}
			else
			{
				type = ParseEnum<BuiltInType>(val);
			}
			ITypeDefinition def = TypeManager.GetBuiltInType(type);
			return new ItemReference<ITypeDefinition>(def.DataId, def.Name);

		}
		private static int? ReadInt(String val)
		{
			if (String.IsNullOrEmpty(val))
			{
				return null;
			}
			return int.Parse(val);
		}
		private static AbilityEntry? ReadAbility(String val, bool hidden)
		{
			if (String.IsNullOrEmpty(val))
			{
				return null;
			}
			ItemReference<Ability> reference = new ItemReference<Ability>(default, val);
			return new AbilityEntry(hidden, reference);
		}
		private static Dictionary<String, Move.Builder> ReadMoves(PokeroleXmlData data)
		{
			Dictionary<String, Move.Builder> moves = new Dictionary<string, Move.Builder>();
			String file = FetchFileIfNeeded("pokeMoveSorted.csv");
			foreach (var line in File.ReadAllLines(file))
			{
				Move.Builder builder = new Move.Builder();
				builder.DataId = new DataId(null, Guid.NewGuid());
				string[] fields = line.Split(new char[] { ',' }, 10);
				builder.Name = fields[0];
				String item = fields[1];
				//BuiltInType type;
				//if (item == "any")
				//{
				//	//"Any Move" support...
				//	type = BuiltInType.Normal;
				//}
				//else
				//{
				//	type = (BuiltInType)Enum.Parse(typeof(BuiltInType), item, true);
				//}
				//ITypeDefinition typeDef = TypeManager.GetBuiltInType(type);
				builder.Type = ReadType(item);// new ItemReference<ITypeDefinition>(typeDef.DataId, typeDef.Name);

				item = fields[2];
				MoveCategory category;
				if (item == "???")
				{
					//"Any Move" support
					category = MoveCategory.Physical | MoveCategory.Special | MoveCategory.Support;
				}
				else
				{
					category = ParseEnum<MoveCategory>(item);
				}
				builder.MoveCategory = category;
				builder.Power = int.Parse(fields[3]);

				//damage skill. Can be empty
				item = fields[4];
				BuiltInSkill skill;
				ISkill skillDef;
				if (!String.IsNullOrEmpty(item))
				{
					skill = ParseEnum<BuiltInSkill>(item);
					skillDef = SkillManager.GetBuiltInSkill(skill);
					builder.DamageSkill = new ItemReference<ISkill>(skillDef.DataId, skillDef.Name);
				}
				//currently cannot handle field 6.
				//TODO: Implement handling field 6... Whatever that is....

				//Accuracy
				item = fields[6];
				bool negative = item.Contains("missing", StringComparison.OrdinalIgnoreCase);
				if (negative)
				{
					item = item.Replace("missing", "", StringComparison.OrdinalIgnoreCase);
				}
				skill = String.IsNullOrEmpty(item) ? BuiltInSkill.None : ParseEnum<BuiltInSkill>(item);
				skillDef = SkillManager.GetBuiltInSkill(skill);
				builder.PrimaryAccuracySkill = new ItemReference<ISkill>(skillDef.DataId, skillDef.Name);
				builder.PrimaryAccuracyIsNegative = negative;

				item = fields[7];
				skill = String.IsNullOrEmpty(item) ? BuiltInSkill.None : ParseEnum<BuiltInSkill>(item);
				skillDef = SkillManager.GetBuiltInSkill(skill);
				builder.SecondaryAccuracySkill = new ItemReference<ISkill>(skillDef.DataId, skillDef.Name);

				//target
				item = fields[8];
				MoveTarget target;
				if (item == "Any")
				{
					//"Any Move" support
					target = MoveTarget.Battlefield;
				}
				else
				{
					target = ParseEnum<MoveTarget>(item);
				}
				builder.MoveTarget = target;

				item = fields[9];
				if (!String.IsNullOrEmpty(item) && "-" != item)
				{
					builder.Effects.Add(item);
				}
				data.Moves.Add(builder);
				moves.Add(builder.Name, builder);
			}
			return moves;
		}
		private static Dictionary<String, DexEntry.Builder> ReadDexEntries(PokeroleXmlData data)
		{
			Dictionary<String, DexEntry.Builder> monByName = new Dictionary<string, DexEntry.Builder>();
			String file = FetchFileIfNeeded("PokeroleStats.csv");
			bool first = true;
			Regex dexRegex = new Regex("^#?(D)?([0-9]+)(.*)$");
			foreach (var line in File.ReadAllLines(file))
			{
				if (first)
				{
					//skip over the header
					first = false;
					continue;
				}
				String[] items = line.Split(',');
				DexEntry.Builder builder = new DexEntry.Builder();
				builder.DataId = new DataId(null, Guid.NewGuid());

				String rawDex = items[0];
				Match m = dexRegex.Match(rawDex);
				if (!m.Success)
				{
					//wat???
					throw new InvalidOperationException("Invalid dex num!");
				}
				int dexNum = int.Parse(m.Groups[2].Value);
				String regonalVariant = m.Groups[3].Value;
				String variant = (regonalVariant) switch
				{
					"A" => "Alolan",
					"G" => "Galarian",
					"B" => "BBF",
					"" => "",
					_ => ""//separate case for breakpoints
				};
				if (m.Groups[1].Success)
				{
					variant = "Delta";
				}
				builder.Variant = variant;
				builder.DexNum = dexNum;

				builder.Name = items[1];

				builder.PrimaryType = ReadType(items[2]);
				builder.SecondaryType = ReadType(items[3]);

				builder.BaseHp = ReadInt(items[4]);
				builder.StartingStrength = ReadInt(items[5]);
				builder.MaxStrength = ReadInt(items[6]);
				builder.StartingDexterity = ReadInt(items[7]);
				builder.MaxDexterity = ReadInt(items[8]);
				builder.StartingVitality = ReadInt(items[9]);
				builder.MaxVitality = ReadInt(items[10]);
				builder.StartingSpecial = ReadInt(items[11]);
				builder.MaxSpecial = ReadInt(items[12]);
				builder.StartingInsight = ReadInt(items[13]);
				builder.MaxInsight = ReadInt(items[14]);

				for (int i = 0; i < 4; i++)
				{
					//0&1 = primary+secondary ability
					//2 = hidden ability
					//3 = event ability. Treating as a hidden ability
					var entry = ReadAbility(items[15 + i], i > 1);
					if (entry != null)
					{
						builder.Abilities.Add(entry);
					}
				}
				//next index is 19
				if (!String.IsNullOrEmpty(items[19]))
				{
					unevolvedEntries.Add(builder.DataId.Value.Uuid);
				}
				if (!String.IsNullOrEmpty(items[20]))
				{
					hasMegaEvolution.Add(builder.DataId.Value.Uuid);
				}
				builder.SuggestedRank = ParseEnum<Rank>(items[21]);

				String genderKind = items[22];
				builder.GenderType = (genderKind.ToLowerInvariant()) switch
				{
					"f" => GenderType.FemaleOnly,
					"m" => GenderType.MaleOnly,
					"n" => GenderType.None,
					_ => GenderType.Default,
				};
				data.DexEntries.Add(builder);
				String name = builder.Name;
				//if (!String.IsNullOrEmpty(builder.Variant))
				//{
				//	name = variant + " " + name;
				//}
				monByName.Add(name, builder);
			}
			return monByName;
		}

		private static void ReadAbilities(PokeroleXmlData data)
		{
			String file = FetchFileIfNeeded("PokeRoleAbilities.csv");
			using (TextFieldParser csvParser = new TextFieldParser(file))
			{
				csvParser.SetDelimiters(new string[] { "," });
				csvParser.HasFieldsEnclosedInQuotes = true;
				bool first = true;
				while (!csvParser.EndOfData)
				{
					String[] fields = csvParser.ReadFields();
					if (first)
					{
						//skip headers
						first = false;
						continue;
					}
					Ability.Builder builder = new Ability.Builder();
					builder.DataId = new DataId(null, Guid.NewGuid());
					builder.Name = fields[0];
					builder.Effect = fields[1];
					data.Abilities.Add(builder);
				}
			}
		}

		private static void ReadMoveLists(PokeroleXmlData data, Dictionary<String, DexEntry.Builder> monByName,
			Dictionary<String, Move.Builder> moveList)
		{
			String file = FetchFileIfNeeded("PokeLearnMovesFull.csv");
			foreach (var line in File.ReadLines(file))
			{
				String[] fields = line.Split(",");
				String[] id = fields[0].Split(new char[] { ' ' }, 2);
				DexEntry.Builder? builder;
				if (!monByName.TryGetValue(id[1], out builder))
				{
					throw new InvalidOperationException($"Could not find moves for {id[1]}");
				}
				for (int i = 0; i < fields.Length - 1; i += 2)
				{
					String moveName = fields[i + 1];
					String rawRank = fields[i + 2];
					Rank rank = ParseEnum<Rank>(rawRank);
					Move.Builder? move;
					if (!moveList.TryGetValue(moveName, out move))
					{
						throw new InvalidOperationException($"Could not find a move called \"{moveName}\"");
					}

					MoveEntry entry = new MoveEntry(rank, new ItemReference<Move>(move.DataId!.Value, moveName));
					builder.MoveSet.Add(entry);
				}
			}
		}

	}
}
