using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;
using Pokerole.Core;

namespace Pokerole.Tools
{
	class InitialDataImporter
	{
		//private const String csvFetchUrl = "https://raw.githubusercontent.com/XShadeSlayerXx/PokeRole-Discord.py-Base/master/";
		private const String csvFetchUrl = "https://raw.githubusercontent.com/SirIntellegence/PokeRole-Discord.py-Base/typofix3/";
		private const string MOVE_MISSING_DESCRIP = "This move is missing";
		private readonly HashSet<Guid> unevolvedEntries = new HashSet<Guid>();
		private readonly HashSet<Guid> hasMegaEvolution = new HashSet<Guid>();
		private PokeroleXmlData data;
		private XmlSerializer xmlSerializer = new XmlSerializer(typeof(PokeroleXmlData));
		private readonly Dictionary<String, Move.Builder> movesByName = new Dictionary<string, Move.Builder>();
		private readonly Dictionary<String, Item.Builder> itemsByName = new Dictionary<string, Item.Builder>();
		private readonly Dictionary<String, DexEntry.Builder> monByName = new Dictionary<string, DexEntry.Builder>();
		private readonly Dictionary<String, Ability.Builder> abilitiesByName = new Dictionary<string, Ability.Builder>();
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
			ReadPrimaryImages();




			StringWriter writer = new StringWriter();
			xmlSerializer.Serialize(writer, data);
			File.WriteAllText("output.xml", writer.ToString());
			Console.WriteLine(writer.ToString());

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
			foreach (var line in File.ReadAllLines(file))
			{
				Move.Builder? builder;
				string[] fields = line.Split(new char[] { ',' }, 10);
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
					throw new InvalidOperationException("Move name missing");
				}
				moves.Add(name, builder);
			}
			//these moves should exist, but don't, so create dummy entries for them
			String[] missingMoves =
			{
				"Poltergeist",
				"Behemoth Blade",
				"Behemoth Bash"
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
			Guid guid = Guid.Empty;
			//someone mispelled something...
			String moveName = fields[0].Replace("Behemot ", "Behemoth ");
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
				if (first)
				{
					//skip over the header
					first = false;
					continue;
				}
				String[] items = line.Split(',');
				if (monByName.ContainsKey(items[1]))
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
				if (!monByName.TryGetValue(id[1], out DexEntry.Builder? builder))
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
					Rank rank = ParseEnum<Rank>(rawRank);
					if (!movesByName.TryGetValue(moveName, out Move.Builder? move))
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
		private void ReadPrimaryImages()
		{
			//build dex notation dict
			//skip "Egg"
			var monByDexNotation = data.DexEntries.Where(item=>item.DexNum != 0).ToLookup(item =>
			{
				//foreach (var item in data.DexEntries)
				//{
				int dexNum = item.DexNum!.Value;
				StringBuilder builder = new StringBuilder(20);
				builder.Append(dexNum.ToString("D3"));
				char c = (item.Variant) switch
				{
					"Alolan" => 'A',
					"Galarian" => 'G',
					"BBF" => 'B',
					"Delta" => 'D',
					_ => '\0'
				};
				if (c != 0)
				{
					builder.Append(c);
				}
				String name = item.Name!;
				if (item.MegaEvolutionBaseEntry.HasValue)
				{
					builder.Append('M');
					if (name.Contains("Charizard") || name.Contains("Mewtwo"))
					{
						builder.Append(name[^1]);
					}
				}
				if (name.StartsWith("Primal"))
				{
					builder.Append('P');
				}
				return builder.ToString();
			});
			//this may not be fully portable... Oh well
			//path to pokerole-companion
			//if you don't have it, git clone it (https://github.com/paxwort/Pokerole-Companion.git) and have
			//the copy in the same directory as this git repo instance
			//                      pokerole-tools (root)-|
			//                      pokerole-tools------| |
			//                                bin ----| | |
			//                                Debug-| | | |
			// netcoreapp3.1 ---------------------V V V V V
			const String pokeroleCompanionDir = "../../../../../Pokerole-Companion";
			const String primaryImageDir = "PokeroleUI2/Graphics/Sprites/Tinified";
			String imagePath = Path.Combine(Directory.GetCurrentDirectory(), pokeroleCompanionDir,
				primaryImageDir);
			var filenames = Directory.GetFiles(imagePath);
			var filesByDexNotation = filenames.ToLookup(path =>
			{
				String working = Path.GetFileName(path);
				working = working.Replace("_Dream.png", "");
				String[] parts = working.Split('_');
				String key = parts[0];
				string last = parts[^1];
				if (last == "X" || last == "Y") //X or Y
				{
					if (parts[1] != "Unown")
					{
						key += last;
					}
				}
				else if (last == "Primal")
				{
					key += "P";
				}
				else if (last == "Ash")
				{
					key += "B";
				}
				return key;
			});
			HashSet<String> remainingMon = new HashSet<string>();
			HashSet<String> remainingFiles = new HashSet<string>();
			foreach (var item in monByDexNotation)
			{
				remainingMon.Add(item.Key);
			}
			foreach (var item in filesByDexNotation)
			{
				remainingFiles.Add(item.Key);
			}
			HashSet<String> missing = new HashSet<string>();
			foreach (var pairing in monByDexNotation)
			{
				if (pairing.Count() > 1)
				{
					//handle that later...
					continue;
				}
				DexEntry.Builder entry = pairing.First();
				if (entry.PrimaryImage.HasValue)
				{
					//already done
					remainingMon.Remove(pairing.Key);
					remainingFiles.Remove(pairing.Key);
					continue;
				}
				if (!filesByDexNotation.Contains(pairing.Key))
				{
					missing.Add(pairing.Key);
					remainingMon.Remove(pairing.Key);
					remainingFiles.Remove(pairing.Key);
					continue;
					throw new InvalidOperationException($"Could not find images for {pairing.Key}");
				}
				var files = filesByDexNotation[pairing.Key];
				if (files.Count() == 1)
				{
					//that was easy
					String path = files.First();
					ImageRef.Builder imageBuilder = new ImageRef.Builder
					{
						Filename = Path.GetFileName(path),
						Data = File.ReadAllBytes(path),
						DataId = new DataId(null, Guid.NewGuid())
					};
					data.Images.Add(imageBuilder);
					entry.PrimaryImage = imageBuilder.ItemReference;
					remainingMon.Remove(pairing.Key);
					remainingFiles.Remove(pairing.Key);
					continue;
				}
				//we need to figure out what is the primary image

				String? primary = (entry.Name)switch
				{
					//we shall make 'F' the primary image for now
					"Unown" => "201_Unown_F_Dream",
					"Castform" => "351_Castform_Normal_Dream",
					//umm... we will go with plant for now
					"Burmy" => "412_Burmy_Plant_Cloak_Dream",
					"Wormadam" => "413_Wormadam_Plant_Cloak_Dream",
					//east?
					"Shellos" => "422_Shellos_East_Sea_Dream",
					"Gastrodon" => "423_Gastrodon_East_Sea_Dream",
					//you only see othe other form on a sunny day
					"Cherrim" => "421_Cherrim_Overcast_Form_Dream",
					"Arceus" => "493_Arceus_Normal_Dream",
					//apparently that one has two?
					"Pansage" => "511_Pansage_Dream",
					"Basculin" => "550_Basculin_Blue_Stripe_Form_Dream",
					"Deerling" => "585_Deerling_Autumn_Form_Dream",
					"Sawsbuck" => "586_Sawsbuck_Autumn_Form_Dream",
					//chosen by google coin flip
					"Frillish" => "592_Frillish_Male_Dream",
					//other one
					"Jellicent" => "593_Jellicent_Female_Dream",
					"Genesect" => "649_Genesect_Dream",
					"Vivillon" => "666_Vivillon_Dream",
					//red is my favorite color
					"Flabebe" => "669_Flabébé_Red_Flower_Dream",
					"Floette" => "670_Floette_Red_Flower_Dream",
					"Florges" => "671_Florges_Red_Flower_Dream",
					"Furfrou" => "676_Furfrou_Dream",
					"Aegislash" => "681_Aegislash_Dream",
					//coin flip chose male last time... So doing female this time
					"Meowstic" => "678_Meowstic_Female_Dream",
					"Silvally" => "773_Silvally_Normal_Dream",
					"Mimikyu" => "778_Mimikyu_Dream",
					_ => null
				};
				if (primary == null)
				{
					Debugger.Break();
				}
				else
				{
					primary = files.First(itemPath => Path.GetFileNameWithoutExtension(itemPath) == primary);
					ImageRef.Builder imageBuilder = new ImageRef.Builder
					{
						Filename = Path.GetFileName(primary),
						Data = File.ReadAllBytes(primary),
						DataId = new DataId(null, Guid.NewGuid())
					};
					data.Images.Add(imageBuilder);
					entry.PrimaryImage = imageBuilder.ItemReference;
					foreach (var item in files)
					{
						if (item == primary)
						{
							continue;
						}
						imageBuilder = new ImageRef.Builder
						{
							Filename = Path.GetFileName(primary),
							Data = File.ReadAllBytes(primary),
							DataId = new DataId(null, Guid.NewGuid())
						};
						data.Images.Add(imageBuilder);
						entry.AdditionalImages.Add(imageBuilder.ItemReference!.Value);
					}
					remainingMon.Remove(pairing.Key);
					remainingFiles.Remove(pairing.Key);
				}
			}
			foreach (var key in remainingMon)
			{
				var grouping = monByDexNotation[key];
				var files = filesByDexNotation[key];
				if (files.Count() < 1)
				{
					//missing
					continue;
				}
				Dictionary<String, String> formeDict;
				ImageRef.Builder imageBuilder;
				HashSet<DexEntry.Builder> remainingEntries = new HashSet<DexEntry.Builder>(grouping);
				Regex extractRegex;
				String baseMonName;
				String? emptyStringForme;
				Dictionary<String, String> formeNameMapping = new Dictionary<string, string>(10);
				List<String> nullEntries = new List<string>(4);
				switch (key)
				{
					case "386": //Deoxys
						extractRegex = new Regex("386_Deoxys_(\\w+?)_Forme_Dream");
						baseMonName = "Deoxys";
						emptyStringForme = "Normal";
						break;
					case "413":
						extractRegex = new Regex("413_Wormadam_(\\w+?)_Cloak_Dream");
						baseMonName = "Wormadam";
						formeNameMapping.Add("Plant", "Grass");
						formeNameMapping.Add("Sandy", "Ground");
						formeNameMapping.Add("Trash", "Steel");
						emptyStringForme = null;
						break;
					case "479":
						extractRegex = new Regex("479_Rotom_(\\w+?)_(?:Rotom|Forme)_Dream");
						baseMonName = "Rotom";
						emptyStringForme = "Normal";
						break;
					case "487":
						extractRegex = new Regex("487_Giratina_(\\w+?)_Forme_Dream");
						baseMonName = "Giratina";
						emptyStringForme = "Altered";
						break;
					case "492":
						extractRegex = new Regex("492_Shaymin_(\\w+?)_Forme_Dream");
						baseMonName = "Shaymin";
						emptyStringForme = "Land";
						break;
					case "555":
					case "555G":
						extractRegex = new Regex("555_Darmanitan(?:_(\\w+?))?_Dream");
						baseMonName = "Darmanitan";
						emptyStringForme = "";
						formeNameMapping.Add("Zen Mode", "Zen");
						formeNameMapping.Add("", "");
						break;
					case "641":
						extractRegex = new Regex("641_Tornadus(_T)?_Dream");
						baseMonName = "Tornadus";
						emptyStringForme = "";
						formeNameMapping.Add("", "");
						formeNameMapping.Add(" T", "Form");
						break;
					case "642":
						extractRegex = new Regex("642_Thundurus(_T)?_Dream");
						baseMonName = "Thundurus";
						emptyStringForme = "";
						formeNameMapping.Add("", "");
						formeNameMapping.Add(" T", "Form");
						break;
					case "645":
						extractRegex = new Regex("645_Landorus(_T)?_Dream");
						baseMonName = "Landorus";
						emptyStringForme = "";
						formeNameMapping.Add("", "");
						formeNameMapping.Add(" T", "Form");
						break;
					case "646":
						extractRegex = new Regex("646_(?:(Black|White)_)?Kyurem_Dream");
						baseMonName = "Kyurem";
						emptyStringForme = "";
						break;
					case "647":
						extractRegex = new Regex("647_Keldeo(_R)?_Dream");
						baseMonName = "Keldeo";
						emptyStringForme = "";
						formeNameMapping.Add("", "");
						formeNameMapping.Add(" R", "Form");
						break;
					case "648":
						extractRegex = new Regex("648_Meloetta(_P)?_Dream");
						baseMonName = "Meloetta";
						emptyStringForme = "";
						formeNameMapping.Add("", "");
						formeNameMapping.Add(" P", "Form");
						break;
					case "681":
						extractRegex = new Regex("681_Aegislash_(Blade|Shield)_Forme_Dream");
						baseMonName = "Aegislash";
						emptyStringForme = "";
						formeNameMapping.Add("Blade", "");
						formeNameMapping.Add("Shield", "Form");
						//It would take too long to figure out how to do that nicely... Maybe do it by hand later?
						formeNameMapping.Add("", "Ignored");
						break;
					case "718":
						//I don't want to spend the time to get an image done for that...
						nullEntries.Add("Cell");
						extractRegex = new Regex("718_Zygarde(?:_(\\w+))_Dream");
						baseMonName = "Zygarde";
						formeNameMapping.Add("Cell", "Cell");
						formeNameMapping.Add("Complete", "100%");
						formeNameMapping.Add("", "50%");
						formeNameMapping.Add("10 Percent", "10%");
						emptyStringForme = "50%";
						break;
					case "720":
						extractRegex = new Regex("720_Hoopa-(Confined|Unbound)_Dream");
						baseMonName = "Hoopa";
						emptyStringForme = "Confined";
						break;
					default:
						continue;

				}
				formeDict = files.ToDictionary(path => extractRegex.Match(Path.GetFileNameWithoutExtension(
					path)).Groups[1].Value.Replace("_", " "));
				foreach (var item in nullEntries)
				{
					formeDict.Add(item, "null");
				}
				if (formeNameMapping.Count > 0)
				{
					//expecting a 1:1 mapping
					Debug.Assert(formeNameMapping.Count == formeDict.Count);
					formeDict = formeDict.ToDictionary(pair => formeNameMapping[pair.Key], pair => pair.Value);
				}
				foreach (var entry in grouping)
				{
					if (entry.PrimaryImage.HasValue)
					{
						//done
						remainingEntries.Remove(entry);
						continue;
					}
					String forme = entry.Name!.Replace(baseMonName, "").Trim();
					if (forme == "")
					{
						forme = emptyStringForme ??
							throw new InvalidOperationException("Was not expecting an non-forme mon!");
					}
					String file = formeDict[forme];
					if (file == "null")
					{
						//we don't have an iamge for that one
						remainingEntries.Remove(entry);
						continue;
					}
					imageBuilder = new ImageRef.Builder
					{
						Filename = Path.GetFileName(file),
						Data = File.ReadAllBytes(file),
						DataId = new DataId(null, Guid.NewGuid())
					};
					entry.PrimaryImage = imageBuilder.ItemReference;
					data.Images.Add(imageBuilder);
					remainingEntries.Remove(entry);
				}
				if (remainingEntries.Count > 0)
				{
					//item was missed
					Debugger.Break();
				}
			}
			if (remainingMon.Count > 0 || remainingFiles.Count > 0)
			{
				//not fully implemented
				Debugger.Break();
			}
		}


	}
}
