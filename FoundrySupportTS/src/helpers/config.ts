import { TypeManager } from "./TypeManager.js"
export const POKEROLE = {
	//ASCII art becase everyone else is doing it...
	ASCII:
`_____________________________________
______     _                  _      
| ___ \\   | |     /          | |     
| |_/ /__ | | _____ _ __ ___ | | ___ 
|  __/ _ \\| |/ / _ \\ '__/ _ \\| |/ _ \\
| | | (_) |   <  __/ | | (_) | |  __/
\\_|  \\___/|_|\\_\\___|_|  \\___/|_|\\___|
=====================================`,
	UseInsightForSpecialDefense: true,
	Bar: {

	},
	ActorTypes: {
		mon: "pokemon",
		trainer: "trainer",
		rival: "rival"
	},
	KnownStats: [
		//player
		"strength",
		"dexterity",
		"vitality",
		"insight",
		"brawl",
		"evasion",
		"alert",
		"athletic",
		"nature",
		"stealth",
		"allure",
		"etiquette",
		"intimidate",
		"perform",
		"tough",
		"cool",
		"beauty",
		"clever",
		"cute",
		"misc1",
		//mon
		"happiness",
		"loyalty",
		"battleCount",
		"vicoryCount",
		"special",
		"channel",
		"clash",
		//trainer
		"throw",
		"weapons",
		"crafts",
		"lore",
		"medicine",
		"science",
		"misc2",
		"misc3",
		"misc4",
	],
	StatsUtils: {
		getStatType: function (val: string): ("trainer" | "mon" | "both" | undefined) {

			switch (val) {
				//player
				case "strength":
				case "dexterity":
				case "vitality":
				case "insight":
				case "brawl":
				case "evasion":
				case "alert":
				case "athletic":
				case "nature":
				case "stealth":
				case "allure":
				case "etiquette":
				case "intimidate":
				case "perform":
				case "tough":
				case "cool":
				case "beauty":
				case "clever":
				case "cute":
				case "misc1":
					return "both";
				//mon
				case "happiness":
				case "loyalty":
				case "battleCount":
				case "vicoryCount":
				case "special":
				case "channel":
				case "clash":
					return "mon";
				//trainer
				case "throw":
				case "weapons":
				case "crafts":
				case "lore":
				case "medicine":
				case "science":
				case "misc2":
				case "misc3":
				case "misc4":
					return "trainer";
			}
			return undefined;
		},
		isStat: function (val: string): boolean {
			return this.getStatType(val) !== undefined;
		},
		getStatList: function (val: ("trainer" | "mon")): string[]{
			var result = [];
			for (var stat of POKEROLE.KnownStats) {
				var typeResult = POKEROLE.StatsUtils.getStatType(stat);
				if (typeResult === val || typeResult === "both") {
					result.push(stat);
				}
			}
			return result;
		}
	},
	//Currently not supporting custom types
	TypeManager: new TypeManager(),
	
};

// Define constants here, such as:
// POKEROLE.foobar = {
//   'bas': 'POKEROLE.bas',
//   'bar': 'POKEROLE.bar'
// };
