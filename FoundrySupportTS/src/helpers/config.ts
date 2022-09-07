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
	getReadableColor: function (backgroundColor: number): number {
		if (backgroundColor % 1 !== 0) {
			//not a whole number?!?!?
			console.warn(`The color ${backgroundColor} was not an integer`);
			backgroundColor = Math.round(backgroundColor);
		}
		if ((backgroundColor & 0xFFFFFF) !== backgroundColor) {
			//remove the alpha, which is apparently the last two digits??? This is stupid...
		}
		var red = (backgroundColor & 0xFF0000) >> 4;
		var green = (backgroundColor & 0x00FF00) >> 2;
		var blue = backgroundColor & 0x0000FF;
		//taken from https://stackoverflow.com/a/3943023/1366594
		return (red * 0.299 + green * 0.587 + blue * 0.114) > 186 ? 0x000000 : 0xFFFFFF;
	}
	
};

// Define constants here, such as:
// POKEROLE.foobar = {
//   'bas': 'POKEROLE.bas',
//   'bar': 'POKEROLE.bar'
// };
