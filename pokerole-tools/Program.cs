using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Pokerole.Core;

namespace Pokerole.Tools
{
	class Program
	{
		private const String csvFetchUrl = "https://raw.githubusercontent.com/XShadeSlayerXx/PokeRole-Discord.py-Base/master/";
		static void Main(string[] args)
		{
			PokeroleXmlData data = new PokeroleXmlData();
			//read them datas!!!
			ReadMoves(data);
			ReadDexEntries(data);

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PokeroleXmlData));
			xmlSerializer.Serialize(Console.Out, data);
			//Console.WriteLine("Hello World!");
		}
		private static String FetchFileIfNeeded(String file)
		{
			String path = Path.Combine(Directory.GetCurrentDirectory(), file);
			if (!File.Exists(path))
			{
				//download it!
				using (var client = new WebClient())
				{
					client.DownloadFile(csvFetchUrl + file, path);
				}
			}
			return path;
		}
		private static T ParseEnum<T>(string val) where T : Enum{
			return (T)Enum.Parse(typeof(T), val.Replace(" ", ""), true);
		}
		private static void ReadMoves(PokeroleXmlData data)
		{
			String file = FetchFileIfNeeded("pokeMoveSorted.csv");
			foreach (var line in File.ReadAllLines(file))
			{
				Move.Builder builder = new Move.Builder();
				builder.DataId = new DataId(null, Guid.NewGuid());
				string[] fields = line.Split(new char[] { ',' }, 10);
				builder.Name = fields[0];
				String item = fields[1];
				BuiltInType type = (BuiltInType)Enum.Parse(typeof(BuiltInType), item, true);
				ITypeDefinition typeDef = TypeManager.GetBuiltInType(type);
				builder.Type = new ItemReference<ITypeDefinition>(typeDef.DataId, typeDef.Name);

				builder.MoveCategory = ParseEnum<MoveCategory>(fields[2]);
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
				skill = String.IsNullOrEmpty(item) ? BuiltInSkill.Undefined : ParseEnum<BuiltInSkill>(item);
				skillDef = SkillManager.GetBuiltInSkill(skill);
				builder.Accuracy.Add(new ItemReference<ISkill>(skillDef.DataId, skillDef.Name));

				item = fields[7];
				skill = String.IsNullOrEmpty(item) ? BuiltInSkill.Undefined : ParseEnum<BuiltInSkill>(item);
				skillDef = SkillManager.GetBuiltInSkill(skill);
				builder.Accuracy.Add(new ItemReference<ISkill>(skillDef.DataId, skillDef.Name));

				//target
				builder.MoveTarget = ParseEnum<MoveTarget>(fields[8]);

				item = fields[9];
				if (!String.IsNullOrEmpty(item) && "-" != item)
				{
					builder.Effects.Add(item);
				}
				data.Moves.Add(builder);
			}
		}
		private static void ReadDexEntries(PokeroleXmlData data)
		{
			FetchFileIfNeeded("PokeroleStats.csv");
		}
	}
}
