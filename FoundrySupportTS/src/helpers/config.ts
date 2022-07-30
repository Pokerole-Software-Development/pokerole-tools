export const POKEROLE = {
	//ASCII art becase everyone else is doing it...
	//TODO: Fix? It doesn't show up in the browser log properly
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
		"evasion",
		"weapons",
		"crafts",
		"lore",
		"medicine",
		"science",
		"misc2",
		"misc3",
		"misc4",
	],
	isStat: function (val: string): boolean {
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
			//mon
			case "happiness":
			case "loyalty":
			case "battleCount":
			case "vicoryCount":
			case "special":
			case "channel":
			case "clash":
			//trainer
			case "throw":
			case "evasion":
			case "weapons":
			case "crafts":
			case "lore":
			case "medicine":
			case "science":
			case "misc2":
			case "misc3":
			case "misc4":
				return true;
		}
		return false;
	}
		
	
};

// Define constants here, such as:
// POKEROLE.foobar = {
//   'bas': 'POKEROLE.bas',
//   'bar': 'POKEROLE.bar'
// };