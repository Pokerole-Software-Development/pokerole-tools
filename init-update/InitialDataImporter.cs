/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;
using Pokerole.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Pokerole.Tools.InitUpdate
{
	class InitialDataImporter
	{
		private const String csvFetchUrl = "https://raw.githubusercontent.com/XShadeSlayerXx/PokeRole-Discord.py-Base/master/";
		//private const String csvFetchUrl = "https://raw.githubusercontent.com/SirIntellegence/PokeRole-Discord.py-Base/typofix3/";
		private const string MOVE_MISSING_DESCRIP = "This move is missing";
		private readonly HashSet<Guid> unevolvedEntries = new HashSet<Guid>();
		private readonly HashSet<Guid> hasMegaEvolution = new HashSet<Guid>();
		private readonly PokeroleXmlData data;
		private readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(PokeroleXmlData));
		private readonly Dictionary<String, Move.Builder> movesByName = new Dictionary<string, Move.Builder>();
		private readonly Dictionary<String, Item.Builder> itemsByName = new Dictionary<string, Item.Builder>();
		private readonly Dictionary<String, DexEntry.Builder> monByName = new Dictionary<string, DexEntry.Builder>();
		private readonly Dictionary<String, Ability.Builder> abilitiesByName = new Dictionary<string, Ability.Builder>();
		private const bool HAS_DLC_MON = false;
		//                                                pokerole-tools (root)--|
		//                                                  init-update ------|  |
		//                                                          bin ---|  |  |
		//                                                         Debug|  |  |  |
		//                                  netcoreapp3.1 ------------V V  V  V  V
		private static readonly String projectRoot = Path.GetFullPath("../../../..");
		private static readonly String processedInputs = Path.Combine(projectRoot, "init-update/inputs");
		
		public InitialDataImporter()
		{
			String previousOutput;
			if (File.Exists("output.xml"))
			{
				previousOutput = File.ReadAllText("output.xml");
				data = (PokeroleXmlData)xmlSerializer.Deserialize(new StringReader(previousOutput));
				//populate dicts
				movesByName = data.Moves.ToDictionary(move => move.Name!);
				itemsByName = data.Items.ToDictionary(item => item.Name!);
				monByName = data.DexEntries.ToDictionary(entry => entry.Name!);
				abilitiesByName = data.Abilities.ToDictionary(ability => ability.Name!);
			}
			if (data == null)
			{
				data = new PokeroleXmlData();
			}
		}

		public void DoImport()
		{

			//read them datas!!!
			AddEntries(movesByName, ReadMoves());
			AddEntries(itemsByName, ReadItems());
			AddEntries(monByName, ReadDexEntries());
			LinkMegaEvolves();
			AddEntries(abilitiesByName, ReadAbilities());
			ReadMoveLists();
			LinkAbilities();
			data.Images.AddRange(ReadPrimaryImages());
			data.Images.AddRange(ReadDexImages());
			GcStockImages();
			ReadHeightAndWeights();
			//ReadEvolutions();




			StringWriter writer = new StringWriter();
			xmlSerializer.Serialize(writer, data);
			File.WriteAllText("output.xml", writer.ToString());
			//Console.WriteLine(writer.ToString());

			//for the sake of testing
			PokeroleXmlData data2 = (PokeroleXmlData)xmlSerializer.Deserialize(new StringReader(writer.ToString()));

			//Console.WriteLine("Hello World!");
		}

		private void AddEntries<K, V>(Dictionary<K, V> destination, Dictionary<K, V> toAdd) where
			K : notnull
		{
			if (destination == toAdd)
			{
				return;
			}
			foreach (var item in toAdd)
			{
				destination.Add(item.Key, item.Value);
			}
		}

		private String FetchFileIfNeeded(String file)
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
		private T ParseEnum<T>(string val) where T : Enum
		{
			return (T)Enum.Parse(typeof(T), val.Replace(" ", ""), true);
		}
		private ItemReference<ITypeDefinition>? ReadType(string val)
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
		private int? ReadInt(String val)
		{
			if (String.IsNullOrEmpty(val))
			{
				return null;
			}
			return int.Parse(val);
		}
		private AbilityEntry? ReadAbility(String val, bool hidden)
		{
			if (String.IsNullOrEmpty(val))
			{
				return null;
			}
			ItemReference<Ability> reference = new ItemReference<Ability>(default, val);
			return new AbilityEntry(hidden, reference);
		}
		private Dictionary<String, Move.Builder> ReadMoves()
		{
			Dictionary<String, Move.Builder> moves = new Dictionary<string, Move.Builder>();
			String file = FetchFileIfNeeded("pokeMoveSorted.csv");
			using (TextFieldParser csvParser = new TextFieldParser(file))
			{
				csvParser.SetDelimiters(new string[] { "," });
				csvParser.HasFieldsEnclosedInQuotes = true;
				while (!csvParser.EndOfData)
				{
					Move.Builder? builder;
					string[] fields = csvParser.ReadFields();
					if (fields[0] == "Struggle")
					{
						//read a physical and special variant
						String[] physical, special;
						physical = new String[fields.Length];
						special = new String[fields.Length];
						for (int i = 0; i < fields.Length; i++)
						{
							String item = fields[i];
							if (item.Contains("/"))
							{
								String[] halves = item.Split('/');
								physical[i] = halves[0];
								special[i] = halves[1];
							}
							else
							{
								physical[i] = item;
								special[i] = item;
							}
						}
						physical[0] = "Struggle (Physical)";
						special[0] = "Struggle (Special)";
						builder = ReadMove(physical);
						if (builder != null)
						{
							data.Moves.Add(builder);
							moves.Add(builder.Name!, builder);
						}
						builder = ReadMove(special);
						if (builder != null)
						{
							data.Moves.Add(builder);
							moves.Add(builder.Name!, builder);
						}
						continue;
					}
					builder = ReadMove(fields);
					if (builder == null)
					{
						continue;
					}
					data.Moves.Add(builder);
					String? name = builder.Name;
					if (name == null)
					{
						throw new WasNullException("Move name missing");
					}
					moves.Add(name, builder);
				}
			}
			//these moves should exist, but don't, so create dummy entries for them
			String[] missingMoves =
			{
				"Poltergeist",
				//"Behemoth Blade",
				//"Behemoth Bash"
				"Expanding Force",
				"Shell Side Arm",
				"Eerie Spell",
				"Freezing Glare",
				"Thunderous Kick",
				"Fiery Wrath"
			};
			ISkill noneSkill = SkillManager.GetBuiltInSkill(BuiltInSkill.None);
			ITypeDefinition normalType = TypeManager.GetBuiltInType(BuiltInType.Normal);
			foreach (var moveName in missingMoves)
			{
				if (moves.ContainsKey(moveName) || movesByName.ContainsKey(moveName))
				{
					continue;
				}

				Move.Builder builder = new Move.Builder()
				{
					DataId = new DataId(null, Guid.NewGuid()),
					Name = moveName,
					Type = normalType.ItemReference,
					Description = MOVE_MISSING_DESCRIP,
					MoveCategory = MoveCategory.Invalid,
					PrimaryAccuracySkill = noneSkill.ItemReference,
					SecondaryAccuracySkill = noneSkill.ItemReference,

				};
				data.Moves.Add(builder);
				moves.Add(moveName, builder);
			}
			return moves;
		}

		private Move.Builder? ReadMove(string[] fields)
		{
			//officially, the fields are (in order):
			//name, type, pwrtype, power, dmg1, dmg2, acc1, acc2, foe, effect, description
			Guid guid = Guid.Empty;
			//someone mispelled something...
			String moveName = NameErrata(fields[0]);
			//String moveName = fields[0].Replace("Behemot ", "Behemoth ");
			if (movesByName.TryGetValue(moveName, out Move.Builder? prevEntry))
			{
				if (prevEntry.Description != MOVE_MISSING_DESCRIP)
				{
					//we already have that one
					return null;
				}
				//use existing id to avoid issues
				guid = prevEntry.DataId!.Value.Uuid;
			}
			Move.Builder builder = new Move.Builder();
			if (guid == Guid.Empty)
			{
				guid = Guid.NewGuid();
			}
			builder.DataId = new DataId(null, guid);
			builder.Name = moveName;
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
			//damage 2
			item = fields[5];
			bool negative;
			if (!String.IsNullOrEmpty(item))
			{
				negative = item.Contains("missing", StringComparison.OrdinalIgnoreCase);
				if (negative)
				{
					item = item.Replace("missing", "", StringComparison.OrdinalIgnoreCase);
				}
				skill = ParseEnum<BuiltInSkill>(item);
				skillDef = SkillManager.GetBuiltInSkill(skill);
				builder.SecondaryDamageSkill = new ItemReference<ISkill>(skillDef.DataId, skillDef.Name);
				builder.SecondaryDamageIsNegative = negative;
			}


			//currently cannot handle field 6.
			//TODO: Implement handling field 6... Whatever that is....

			//Accuracy
			item = fields[6];
			negative = item.Contains("missing", StringComparison.OrdinalIgnoreCase);
			if (negative)
			{
				item = item.Replace("missing", "", StringComparison.OrdinalIgnoreCase);
			}
			if (item.Contains("/"))
			{
				//has two variants
				skill = BuiltInSkill.Varies;
			}
			else
			{
				skill = String.IsNullOrEmpty(item) ? BuiltInSkill.None : ParseEnum<BuiltInSkill>(item);
			}
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
			item = fields[10];
			builder.Description = item;


			return builder;
		}

		private Dictionary<String, Item.Builder> ReadItems()
		{
			String file = FetchFileIfNeeded("PokeRoleItems.csv");
			Dictionary<String, Item.Builder> items = new Dictionary<string, Item.Builder>(200);
			using (TextFieldParser csvParser = new TextFieldParser(file))
			{
				csvParser.SetDelimiters(new string[] { "," });
				csvParser.HasFieldsEnclosedInQuotes = true;
				//name+descrip only for now
				while (!csvParser.EndOfData)
				{
					String[] fields = csvParser.ReadFields();
					if (String.IsNullOrEmpty(fields[1]))
					{
						//delimiter. Ignore
						continue;
					}
					String name = fields[0];
					if (itemsByName.ContainsKey(name))
					{
						//already imported
						continue;
					}
					Item.Builder builder = new Item.Builder
					{
						Name = fields[0],
						Description = fields[1],
						DataId = new DataId(null, Guid.NewGuid())
					};
					items.Add(builder.Name, builder);
					data.Items.Add(builder);
				}
			}
			return items;
		}
		private Dictionary<String, DexEntry.Builder> ReadDexEntries()
		{
			String file = FetchFileIfNeeded("PokeroleStats.csv");
			bool first = true;
			Regex dexRegex = new Regex("^#?(D)?([0-9]+)(.*)$");
			foreach (var line in File.ReadAllLines(file))
			{
				//official order:
				//number, name, type1, type2, hp, str, maxstr, dex, maxdex, vit, maxvit, spc, maxspc, ins, maxins,
				//ability, ability2, abilityhidden, abilityevent, unevolved, form, rank, gender, generation
				if (first)
				{
					//skip over the header
					first = false;
					continue;
				}
				String[] items = line.Split(',');
				if (monByName.ContainsKey(NameErrata(items[1])))
				{
					//already imported
					continue;
				}
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
					"B" => "Ash",
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
				builder.Name = NameErrata(builder.Name);

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

		private string NameErrata(string name)
		{
			return name switch
			{
				"Necrozma Dark Wings" => "Necrozma Dawn Wings",
				//"Behemot Blade" => "Behemoth Blade", resolved in repo
				//"Behemot Bash" => "Behomoth Bash",
				//"Roar of Time" => "Roar Of Time",
				//"Light of Ruin" => "Light Of Ruin",
				_ => name,
			};
		}

		private Dictionary<String, Ability.Builder> ReadAbilities()
		{
			string file = FetchFileIfNeeded("PokeRoleAbilities.csv");
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
					if (abilitiesByName.ContainsKey(fields[0]))
					{
						//we already have that one
						continue;
					}
					Ability.Builder builder = new Ability.Builder
					{
						DataId = new DataId(null, Guid.NewGuid()),
						Name = fields[0],
						Effect = fields[1]
					};
					data.Abilities.Add(builder);
					abilitiesByName.Add(builder.Name, builder);
				}
			}
			return abilitiesByName;
		}

		private void ReadMoveLists()
		{
			String file = FetchFileIfNeeded("PokeLearnMovesFull.csv");
			foreach (var line in File.ReadLines(file))
			{
				String[] fields = line.Split(",");
				String[] id = fields[0].Split(new char[] { ' ' }, 2);
				if (!monByName.TryGetValue(NameErrata(id[1]), out DexEntry.Builder? builder))
				{
					throw new InvalidOperationException($"Could not find moves for {id[1]}");
				}
				if (builder.MoveSet.Count > 0)
				{
					//already populated
					continue;
				}

				for (int i = 0; i < fields.Length - 1; i += 2)
				{
					String moveName = fields[i + 1];
					String rawRank = fields[i + 2];
					if (String.IsNullOrEmpty(rawRank))
					{
						//no more data
						break;
					}
					Rank rank = ParseEnum<Rank>(rawRank);
					if (!movesByName.TryGetValue(NameErrata(moveName), out Move.Builder? move))
					{
						throw new InvalidOperationException($"Could not find a move called \"{moveName}\"");
					}

					MoveEntry entry = new MoveEntry(rank, move.ItemReference!.Value);
					builder.MoveSet.Add(entry);
				}
			}
		}
		private void LinkAbilities()
		{
			List<String> missingAbilities = new List<string>(10);
			foreach (var entry in monByName.Values)
			{
				List<AbilityEntry> newEntries = new List<AbilityEntry>(entry.Abilities.Count);
				foreach (var ability in entry.Abilities)
				{
					//lookup the ability
					if (!abilitiesByName.TryGetValue(ability.Ability.DisplayName!, out Ability.Builder? abilityBuilder))
					{
						throw new InvalidOperationException($"Unknown ability: {ability.Ability.DisplayName}");
						//missingAbilities.Add(ability.Ability.DisplayName!);
						//newEntries.Add(ability);
						//continue;
					}
					newEntries.Add(new AbilityEntry(ability.Hidden, abilityBuilder.ItemReference!.Value));
				}
				entry.Abilities = newEntries;
			}
			if (missingAbilities.Count > 0)
			{
				Debugger.Break();
			}

		}
		private void LinkMegaEvolves()
		{
			List<Item.Builder> megastones = (from keyPair in itemsByName
											 where keyPair.Key.EndsWith("ite") || keyPair.Key.EndsWith("ite X") ||
											 keyPair.Key.EndsWith("ite Y")
											 select
											 keyPair.Value).ToList();
			foreach (var builder in data.DexEntries)
			{
				if (!builder.Name!.StartsWith("Mega-"))
				{
					continue;
				}
				String sansMega = builder.Name!.Substring("Mega-".Length);
				string kind = "";
				if (!monByName.TryGetValue(sansMega, out DexEntry.Builder? baseEntry))
				{
					//probably charizard or Mewtwo
					kind = "" + sansMega[^1];
					//chop off the " X" or " Y"
					sansMega = sansMega[0..^2];
					if (!monByName.TryGetValue(sansMega, out baseEntry))
					{
						throw new InvalidOperationException($"Could not find base entry for {builder.Name}!");
					}
				}
				//has this been found already?
				bool found = false;
				foreach (var item in baseEntry.MegaEvolutions)
				{
					if (item.TargetEvolution.DataId.Uuid == builder.ItemReference!.Value.DataId.Uuid)
					{
						found = true;
					}
				}
				if (found)
				{
					continue;
				}
				//find the item for this mega evolve
				String prefix = sansMega[0..^3];
				Item.Builder? evolveItem = null;
				if (baseEntry.Name == "Rayquaza")
				{
					//doesn't need an item, but giving no item is not an option, so we are giving it a pokeball
					//since that is nonsense anyway
					evolveItem = itemsByName["Pokeball"];
				}
				else
				{
					foreach (var item in megastones)
					{
						if (item.Name!.StartsWith(prefix))
						{
							if (kind == "")
							{
								evolveItem = item;
								break;
							}
							if (item.Name!.EndsWith(kind))
							{
								evolveItem = item;
								break;
							}
						}
					}
				}
				if (evolveItem == null)
				{
					throw new InvalidOperationException($"Could not find mega item for {sansMega}");
				}
				builder.MegaEvolutionBaseEntry = baseEntry.ItemReference;
				baseEntry.MegaEvolutions.Add(new MegaEvolutionEntry(evolveItem.ItemReference!.Value,
					builder.ItemReference!.Value));
			}
		}
		private List<ImageRef.Builder> ReadPrimaryImages()
		{
			//this doesn't have to be efficient. It just needs to work!!!
			String primaryImageDir = Path.Combine(projectRoot, "Images/Crunched/PrimaryImages");
			var filenames = new List<String>(Directory.GetFiles(primaryImageDir));
			RemoveEvilImages(filenames);
			var imageData = filenames.Select(file =>
			{
				String filename = Path.GetFileNameWithoutExtension(file);
				if (!filename.StartsWith("HOME"))
				{
					throw new InvalidOperationException();
				}
				String remaining = filename[4..];
				int dexNum = int.Parse(remaining[0..3]);
				//not interested in egg
				if (dexNum == 0)
				{
					return null;
				}
				remaining = remaining[3..];
				bool mega = false;
				bool? megaVariantIsX = null;
				bool gigantamax = false;
				char? variant = null;
				String? misc = null;
				int subIndex = remaining.IndexOf('_');
				//there is no mega for unown. There is an M form though. And an EX form and a QU form
				if (dexNum == 201)
				{
					if (remaining.Length > 0 && remaining[0] != '_')
					{
						String preview;
						if (remaining.Length > 1)
						{
							preview = remaining[0..2];
						}
						else
						{
							preview = remaining[0..1];
						}
						preview = preview.TrimEnd('_');
						misc += preview;
						remaining = remaining[preview.Length..];
					}
				}
				else if (remaining.Length > 0 && subIndex != 0)
				{
					String infoBlock;
					if (subIndex > -1)
					{
						infoBlock = remaining[0..subIndex];
						remaining = remaining[subIndex..];
					}
					else
					{
						infoBlock = remaining;
						remaining = "";
					}
					if (infoBlock.StartsWith("Gi"))
					{
						gigantamax = true;
						infoBlock = infoBlock[2..];
					}
					if (infoBlock.Length > 2)
					{
						if ("RGi" == infoBlock)
						{
							//gigantamax rapid strike
							if (dexNum != 892)
							{
								throw new InvalidOperationException();
							}
							gigantamax = true;
							variant = 'R';
						}
						else
						{
							//dat is a word, such as an Arcius sub type.
							misc += infoBlock;
						}
						infoBlock = "";
					}
					//else if (dexNum == 676)
					//{
					//	//676: doggie styles
					//	//someone has some styles...
					//	misc += infoBlock;
					//	infoBlock = "";
					//}
					else if (infoBlock.StartsWith("M"))
					{
						if (dexNum == 745)
						{
							//not mega, is midnight lycanrock
							misc += infoBlock[0..2];
							infoBlock = infoBlock[2..];
							variant = 'M';//for simplicity
						}
						else
						{
							mega = true;
							infoBlock = infoBlock[1..];
							if (infoBlock.Length > 0)
							{
								megaVariantIsX = infoBlock[0] == 'X' ? true : infoBlock[0] == 'Y' ? false : null;
							}
							if (megaVariantIsX.HasValue)
							{
								infoBlock = infoBlock[1..];
							}
						}
					}
					if (infoBlock.Length > 1)
					{
						var part = infoBlock[0..2];
						if (part == "GZ")
						{
							if (dexNum != 555)
							{
								throw new InvalidOperationException();
							}
							variant = 'G';
							misc += "Z";
						}
						else
						{
							//two next chars go into misc since they are specific
							misc += part;
						}
						infoBlock = infoBlock[2..];
					}
					if (infoBlock.Length > 0)
					{
						variant = infoBlock[0];
						infoBlock = infoBlock[1..];
					}
					if (infoBlock.Length > 0)
					{
						//should have all been consumed
						throw new NotImplementedException();
					}
				}
				bool female = false;
				if (remaining.StartsWith("_f"))
				{
					female = true;
					remaining = remaining[2..];
				}
				bool shiny = false;
				if (remaining.StartsWith("_s"))
				{
					//shiny
					shiny = true;
					remaining = remaining[2..];
				}
				bool backImage = false;
				if (remaining.StartsWith("_b"))
				{
					if (dexNum == 255)
					{
						//I don't care about your stupid backside torchic
						//https://discord.com/channels/245675629515767809/324413827632463872/905276601279856670
						//Word of Glee: On a scale of 1-10, your backside has an importance of 0
						return null;
					}
					backImage = true;
					remaining = remaining[2..];
				}
				if (remaining.Length == 0)
				{
					return new ImageData(file, dexNum, mega, megaVariantIsX, gigantamax, female, shiny, variant,
						backImage, misc);
				}
				//here;



				throw new NotImplementedException();
			}).NonNull().ToList();
			var missed = imageData.Where(item => !String.IsNullOrEmpty(item.Misc)).ToList();
			//delta may be slated for removal. Also, the numbers don't line up and I don't have images for them
			var effectiveDex = data.DexEntries.Where(item => item.DexNum > 0 && item.Variant != "Delta").ToList();
			var remainingImages = new HashSet<ImageData>(imageData);
			var remainingDexEntries = new HashSet<DexEntry.Builder>(effectiveDex);
			//dict compile
			//not using "dexNotation" since we don't know what "Variant" might be
			//egg already filtered out
			var imagesByDex = imageData.ToLookup(item=>item.DexNum);
			var entriesByDex = effectiveDex.ToLookup(item => item.DexNum);
			//if (imagesByDex.Count != entriesByDex.Count) We are missing Galar DLC mon, so we are not doing this check
			//{
			//	//not expected
			//	throw new InvalidOperationException();
			//}
			List<ImageRef.Builder> result = new List<ImageRef.Builder>(imageData.Count);
			//link them up!
			foreach (var item in imagesByDex)
			{
				var images = item.ToList();
				var dexEntries = entriesByDex[item.Key].ToList();
				if (images.Count < 1 || dexEntries.Count < 1)
				{
					//We are missing Galar DLC mon, so we are not doing this check
					continue;
					////items missing!!!!
					//throw new InvalidOperationException();
				}
				ImageData? shiny, normal;
				DexEntry.Builder builder;
				ImageRef.Builder imageBuilder;
				if (images.Count == 2 && dexEntries.Count == 1)
				{
					//hurray! Simplicity!!!
					shiny = images.Single(thing => thing.Shiny);
					normal = images.Single(thing => !thing.Shiny);
					builder = dexEntries[0];


					imageBuilder = MakeImageRef(normal.Filename);
					builder.PrimaryImage = imageBuilder.ItemReference;
					result.Add(imageBuilder);
					imageBuilder = MakeImageRef(shiny.Filename);
					builder.ShinyImage = imageBuilder.ItemReference;
					result.Add(imageBuilder);
					remainingDexEntries.Remove(builder);
					remainingImages.Remove(normal);
					remainingImages.Remove(shiny);
					continue;
				}
				ProcessPrimaryImages(item.Key, images, dexEntries, remainingDexEntries, remainingImages, result);
			}
			//exclude images for the ones we don't have dex entries for since I don't have them for 891+, but DON'T
			//skip them if we DO have the stat blocks
			int imageRemainingCount = HAS_DLC_MON ? remainingImages.Count :
				remainingImages.Count(item => entriesByDex.Contains(item.DexNum));
			if (imageRemainingCount > 0 || remainingDexEntries.Count > 0)
			{
				throw new InvalidOperationException("Items were missed!");
			}
			return result;
		}

		private void ProcessPrimaryImages(int dexNum, List<ImageData> images, List<DexEntry.Builder> entries,
			HashSet<DexEntry.Builder> remainingEntries, HashSet<ImageData> remainingImages,
			List<ImageRef.Builder> results)
		{
			HashSet<ImageData> toRemove = new HashSet<ImageData>();
			foreach (var entry in entries)
			{
				toRemove.Clear();
				entry.AdditionalFemaleImages.Clear();
				entry.AdditionalImages.Clear();
				entry.AdditionalShinyFemaleImages.Clear();
				entry.AdditionalShinyImages.Clear();
				foreach (ImageData image in images)
				{
					ItemReference<ImageRef> MakeAndAdd()
					{
						var result = MakeImageRef(image.Filename);
						results.Add(result);
						toRemove.Add(image);
						remainingImages.Remove(image);
						return result.ItemReference!.Value;
					}
					bool VariantCompare()
					{
						String name = entry.Name ?? throw new WasNullException("Null name");
						switch (dexNum)
						{
							case 386:
								//deoxys
								return image.Variant switch
								{
									'A' => name.StartsWith("Attack"),
									'D' => name.StartsWith("Defense"),
									'S' => name.StartsWith("Speed"),
									_ => name == "Deoxys",
								};
							case 413:
								return image.Variant switch
								{
									'G' => name.StartsWith("Ground"),
									'S' => name.StartsWith("Steel"),
									null => name.StartsWith("Grass"),
									_ => throw new InvalidOperationException(),
								};
							case 479:
								//rotom
								return (image.Variant) switch
								{
									'D' => name.EndsWith("Dex"),
									'F' => name.EndsWith("Fan"),
									'L' => name.EndsWith("Mow"),
									'O' => name.EndsWith("Heat"),
									'R' => name.EndsWith("Frost"),
									'W' => name.EndsWith("Wash"),
									null => name == "Rotom",
									_ => throw new InvalidOperationException()
								};
							case 555:
								//stupid dumb...
								//Darmanitan
								switch (image.Variant)
								{
									case 'G':
										if (String.IsNullOrEmpty(image.Misc))
										{
											return name == "Galarian Darmanitan";
										}
										return name == "Galarian Zen Darmanitan";
									case 'Z':
										return name == "Zen Darmanitan";
									case null:
										return name == "Darmanitan";
								}
								throw new InvalidOperationException();
							case 718:
								return image.Variant switch
								{
									'L' => name.EndsWith("Cell"),
									'T' => name.EndsWith("10%"),
									'C' => name.EndsWith("100%"),
									null => name.EndsWith("50%"),
									_ => throw new InvalidOperationException()
								};
							case 741:
								return image.Misc switch
								{
									"Pa" => name.EndsWith("Psychic"),
									"Po" => name.EndsWith("Electric"),
									"Se" => name.EndsWith("Ghost"),
									"" or null => name.EndsWith("Fire"),
									_ => throw new InvalidOperationException()
								};
							case 745:
								return image.Variant switch
								{
									'D' => name.EndsWith("Dusk"),
									'M' => name.EndsWith("Midnight"),
									null => name.EndsWith("Midday"),
									_ => throw new InvalidOperationException()
								};
							case 746:
								return image.Misc switch
								{
									"Sc" => name.EndsWith("Swarm"),
									null or "" => name == "Wishiwashi",
									_ => throw new InvalidOperationException()
								};
							case 800:
								if (image.Variant == 'U')
								{
									return name.EndsWith("Ultra Burst");
								}
								return image.Misc switch
								{
									"DM" => name.EndsWith("Dusk Mane"),
									"DW" => name.EndsWith("Dawn Wings"),
									null or "" => name == "Necrozma",
									_ => throw new InvalidOperationException()
								};
							case 849:
								return image.Variant switch
								{
									'L' => name.EndsWith("Low Key"),
									null => name.EndsWith("Amped"),
									_ => throw new InvalidOperationException()
								};
							case 875:
								return image.Variant switch
								{
									'N' => name.StartsWith("Form"),
									null => name == "Eiscue",
									_ => throw new InvalidOperationException()
								};

						}
						return image.Variant switch
						{
							'A' => entry.Variant == "Ash" || entry.Variant == "Alolan",
							'G' => entry.Variant == "Galarian",
							//technically, 'P' means Primal, but it is easier to handle it here than elsewhere
							'P' => name.StartsWith("Primal") || name.StartsWith("Form"),
							'O' => name.StartsWith("Origin"),
							'S' => name.StartsWith("Sky"),
							'T' => name.StartsWith("Form"),
							'B' => name.StartsWith("Black") || name.StartsWith("Form"),
							'W' => name.StartsWith("White"),
							'R' => name.StartsWith("Form"),
							'U' => name.EndsWith("Unbound"),
							'C' => name.StartsWith("Form"),
							null => String.IsNullOrEmpty(entry.Variant),
							_ => throw new NotImplementedException(),
						};
					}
					//pain!!!
					if (dexNum == 774)
					{
						//many forms and two dex entries...
						//apparently there is no shiny form for "shields up"
						if (!entry.Name!.Contains("Core") && !image.Shiny && !image.Variant.HasValue)
						{
							entry.PrimaryImage = MakeAndAdd();
						}
						else if (entry.Name!.Contains("Core") && (image.Shiny || image.Variant.HasValue))
						{
							if (image.Shiny)
							{
								entry.ShinyImage = MakeAndAdd();
							}
							else if (image.Variant == 'R')//red is my favorite color, so it is the primary image
							{
								entry.PrimaryImage = MakeAndAdd();
							}
							else
							{
								entry.AdditionalImages.Add(MakeAndAdd());
							}
						}
						continue;
					}
					//dex entries that have multiple forms but one entry. These have no mega forms
					bool isSingleWithMultipleImages = (entry.DexNum) switch
					{
						201 or 351 or 412 or 421 or 422 or 423 or 493 or 550 or 585 or 586 or 649 or 666 or
							669 or 670 or 671 or 676 or 710 or 711 or 716 or 773 or 778 or 801 or 845 or
							854 or 855 or 869 or 877 or 890 => true,
						_ => false
					};
					if (isSingleWithMultipleImages)//unkown has no mega or any other special
					{
						//201=Unkown
						//351=Castform
						//412=Burmy
						//421=Cherrim
						//422=Shellos
						//423=Gastrodon
						//493=Arceus
						//550=Basculin
						//585=Deerling
						//586=Sawsbuck
						//649=Genesect
						//666=Vivillon
						//669-671=Flabébé line
						//676=Furfrou
						//710-711=Pumpkaboo line
						//716=Xerneas
						//773=Silvally
						//778=Mimikyu
						//801=Magearna
						//845=Cramorant
						//854-855=Sinistea line
						//869=Alcremie
						//877=Morpeko
						//890=Eternatus
						//assert that there is only one entry
						if (entries.Count > 1)
						{
							throw new InvalidOperationException("Entry with one presumed entry has multiple");
						}
						if (!image.Variant.HasValue && String.IsNullOrEmpty(image.Misc) && !image.BackImage &&
							!image.Gigantamax)//Alcremie has gigantamax
						{
							if (image.Shiny)
							{
								entry.ShinyImage = MakeAndAdd();
							}
							else
							{
								entry.PrimaryImage = MakeAndAdd();
							}
						}
						else
						{
							//other letters or forms
							if (image.Shiny)
							{
								entry.AdditionalShinyImages.Add(MakeAndAdd());
							}
							else
							{
								entry.AdditionalImages.Add(MakeAndAdd());
							}
						}
						continue;
					}
					//filter
					if (image.Mega != entry.MegaEvolutionBaseEntry.HasValue)
					{
						continue;
					}
					if (image.MegaVariantIsX.HasValue)
					{
						if (image.MegaVariantIsX.Value != entry.Name!.EndsWith("X"))
						{
							continue;
						}
					}
					if (entry.DexNum == 25 && image.Variant.HasValue)
					{
						//stupid hats....
						entry.AdditionalImages.Add(MakeAndAdd());
						continue;
					}
					if (!VariantCompare())
					{
						continue;
					}
					if (!String.IsNullOrEmpty(image.Misc) && dexNum != 555 && dexNum != 741 && dexNum != 745 &&
						dexNum != 746 && dexNum != 800)
					{
						//not implemented
						throw new NotImplementedException();
					}
					//so I won't have to re-write the parsing logic more than once...
					Action<ItemReference<ImageRef>>? itemSetter = null;
					if (image.BackImage || image.Gigantamax)
					{
						if (image.Female)
						{
							itemSetter = image.Shiny ? item => entry.AdditionalShinyFemaleImages.Add(item) :
								item => entry.AdditionalFemaleImages.Add(item);
						}
						else
						{
							itemSetter = image.Shiny ? item => entry.AdditionalShinyImages.Add(item) :
								item => entry.AdditionalImages.Add(item);
						}
					}
					else if (image.Female)
					{
						itemSetter = image.Shiny ? item => entry.ShinyFemaleImage = item :
							item => entry.PrimaryFemaleImage = item;
					}
					else if (image.Shiny)
					{
						itemSetter = item => entry.ShinyImage = item;
					}
					else
					{
						itemSetter = item => entry.PrimaryImage = item;
					}
					if (itemSetter == null)
					{
						throw new NotImplementedException();
					}
					itemSetter(MakeAndAdd());
				}
				images.RemoveAll(toRemove.Contains);
				remainingEntries.Remove(entry);
			}
			if (images.Count > 0)
			{
				//don't have the galaran birds yet...
				//if (images.Count == 2 && (dexNum == 144 || dexNum == 145 || dexNum == 146))
				//{
				//	return;
				//}
				throw new InvalidOperationException("All images were not consumed");
			}
		}
		private static void RemoveEvilImages(List<string> filenames) => filenames.RemoveAll(file => (Path.GetFileNameWithoutExtension(file)) switch
		{
			"449Hippopotas-BothGenders" or "450Hippowdon-BothGenders" => true,
			_ => false
		});
		private List<ImageRef.Builder> ReadDexImages()
		{
			String pokespriteDir = Path.Combine(projectRoot, "ProjectReference/pokesprite");
			JObject dexData;
			using (JsonReader reader = new JsonTextReader(new StreamReader(Path.Combine(pokespriteDir, "data",
				"pokemon.json"))))
			{
				dexData = JObject.Load(reader);
			}
			static JToken GetNonNullValue(JToken parent, params String[] names)
			{
				JToken token = parent;
				foreach (var name in names)
				{
					var value = token[name] ?? throw new WasNullException($"'{name}' was null!");
					token = value;
				}
				return token;
			}
			static JProperty PropertyFromToken(JToken token)
			{
				if (token.Type != JTokenType.Property)
				{
					throw new InvalidOperationException("Given token was not a Property");
				}
				return (JProperty)token;
			}
			//static JObject ObjectFromToken(JToken token)
			//{
			//	if (token.Type != JTokenType.Object)
			//	{
			//		throw new InvalidOperationException("Given token was not an Object");
			//	}
			//	return (JObject)token;
			//}
			//gota collect them all!
			//*2 for shinies, .5 for misc images
			Dictionary<int, PokeSpriteDexEntry> spriteEntries = new Dictionary<int, PokeSpriteDexEntry>(
				(int)(data.DexEntries.Count * 2.5));
			//we are just parsing values here. NOT INTERPRETING THEM
			foreach (var node in dexData)
			{
				int dexNum = int.Parse(node.Key);
				PokeSpriteDexEntry entry = new PokeSpriteDexEntry(dexNum);
				spriteEntries.Add(dexNum, entry);
				var value = node.Value;
				if (value == null)
				{
					throw new WasNullException("Value was null");
				}
				foreach (var item in GetNonNullValue(value, "name"))
				{
					var prop = PropertyFromToken(item);
					entry.name[prop.Name] = (String?)prop.Value ?? throw new WasNullException("prop was null");
				}
				foreach (var item in GetNonNullValue(value, "slug"))
				{
					var prop = PropertyFromToken(item);
					entry.slug[prop.Name] = (String?)prop.Value ?? throw new WasNullException("prop was null");
				}
				void ReadForms(JObject formNode)
				{
					foreach(var pair in formNode)
					{
						String key = pair.Key;
						if (!entry.forms.TryGetValue(key, out PokeSpriteForm? form))
						{
							form = new PokeSpriteForm();
							entry.forms[key] = form;
						}
						//if ((bool?)pair.Value?["is_prev_gen_icon"] ?? false)
						//{
						//	//nothing new
						//	continue;
						//}
						foreach (JProperty prop in pair.Value ?? throw new WasNullException("Form pair"))
						{
							switch (prop.Name)
							{
								case "is_alias_of":
									form.alias = (String?)prop.Value ?? throw new WasNullException();
									break;
								case "is_unofficial_icon":
									form.unofficial = (bool?)prop.Value ?? throw new WasNullException();
									break;
								case "is_unofficial_legacy_icon":
									form.unofficialLegacy = (bool?)prop.Value ?? throw new WasNullException();
									break;
								case "is_prev_gen_icon":
									form.isPrevGen = (bool?)prop.Value ?? throw new WasNullException();
									continue;
								case "has_right":
									form.hasRight = (bool?)prop.Value ?? throw new WasNullException();
									break;
								case "has_female":
									form.hasFemale = (bool?)prop.Value ?? throw new WasNullException();
									break;
								case "has_unofficial_female_icon":
									form.unofficialFemale = (bool?)prop.Value ?? throw new WasNullException();
									break;
								default:
									throw new InvalidOperationException($"Unknown form prop: {prop.Name}");
							}
						}
					}
				}
				var gen7 = value["gen-7"]?["forms"];// GetNonNullValue(value, "gen-7", "forms");
				var gen8 = GetNonNullValue(value, "gen-8", "forms");
				if (gen7 != null)// null for gen8 mon
				{
					ReadForms((JObject)gen7);
				}
				ReadForms((JObject)gen8);
			}
			HashSet<DexEntry.Builder> remainingDexEntries = data.DexEntries.Where(item => !item.Name!.StartsWith("Delta")
					&& item.DexNum.HasValue && item.DexNum.Value > 0).ToHashSet();
			ILookup<int, DexEntry.Builder> entriesByDexNum = remainingDexEntries.ToLookup(item => item.DexNum!.Value);
			String imagesDir = Path.Combine(pokespriteDir, "pokemon-gen8");
			List<ImageRef.Builder> newImages = new List<ImageRef.Builder>(remainingDexEntries.Count * 3);
			//Orderby since I don't recall if dicts care about that...
			foreach (var spritePair in spriteEntries.OrderBy(pair => pair.Key))
			{
				int dexNum = spritePair.Key;
				IEnumerable<DexEntry.Builder> entries = entriesByDexNum[dexNum];
				if (entries.Count() < 1)
				{
					if (dexNum > 890 && !HAS_DLC_MON)
					{
						continue;
					}
					throw new NotImplementedException();
				}
				HashSet<DexEntry.Builder> remaining = new HashSet<DexEntry.Builder>(entries);
				var spriteDex = spritePair.Value;
				foreach (var formPair in spriteDex.forms)
				{
					String key = formPair.Key;
					PokeSpriteForm form = formPair.Value;
					//skipping aliases for now
					if (!String.IsNullOrWhiteSpace(form.alias))
					{
						//skip for now to avoid creating garbage entries
						continue;
					}
					////dereference alias
					//while (!String.IsNullOrEmpty(form.alias))
					//{

					//}
					ItemReference<ImageRef> MakeImage(bool shiny, bool female, bool right)
					{
						bool gen7 = form.isPrevGen;
						String baseSlug = spriteDex.Slug;
						String formName = key;
						if (formName == "$")
						{
							//base
							formName = "";
						}
						List <String> parts = new List<string>(10);
						//no gen 8 icons exist that face right
						parts.Add(gen7 || right ? "pokemon-gen7x" : "pokemon-gen8");
						parts.Add(shiny ? "shiny" : "regular");
						//to my astonishment, no female images have a right variant...
						if (female && right)
						{
							throw new NotImplementedException();//not handled elsewhere
						}
						if (female)
						{
							parts.Add("female");
						}
						if (right)
						{
							parts.Add("right");
						}
						String baseName = baseSlug;
						if (!String.IsNullOrWhiteSpace(formName))
						{
							baseName += $"-{formName}";
						}
						parts.Add(baseName + ".png");
						//Note: we are using the path section we build since we would otherwise have duplicate filenames
						String fileNamePart = Path.Join(parts.ToArray());
						String filenameFull = Path.GetFullPath(Path.Join(pokespriteDir, fileNamePart));
						ImageRef.Builder builder = new ImageRef.Builder()
						{
							Filename = fileNamePart,
							Data = ReadImage(filenameFull),
							DataId = new DataId(null, Guid.NewGuid())
						};
						if (!File.Exists(filenameFull))
						{
							throw new InvalidOperationException();
						}
						newImages.Add(builder);
						return builder.ItemReference!.Value;
					}
					bool IsVariant(DexEntry.Builder entry, out bool isAdditional)
					{
						isAdditional = false;
						//specific first, then general
						bool isSingleWithMultipleImages = (dexNum) switch
						{
							25 or 133 or 172 or 201 or 249 or 327 or 351 or 412 or 421 or 422 or 423 or 493 or 550 or 585 or 586 or 649 or
								666 or 669 or 670 or 671 or 676 or 710 or 711 or 716 or 773 or 778 or 801 or 845 or
								854 or 855 or 869 or 877 or 890 => true,
							_ => false
						};
						if (isSingleWithMultipleImages)
						{
							//25=Pikachu (hats)
							//133=Eevee (Starter)
							//172=Pichu (Spiky ear)
							//201=Unkown
							//249=Lugia (Shadow)
							//327=Spinda (Don't know why...)
							//351=Castform
							//412=Burmy
							//421=Cherrim
							//422=Shellos
							//423=Gastrodon
							//493=Arceus
							//550=Basculin
							//585=Deerling
							//586=Sawsbuck
							//649=Genesect
							//666=Vivillon
							//669-671=Flabébé line
							//676=Furfrou
							//710-711=Pumpkaboo line
							//716=Xerneas
							//773=Silvally
							//778=Mimikyu
							//801=Magearna
							//845=Cramorant
							//854-855=Sinistea line
							//869=Alcremie
							//877=Morpeko
							//890=Eternatus
							isAdditional = true;
							return true;
						}
						String name = entry.Name ?? throw new InvalidOperationException("Name was null!");
						switch (dexNum)
						{
							case 382:
							case 383:
								return key switch
								{
									"primal" => name.StartsWith("Primal", StringComparison.OrdinalIgnoreCase),
									"$" => !name.StartsWith("Primal", StringComparison.OrdinalIgnoreCase),
									_ => throw new InvalidOperationException()
								};
							case 386:
								return key switch
								{
									"$" => name == "Deoxys",
									"attack" => name.StartsWith("Attack"),
									"defense" => name.StartsWith("Defense"),
									"speed" => name.StartsWith("Speed"),
									_ => throw new InvalidOperationException()
								};

							case 413:
								return key switch
								{
									"sandy" => name.StartsWith("Ground"),
									"trash" => name.StartsWith("Steel"),
									"$" or "plant" => name.StartsWith("Grass"),
									_ => throw new InvalidOperationException(),
								};
							case 479:
								//rotom
								return key switch
								{
									"$" => name == "Rotom",
									//'D' => name.EndsWith("Dex"), no dex image for now
									"fan" => name.EndsWith("Fan"),
									"mow" => name.EndsWith("Mow"),
									"heat" => name.EndsWith("Heat"),
									"frost" => name.EndsWith("Frost"),
									"wash" => name.EndsWith("Wash"),
									_ => throw new InvalidOperationException()
								};
							case 487:
								//Giratina
								return key switch
								{
									"origin" => name.StartsWith("Origin"),
									"$" or "altered" => name == "Giratina",
									_ => throw new InvalidOperationException()
								};
							case 492:
								//shaymin
								return key switch
								{
									"sky" => name.StartsWith("Sky"),
									"$" or "land" => name == "Shaymin",
									_ => throw new InvalidOperationException()
								};
							case 555:
								//stupid dumb...
								//Darmanitan
								return key switch
								{
									"$" => name == "Darmanitan",
									"zen" => name == "Zen Darmanitan",
									"galar" => name == "Galarian Darmanitan",
									"galar-zen" => name == "Galarian Zen Darmanitan",
									_ => throw new InvalidOperationException()
								};
							case 641:
							case 642:
							case 645:
								//Tornadus, Thundurus, and Landorus
								return key switch
								{
									"therian" => name.StartsWith("Form"),
									"$" or "incarnate" => !name.StartsWith("Form"),
									_ => throw new InvalidOperationException()
								};
							case 646:
								//Kyurem
								return key switch
								{
									"black" => name.StartsWith("Black"),
									"white" => name.StartsWith("White"),
									"$" => name == "Kyurem",
									_ => throw new InvalidOperationException()
								};
							case 647:
								//Keldeo
								return key switch
								{
									"resolute" => name.StartsWith("Form"),
									"$" or "ordinary" => name == "Keldeo",
									_ => throw new InvalidOperationException()
								};
							case 648:
								//Meloetta
								return key switch
								{
									"pirouette" => name.StartsWith("Form"),
									"$" or "aria" => !name.StartsWith("Form"),
									_ => throw new InvalidOperationException()
								};
							case 658:
								//Greninja
								return key switch
								{
									"ash" or "battle-bond" => name.StartsWith("BBF"),
									"$" => name == "Greninja",
									_ => throw new InvalidOperationException()
								};
							case 681:
								//Aegislash
								return key switch
								{
									"blade" => name.StartsWith("Form"),
									"$" or "shield" => name == "Aegislash",
									_ => throw new InvalidOperationException()
								};
							case 718:
								//Zygarde
								return key switch
								{
									//'L' => name.EndsWith("Cell"), same as rotom dex, skip
									"10" => name.EndsWith("10%"),
									"complete" => name.EndsWith("100%"),
									"$" or "50" => name.EndsWith("50%"),
									_ => throw new InvalidOperationException()
								};
							case 720:
								//Hoopa
								return key switch
								{
									"unbound" => name.EndsWith("Unbound"),
									"$" => name == "Hoopa",
									_ => throw new InvalidOperationException()
								};
							case 741:
								return key switch
								{
									"pau" => name.EndsWith("Psychic"),
									"pom-pom" => name.EndsWith("Electric"),
									"sensu" => name.EndsWith("Ghost"),
									"baile" or "$" => name.EndsWith("Fire"),
									_ => throw new InvalidOperationException()
								};
							case 745:
								return key switch
								{
									"dusk" => name.EndsWith("Dusk"),
									"midnight" => name.EndsWith("Midnight"),
									"$" or "midday" => name.EndsWith("Midday"),
									_ => throw new InvalidOperationException()
								};
							case 746:
								return key switch
								{
									"school" => name.EndsWith("Swarm"),
									"$" or "solo" => name == "Wishiwashi",
									_ => throw new InvalidOperationException()
								};
							case 774:
								//Minior
								if (key == "$" || key.EndsWith("meteor"))
								{
									isAdditional = key != "$";
									return name == "Minior";
								}
								//I wrote this code and I like red, so red will be the default
								isAdditional = key != "red";
								return name.EndsWith("Core");
							case 800:
								return key switch
								{
									"ultra" => name.EndsWith("Ultra Burst"),
									"dusk" => name.EndsWith("Dusk Mane"),
									"dawn" => name.EndsWith("Dawn Wings"),
									"$" => name == "Necrozma",
									_ => throw new InvalidOperationException()
								};
							case 802:
								//there seems to be a duplicate??
								//ignore the duplicate
								return key == "$";
							case 849:
								isAdditional = key.Contains("gmax");
								String switchKey = Regex.Replace(key, "-?gmax", "");
								if (switchKey == "")
								{
									switchKey = "$";
								}
								return switchKey switch
								{
									"low-key" => name.EndsWith("Low Key"),
									"$" or "amped" => name.EndsWith("Amped"),
									_ => throw new InvalidOperationException()
								};
							case 875:
								return key switch
								{
									"noice" => name.StartsWith("Form"),
									"$" or "ice" => name == "Eiscue",
									_ => throw new InvalidOperationException()
								};
							case 888:
							case 889:
								//sword+shield doggos
								return key switch
								{
									"$" or "hero-of-many-battles" => !name.StartsWith("Form"),
									"crowned" => name.StartsWith("Form"),
									_ => throw new InvalidOperationException()
								};
						}
						if (key == "mega")
						{
							return entry.MegaEvolutionBaseEntry != null;
						}
						if (key == "mega-x")
						{
							return entry.Name!.EndsWith(" X");
						}
						if (key == "mega-y")
						{
							return entry.Name!.EndsWith(" Y");
						}
						if (key == "gmax" || key == "$")
						{
							return String.IsNullOrEmpty(entry.Variant);
						}
						switch (key)
						{
							case "alola":
								return "Alolan" == entry.Variant;
							case "galar":
								return "Galarian" == entry.Variant;
						}
						throw new NotImplementedException();
					}
					DexEntry.Builder? FindVariant(out bool isAdditional)
					{
						foreach (var entry in entries)
						{
							if (IsVariant(entry, out isAdditional))
							{
								return entry;
							}
						}
						isAdditional = false;
						return null;
					}
					var entry = FindVariant(out bool isAdditional);
					if (entry == null)
					{
						if (dexNum == 802)
						{
							//had a duplicate for some reason. Skip it
							continue;
						}
						throw new NotImplementedException();
					}
					if (key == "gmax")
					{
						//gmax has no right, no female, and no actual dex entry other than the default one
						entry.AdditionalSpriteImages.Add(MakeImage(false, false, false));
						entry.AdditionalShinySpriteImages.Add(MakeImage(true, false, false));
						continue;
					}
					if (isAdditional)
					{
						//add additional images
						entry.AdditionalSpriteImages.Add(MakeImage(false, false, false));
						entry.AdditionalShinySpriteImages.Add(MakeImage(true, false, false));
						if (form.hasFemale)
						{
							entry.AdditionalFemaleSpriteImages.Add(MakeImage(false, true, false));
							entry.AdditionalShinyFemaleSpriteImages.Add(MakeImage(true, true, false));
						}
					}
					else
					{
						entry.SpriteImage = MakeImage(false, false, false);
						entry.SpriteShinyImage = MakeImage(true, false, false);
						if (form.hasFemale)
						{
							entry.SpriteFemaleImage = MakeImage(false, true, false);
							entry.SpriteShinyFemaleImage = MakeImage(true, true, false);

						}
					}
					if (form.hasRight)
					{
						entry.AdditionalSpriteImages.Add(MakeImage(false, false, true));
						entry.AdditionalShinySpriteImages.Add(MakeImage(true, false, true));
					}
					remaining.Remove(entry);
					remainingDexEntries.Remove(entry);
				}
				if (remaining.Count > 0)
				{
					if (remaining.Count == 1)
					{
						var entry = remaining.First();
						if ((entry.DexNum == 479 && entry.Name!.EndsWith("Dex")) || (entry.DexNum == 718 &&
							entry.Name!.EndsWith("Cell")))
						{
							continue;//ignore
						}
					}
					throw new NotImplementedException();
				}
			}
			return newImages;
		}
		private byte[]? ReadImage(String path)
		{
#pragma warning disable CS0162 // Unreachable code detected
			const bool readImages = false; //set to true to have the importer include image data in the export
			if (readImages)
			{
				return File.ReadAllBytes(path);
			}
			return null;
#pragma warning restore CS0162 // Unreachable code detected
		}
		private ImageRef.Builder MakeImageRef(String filePath)
		{
			return new ImageRef.Builder()
			{
				Filename = Path.GetFileName(filePath),
				Data = ReadImage(filePath),
				DataId = new DataId(null, Guid.NewGuid())
			};
		}
		//why? because I eventually decided it would be easier to just do a GC instead of writing logic to remove old
		//images as things are being updated.
		private void GcStockImages()
		{
			GcList<ImageRef.Builder, ImageRef>(data.Images);
		}
		private void GcList<B, T>(List<B> list) where B : DataItemBuilder<T> where T : IDataItem<T> {

			//collect all references to type "T" in 'data' which can be built using type "B"
			//using reflection to avoid having to update code again...
			Type root = data.GetType();
			HashSet<ItemReference<T>> references = new HashSet<ItemReference<T>>(list.Count);
			Dictionary<Type, GcTypeInfo> typeInfo = new Dictionary<Type, GcTypeInfo>(20);
			Dictionary<Type, Type> enumerableInfo = new Dictionary<Type, Type>(20);
			HashSet<Object> seenObjects = new HashSet<object>();
			Type searchType = typeof(ItemReference<T>);
			foreach (var prop in root.GetProperties())
			{
				if (!prop.CanRead || !prop.CanWrite)
				{
					continue;
				}
				Type listType = prop.PropertyType;
				if (!listType.IsGenericType || listType.GetGenericTypeDefinition() != typeof(List<>))
				{
					continue;
				}
				if (listType == typeof(List<B>))
				{
					//no recursion for you!
					continue;
				}
				Type genericType = listType.GetGenericArguments()[0];
				Type? builderType = genericType.BaseType;
				if (builderType == null || !builderType.IsGenericType || !typeof(DataItemBuilder<>).IsAssignableFrom(
					builderType.GetGenericTypeDefinition()))
				{
					throw new InvalidOperationException("Unknown list type: " + genericType);
				}
				//now we know that is a valid builder. Now to check entries of said builder
				GcProcessType<B, T>(typeInfo, enumerableInfo, references, seenObjects, prop.GetValue(data) ??
					throw new InvalidOperationException($"Data member {prop.Name} was null!"));
				//List<PropertyInfo> singleProps = new List<PropertyInfo>(10);
				//List<PropertyInfo> nullableProps = new List<PropertyInfo>(10);
				//List<PropertyInfo> listProps = new List<PropertyInfo>(10);
				//foreach (var tarProp in genericType.GetProperties())
				//{
				//	switch (tarProp.PropertyType)
				//	{
				//		case Type innerType when innerType == typeof(ItemReference<T>):
				//			singleProps.Add(tarProp);
				//			break;
				//		case Type innerType when innerType == typeof(ItemReference<T>?):
				//			nullableProps.Add(tarProp);
				//			break;
				//		case Type innerType when innerType == typeof(List<ItemReference<T>>):
				//			listProps.Add(tarProp);
				//			break;
				//	}

				//}
				////get the list in question
				//Object? rawList = prop.GetValue(data);
				//if (rawList == null)
				//{
				//	throw new InvalidOperationException($"How was {prop.Name} null?");
				//}
				//var iterable = (IEnumerable)rawList;
				//foreach (var item in iterable)
				//{
				//	foreach (var itemProp in singleProps)
				//	{
				//		//never null. Enforced by the compiler everywhere else
				//		references.Add((ItemReference<T>)itemProp.GetValue(item)!);
				//	}
				//	foreach (var itemProp in nullableProps)
				//	{
				//		var value = itemProp.GetValue(item);
				//		if (value != null)
				//		{
				//			//`(int)(Object)new Nullable<int>(5)` returns 5, so we sholud be safe if value is not null
				//			references.Add((ItemReference<T>)value);
				//		}
				//	}
				//	foreach (var itemProp in listProps)
				//	{
				//		var value = itemProp.GetValue(item);
				//		if (value == null)
				//		{
				//			//not my job to question it
				//			continue;
				//		}
				//		foreach (var val in (IEnumerable<ItemReference<T>>)value)
				//		{
				//			//we aren't looking for lists of nullables, so the value is not null
				//			references.Add(val);
				//		}
				//	}
				//}
			}
			HashSet<Guid> referencedGuids = new HashSet<Guid>(references.Count);
			HashSet<int> referencedIds = new HashSet<int>(references.Count);
			HashSet<String> referencedNames = new HashSet<string>(references.Count);
			foreach (var item in references)
			{
				var guid = item.DataId.Uuid;
				if (guid != Guid.Empty)
				{
					referencedGuids.Add(guid);
				}
				var dbId = item.DataId.DbId;
				if (dbId.HasValue)
				{
					referencedIds.Add(dbId.Value);
				}
				String? displayName = item.DisplayName;
				if (!String.IsNullOrEmpty(displayName))
				{
					referencedNames.Add(displayName);
				}
			}
			HashSet<B> referencedBuilders = new HashSet<B>(references.Count);
			HashSet<B> unreferencedItems = new HashSet<B>(Math.Max(0, list.Count - references.Count));
			foreach (var item in list)
			{
				DataId? idRaw = item.DataId;
				if (idRaw.HasValue)
				{
					var id = idRaw.Value;
					if (id.Uuid != Guid.Empty && referencedGuids.Contains(id.Uuid))
					{
						//referenced
						continue;
					}
					if (id.DbId.HasValue && referencedIds.Contains(id.DbId.Value))
					{
						//referenced
						continue;
					}
				}
				String? dispName = item.ItemReference?.DisplayName;
				if (!String.IsNullOrEmpty(dispName) && referencedNames.Contains(dispName))
				{
					//referenced
					continue;
				}
				unreferencedItems.Add(item);
			}
			int count = 0;
			list.RemoveAll(item =>
			{
				if (unreferencedItems.Contains(item))
				{
					count++;
					return true;
				}
				return false;
			});
			if (count != 0)
			{
				Debugger.Break();
			}
			Console.WriteLine($"Garbage Collected {count} instances of {typeof(B)}");
		}
		private void GcProcessType<B, T>(Dictionary<Type, GcTypeInfo> typeInfoDict,
			Dictionary<Type, Type> enumerableInfo, HashSet<ItemReference<T>> references,
			HashSet<object> seenObjects, object? item) where B : DataItemBuilder<T> where T : IDataItem<T>
		{
			if (item == null)
			{
				return;
			}
			if (!seenObjects.Add(item))
			{
				//we have seen this one already
				return;
			}
			(GcTypeInfo? typeInfo, bool isEnumerable) = GetTypeInfo<T>(typeInfoDict, enumerableInfo, item);
			if (isEnumerable)
			{
				foreach (var inner in (IEnumerable)item)
				{
					GcProcessType<B, T>(typeInfoDict, enumerableInfo, references, seenObjects, inner);
				}
				return;
			}
			if (typeInfo == null)
			{
				throw new WasNullException("Should not be null!!!");
			}
			foreach (var prop in typeInfo.AllProps)
			{
				if (typeInfo.SingleProps.Contains(prop))
				{
					references.Add((ItemReference<T>)prop.GetValue(item)!);
				}
				else if (typeInfo.NullableProps.Contains(prop))
				{
					ItemReference<T>? val = (ItemReference<T>?)prop.GetValue(item);
					if (val.HasValue)
					{
						references.Add(val.Value);
					}
				}
				else if (typeInfo.EnumerableProps.Contains(prop))
				{
					var value = prop.GetValue(item);
					if (value == null)
					{
						//not my job to question it
						continue;
					}
					foreach (var val in (IEnumerable<ItemReference<T>>)value)
					{
						//we aren't looking for lists of nullables, so the value is not null
						references.Add(val);
					}
				}
				else
				{
					GcProcessType<B, T>(typeInfoDict, enumerableInfo, references, seenObjects, prop.GetValue(item));
				}
			}
		}

		private static (GcTypeInfo? typeInfo, bool isEnumerable) GetTypeInfo<T>(Dictionary<Type, GcTypeInfo> typeInfoDict,
			Dictionary<Type, Type> enumerableInfo, object item) where T : IDataItem<T>
		{
			return GetTypeInfo<T>(typeInfoDict, enumerableInfo, item.GetType());
		}
		private static (GcTypeInfo? typeInfo, bool isEnumerable) GetTypeInfo<T>(Dictionary<Type, GcTypeInfo> typeInfoDict,
			Dictionary<Type, Type> enumerableInfo, Type inputType) where T : IDataItem<T>
		{
			Type searchType = inputType;
			bool isEnumerable = false;
			GcTypeInfo? typeInfo;
			if (enumerableInfo.TryGetValue(searchType, out Type? baseType))
			{
				return (null, true);
				//isEnumerable = true;
				//searchType = baseType;
			}
			else if (typeInfoDict.TryGetValue(searchType, out typeInfo))
			{
				return (typeInfo, false);
			}
			//note: the generic version extends the non-generic one
			else if (searchType.GetInterfaces().Contains(typeof(IEnumerable)))
			{
				isEnumerable = true;
				var genericImpl = searchType.FindInterfaces((type, criteria) => type.IsGenericType &&
					type.GetGenericTypeDefinition() == typeof(IEnumerable<>), null);
				if (genericImpl.Length == 1)
				{
					searchType = genericImpl[0].GetGenericArguments()[0];
				}
				else if (genericImpl.Length < 1)
				{
					searchType = typeof(Object);
				}
				else
				{
					//wat?
					throw new InvalidOperationException($"{searchType} implemented IEnumerable with two different generics!");
				}
				enumerableInfo.Add(inputType, searchType);
				return (null, true);
			}
			if (!typeInfoDict.TryGetValue(searchType, out typeInfo))
			{
				//gather type info
				List<PropertyInfo> allProps = new List<PropertyInfo>(searchType.GetProperties());
				HashSet<PropertyInfo> singleProps = new HashSet<PropertyInfo>(10);
				HashSet<PropertyInfo> nullableProps = new HashSet<PropertyInfo>(10);
				HashSet<PropertyInfo> enumerabaleProps = new HashSet<PropertyInfo>(10);
				foreach (var tarProp in allProps)
				{
					switch (tarProp.PropertyType)
					{
						case Type innerType when innerType == typeof(ItemReference<T>):
							singleProps.Add(tarProp);
							break;
						case Type innerType when innerType == typeof(ItemReference<T>?):
							nullableProps.Add(tarProp);
							break;
						case Type innerType when typeof(IEnumerable<ItemReference<T>>).IsAssignableFrom(innerType):
							enumerabaleProps.Add(tarProp);
							break;
					}
				}
				typeInfo = new GcTypeInfo(searchType, allProps, singleProps, nullableProps, enumerabaleProps);
				typeInfoDict.Add(searchType, typeInfo);
			}
			return (typeInfo, isEnumerable);
		}



		private void ReadHeightAndWeights()
		{
			async Task<List<(String name, String requestUri)>> ListPokemon()
			{
				List<(String, String)> entries = new List<(string, string)>(data.DexEntries.Count);
				await foreach (var item in PokeApiHandler.Instance.PerformRequest("pokemon"))
				{
					foreach (JToken token in item["results"] ?? throw new WasNullException())
					{
						entries.Add(((String?)token["name"] ?? throw new WasNullException(),
							(String?)token["url"] ?? throw new WasNullException()));
					}
				}
				return entries;
			}
			async Task<Dictionary<String, JObject>> LoadStats()
			{
				var list = await ListPokemon().ConfigureAwait(false);
				async Task<(String key, JObject value)> FetchStats((String key, String uri) input)
				{
					String uri = input.uri[26..];
					// only expecting one page
					await using var iter = PokeApiHandler.Instance.PerformRequest(uri).GetAsyncEnumerator();
					if (!await iter.MoveNextAsync().ConfigureAwait(false))
					{
						throw new InvalidOperationException();
					}
					var result = iter.Current;
					if (await iter.MoveNextAsync().ConfigureAwait(false))
					{
						//there should only be one page!!!!
						throw new InvalidOperationException();
					}
					return (input.key, result);
				}
				List<Task<(String key, JObject value)>> tasks = new List<Task<(string key, JObject value)>>(list.Count);
				foreach (var item in list)
				{
					//Task.Run
					tasks.Add(FetchStats(item));
				}
				return (await Task.WhenAll(tasks).ConfigureAwait(false)).OrderBy(item=>item.key).
					ToDictionary(item => item.key, item => item.value, StringComparer.OrdinalIgnoreCase);
			}




			//List<(String name, String requestUri)> apiItems = ListPokemon().GetAwaiter().GetResult();
			//Dictionary<String, String> nameToUri = apiItems.ToDictionary(item => item.name, item => item.requestUri,
			//	StringComparer.OrdinalIgnoreCase);
			Dictionary<String, JObject> nameToStat = LoadStats().GetAwaiter().GetResult();
			HashSet<String> remaining = new HashSet<String>(nameToStat.Keys, StringComparer.OrdinalIgnoreCase);

			
			JObject? GetStats(DexEntry.Builder entry, out String key)
			{
				if (entry.DexNum == 555)
				{
					key = "";
					return null;//workaround so Zen doesn't go missing in the misseed list for some reason
				}
				String entryName = entry.Name!;
				key = entryName;
				if (!String.IsNullOrEmpty(entry.Variant))
				{
					if (entryName.IndexOf(entry.Variant!,StringComparison.OrdinalIgnoreCase) < 0)
					{
						return null;
						//throw new InvalidOperationException();
					}
					String replacement = entry.Variant switch
					{
						"Alolan"=>"Alola",
						"Galarian"=>"Galar",
						_ => throw new InvalidOperationException()
					};
					key = Regex.Replace(entryName, "(\\w+) (.+)", $"$2-{replacement}");
				}
				//.Replace("'", ""); for Farfetch'd
				//.Replace(".", ""); for Mr. Mime
				key = key.Replace(' ', '-').Replace("-(provisional)", "").Replace("'", "").
					Replace(".", "");
				if (nameToStat.TryGetValue(key, out JObject? stats))
				{
					return stats;
				}
				if (entryName.StartsWith("mega", StringComparison.OrdinalIgnoreCase))
				{
					key = Regex.Replace(key, "Mega-(.+?)(-[XY])?$", "$1-Mega$2");
				}
				if (nameToStat.TryGetValue(key, out stats))
				{
					return stats;
				}
				//switch (entry.DexNum)
				//{
				//	case 413:
				//		key = key.Replace("Ground", "sandy", StringComparison.OrdinalIgnoreCase).
				//			Replace("Steel", "trash", StringComparison.OrdinalIgnoreCase).
				//			Replace("Grass", "plant", StringComparison.OrdinalIgnoreCase);
				//		break;
				//}
				if (key.Contains("-"))
				{
					//try swapping bits
					string[] parts = key.Split('-');
					List<String> workingList = parts.ToList();
					for (int i = 0; i < parts.Length; i++)
					{
						//try the 'i'th item in every 'j' position
						for (int j = 0; j < parts.Length; j++)
						{
							if (j == i)
							{
								//skip
								continue;
							}
							workingList.Clear();
							workingList.AddRange(parts);
							if (i < j)
							{
								workingList.Insert(j, parts[i]);
								workingList.RemoveAt(i);
							}
							else
							{
								//need to do it in the other direction
								workingList.RemoveAt(i);
								workingList.Insert(j, parts[i]);
							}
							if (nameToStat.TryGetValue(String.Join('-', workingList), out stats))
							{
								return stats;
							}
						}
					}
				}
				return null;

				/*switch (entry.DexNum)
				{
					//Deoxys
					case 386:// attack, defense, normal, speed
						if (!key.Contains("-"))
						{
							//normal
							key += "-normal";
						}
						else
						{
							//should be covered already
							throw new InvalidOperationException();
						}
						break;
					//rotom
					case 479:
						//use values from stock for dex rotom
						key = "Rotom";
						return nameToStat[key];
					case 487:
						//giratina
						key = "Giratina" == entryName ? "giratina-altered" : "giratina-origin";
						break;
					case 492:
						//shaymin
						key = "Shaymin" == entryName ? "shaymin-land" : "shaymin-sky";
						break;
					case 550:
						//basculin: blue-striped, red-striped
						//same block for each
						key = "basculin-blue-striped";
						break;
				}
				if (nameToStat.TryGetValue(key, out stats))
				{
					return stats;
				}
				throw new NotImplementedException();*/
			}
			//list mismatching entries for someone else to solve
			var entriesRemaining = new HashSet<DexEntry.Builder>(data.DexEntries.Where(entry=>entry.DexNum != 0 && entry.Variant != "Delta"));
			var keysRemaining = new HashSet<String>(nameToStat.Keys.Where(key=>!key.Contains("gmax") && !key.Contains("totem")),
				StringComparer.OrdinalIgnoreCase);
			foreach (var entry in data.DexEntries)
			{
				if (entry.DexNum == 0 || entry.Variant == "Delta" || entry.Name!.Contains("Delta"))
				{
					//we don't care about "egg" or Delta
					continue;
				}
				//determine if there is a matching key or not
				var result = GetStats(entry, out String key);
				if (result != null)
				{
					//handled
					entriesRemaining.Remove(entry);
					keysRemaining.Remove(key);
				}
			}
			StringBuilder listOutput = new StringBuilder(1 >> 16);
			listOutput.Append("DexNum\tName\tVariant\tTypes\tPokéapi key\n");
			List<DexEntry.Builder> missedEntries = entriesRemaining.OrderBy(entry => entry.DexNum).ToList();
			List<String> missedKeys = keysRemaining.OrderBy(key => key).ToList();
			for(int i = 0; i < missedEntries.Count || i < missedKeys.Count; i++)
			{
				DexEntry.Builder? entry = i < missedEntries.Count ? missedEntries[i] : null;
				String? key = i < missedKeys.Count ? missedKeys[i] : null;
				String type = String.Join('/', entry?.PrimaryType?.DisplayName, entry?.SecondaryType?.DisplayName);
				type = type.Trim('/');
				listOutput.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\n", entry?.DexNum, entry?.Name, entry?.Variant, type, key);
			}
			File.WriteAllText("apiMiss.tsv", listOutput.ToString());
			/*foreach (DexEntry.Builder entry in data.DexEntries)
			{
				if (entry.DexNum == 0)
				{
					//we don't care about "egg"
					continue;
				}
				////here;
				//String fetchUri = GetUri(entry, out String key);
				////get relative Uri since our caching api take relative endpoints
				//fetchUri = fetchUri[26..^1];
				//async Task<JObject> FetchEntry()
				//{
				//	// only expecting one page
				//	await using var iter = PokeApiHandler.PerformRequest(fetchUri).GetAsyncEnumerator();
				//	if (!await iter.MoveNextAsync())
				//	{
				//		throw new InvalidOperationException();
				//	}
				//	var result = iter.Current;
				//	if (await iter.MoveNextAsync())
				//	{
				//		//there should only be one page!!!!
				//		throw new InvalidOperationException();
				//	}
				//	return result;
				//}
				var jObject = GetStats(entry, out String key);// FetchEntry().GetAwaiter().GetResult();
				//read dem stats!
				//apparently, all official pokemon heights are in decimeters...
				int heightDecimeters = (int?)jObject["height"] ?? throw new WasNullException();
				int weightHectograms = (int?)jObject["weight"] ?? throw new WasNullException();
				entry.AverageHeight = new Height("deimeters", heightDecimeters);
				entry.AverageWeight = new Weight("hectograms", weightHectograms);
				remaining.Remove(key);
			}*/
			if (remaining.Count > 0)
			{
				throw new InvalidOperationException();
			}
			throw new NotImplementedException();
		}

		private List<(DexEntry.Builder entry, String kind, String value)> ReadDescriptionsAndEvoInfo()
		{
			List<(DexEntry.Builder entry, String kind, String value)> result = new List<(DexEntry.Builder entry,
				string kind, string value)>(data.DexEntries.Count / 3);

		}
		private record ImageData
		{
			public string Filename { get; }
			public int DexNum { get; }
			public bool Mega { get; }
			public bool? MegaVariantIsX { get; }
			public bool Shiny { get; }
			public bool Gigantamax { get; }
			public bool Female { get; }
			public char? Variant { get; }
			public bool BackImage { get; }
			public string? Misc { get; }

			public ImageData(String filename, int dexNum, bool mega, bool? megaVariantIsX, bool gigantamax,
				bool female, bool shiny, char? variant, bool backImage, String? misc)
			{
				Filename = filename;
				DexNum = dexNum;
				Mega = mega;
				MegaVariantIsX = megaVariantIsX;
				Gigantamax = gigantamax;
				Female = female;
				Shiny = shiny;
				Variant = variant;
				BackImage = backImage;
				Misc = misc;
			}
		}
		private record GcTypeInfo
		{
			public Type Type { get; }
			public ReadOnlySet<PropertyInfo> SingleProps { get; }
			public ReadOnlySet<PropertyInfo> NullableProps { get; }
			public ReadOnlySet<PropertyInfo> EnumerableProps { get; }
			public ReadOnlyCollection<PropertyInfo> AllProps { get; }
			public GcTypeInfo(Type type, List<PropertyInfo> allProps, ISet<PropertyInfo> singleProps, ISet<PropertyInfo> nullableProps,
				ISet<PropertyInfo> enumerableProps)
			{
				Type = type;
				SingleProps = new ReadOnlySet<PropertyInfo>(singleProps);
				NullableProps = new ReadOnlySet<PropertyInfo>(nullableProps);
				EnumerableProps = new ReadOnlySet<PropertyInfo>(enumerableProps);
				AllProps = allProps.AsReadOnly();
			}
		}
	}
	class PokeSpriteDexEntry
	{
		public int dexNum;
		public Dictionary<String, String> name;
		public Dictionary<String, String> slug;
		/// <summary>
		/// English slug for file names
		/// </summary>
		public String Slug => slug["eng"];
		/// <summary>
		/// union of gen-7 and gen-8, reading gen-7 first, and then gen-8
		/// </summary>
		public Dictionary<String, PokeSpriteForm> forms;
		public PokeSpriteDexEntry(int dexNum)
		{
			this.dexNum = dexNum;
			name = new Dictionary<string, string>(4);
			slug = new Dictionary<string, string>(3);
			forms = new Dictionary<string, PokeSpriteForm>(1);
		}
	}
	class PokeSpriteForm
	{
		public String? alias;
		public bool unofficial, unofficialFemale;
		public bool unofficialLegacy;
		public bool isPrevGen;
		public bool hasRight, hasFemale;
		public PokeSpriteForm()
		{
			alias = null;
			unofficial = unofficialFemale = unofficialLegacy = isPrevGen = hasRight = hasFemale = false;
		}

	}
	public static class Extensions
	{
		public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> enumerable)
		{
			foreach (var item in enumerable)
			{
				if (item != null)
				{
					yield return item;
				}
			}
		}
	}

	[Serializable]
	public class WasNullException : Exception
	{
		public WasNullException() { }
		public WasNullException(string message) : base(message) { }
		public WasNullException(string message, Exception inner) : base(message, inner) { }
		protected WasNullException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
