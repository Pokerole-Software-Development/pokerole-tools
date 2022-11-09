export class TypeTableData{
	types: TypeTableEntry[] = [];
	// types: TypeTableEntry[] = [];
	public static createDefault(): TypeTableData {
		//populate built-in type effectiveness in bulbapedia order by defense (for simplicity)
		//colors grabbed from "Dex Elements.png"
		type PayloadEntry = {
			type: BuiltInType, backgroundColor: number, weaknesses: BuiltInType[] | null,
			resistances: BuiltInType[] | null, immunities: BuiltInType[] | null
		}
		//#region Data
		var payload = <PayloadEntry[]>[];// new PayloadEntry();
		payload.push(
			{
				type: BuiltInType.Normal,
				backgroundColor: 0xFF9b9074,
				weaknesses: [
					BuiltInType.Fighting
				],
				resistances: null,
				immunities: [
					BuiltInType.Ghost
				]
			},
			{
				type: BuiltInType.Typeless,
				backgroundColor: 0xFF767266,
				weaknesses: null,
				resistances: null,
				immunities: null
			},
			{
				type: BuiltInType.Fire,
				backgroundColor: 0xFFe45633,
				weaknesses: [
					BuiltInType.Ground,
					BuiltInType.Rock,
					BuiltInType.Water,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Fairy,
					BuiltInType.Fire,
					BuiltInType.Grass,
					BuiltInType.Ice,
					BuiltInType.Steel,
				],
				immunities: null
			},
			{
				type: BuiltInType.Fighting,
				backgroundColor: 0xFF9a4e34,
				weaknesses: [
					BuiltInType.Fairy,
					BuiltInType.Flying,
					BuiltInType.Psychic,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Dark,
					BuiltInType.Rock,
				],
				immunities: null
			},
			{
				type: BuiltInType.Water,
				backgroundColor: 0xFF497e97,
				weaknesses: [
					BuiltInType.Electric,
					BuiltInType.Grass,
				],
				resistances: [
					BuiltInType.Fire,
					BuiltInType.Ice,
					BuiltInType.Steel,
					BuiltInType.Water,
				],
				immunities: null
			},
			{
				type: BuiltInType.Flying,
				backgroundColor: 0xFF8b96ad,
				weaknesses: [
					BuiltInType.Electric,
					BuiltInType.Ice,
					BuiltInType.Rock,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Fighting,
					BuiltInType.Grass,
				],
				immunities: [
					BuiltInType.Ground
				]
			},
			{
				type: BuiltInType.Grass,
				backgroundColor: 0xFF6ea94d,
				weaknesses: [
					BuiltInType.Bug,
					BuiltInType.Fire,
					BuiltInType.Flying,
					BuiltInType.Ice,
					BuiltInType.Poison,
				],
				resistances: [
					BuiltInType.Electric,
					BuiltInType.Grass,
					BuiltInType.Ground,
					BuiltInType.Water,
				],
				immunities: null
			},
			{
				type: BuiltInType.Poison,
				backgroundColor: 0xFFa3557e,
				weaknesses: [
					BuiltInType.Ground,
					BuiltInType.Psychic,
				],
				resistances: [
					BuiltInType.Fighting,
					BuiltInType.Poison,
					BuiltInType.Bug,
					BuiltInType.Grass,
					BuiltInType.Fairy,
				],
				immunities: null
			},
			{
				type: BuiltInType.Electric,
				backgroundColor: 0xFFf7b636,
				weaknesses: [
					BuiltInType.Ground
				],
				resistances: [
					BuiltInType.Electric,
					BuiltInType.Flying,
					BuiltInType.Steel,
				],
				immunities: null
			},
			{
				type: BuiltInType.Ground,
				backgroundColor: 0xFFc5a24f,
				weaknesses: [
					BuiltInType.Grass,
					BuiltInType.Ice,
					BuiltInType.Water,
				],
				resistances: [
					BuiltInType.Poison,
					BuiltInType.Rock,
				],
				immunities: [
					BuiltInType.Electric
				]
			},
			{
				type: BuiltInType.Psychic,
				backgroundColor: 0xFFe96d87,
				weaknesses: [
					BuiltInType.Bug,
					BuiltInType.Dark,
					BuiltInType.Ghost,
				],
				resistances: [
					BuiltInType.Fighting,
					BuiltInType.Psychic,
				],
				immunities: null
			},
			{
				type: BuiltInType.Rock,
				backgroundColor: 0xFFa28743,
				weaknesses: [
					BuiltInType.Fighting,
					BuiltInType.Grass,
					BuiltInType.Ground,
					BuiltInType.Steel,
					BuiltInType.Water,
				],
				resistances: [
					BuiltInType.Fire,
					BuiltInType.Flying,
					BuiltInType.Normal,
					BuiltInType.Poison,
				],
				immunities: null
			},
			{
				type: BuiltInType.Ice,
				backgroundColor: 0xFF51bab8,
				weaknesses: [
					BuiltInType.Fighting,
					BuiltInType.Fire,
					BuiltInType.Rock,
				],
				resistances: [
					BuiltInType.Ice
				],
				immunities: null
			},
			{
				type: BuiltInType.Bug,
				backgroundColor: 0xFF9faf30,
				weaknesses: [
					BuiltInType.Fire,
					BuiltInType.Flying,
					BuiltInType.Rock,
				],
				resistances: [
					BuiltInType.Fighting,
					BuiltInType.Grass,
					BuiltInType.Ground,
				],
				immunities: null
			},
			{
				type: BuiltInType.Dragon,
				backgroundColor: 0xFF684181,
				weaknesses: [
					BuiltInType.Dragon,
					BuiltInType.Fairy,
					BuiltInType.Ice,
				],
				resistances: [
					BuiltInType.Electric,
					BuiltInType.Fire,
					BuiltInType.Grass,
					BuiltInType.Water,
				],
				immunities: null
			},
			{
				type: BuiltInType.Ghost,
				backgroundColor: 0xFF585c88,
				weaknesses: [
					BuiltInType.Dark,
					BuiltInType.Ghost,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Poison,
				],
				immunities: [
					BuiltInType.Normal,
					BuiltInType.Fighting,
				]
			},
			{
				type: BuiltInType.Dark,
				backgroundColor: 0xFF664f3b,
				weaknesses: [
					BuiltInType.Bug,
					BuiltInType.Fairy,
					BuiltInType.Fighting,
				],
				resistances: [
					BuiltInType.Dark,
					BuiltInType.Ghost,
				],
				immunities: [
					BuiltInType.Psychic
				]
			},
			{
				type: BuiltInType.Steel,
				backgroundColor: 0xFF979392,
				weaknesses: [
					BuiltInType.Fighting,
					BuiltInType.Fire,
					BuiltInType.Ground,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Dragon,
					BuiltInType.Fairy,
					BuiltInType.Flying,
					BuiltInType.Grass,
					BuiltInType.Ice,
					BuiltInType.Normal,
					BuiltInType.Psychic,
					BuiltInType.Rock,
					BuiltInType.Steel,
				],
				immunities: [
					BuiltInType.Poison
				]
			},
			{
				type: BuiltInType.Fairy,
				backgroundColor: 0xFFdd9ba7,
				weaknesses: [
					BuiltInType.Poison,
					BuiltInType.Steel,
				],
				resistances: [
					BuiltInType.Bug,
					BuiltInType.Dark,
					BuiltInType.Fighting,
				],
				immunities: [
					BuiltInType.Dragon
				]
			}
		);
		//#endregion
		var result = new TypeTableData();
		for (var item of payload) {
			var entry = new TypeTableEntry(item.type.toString());
			entry.builtInType = item.type;
			entry.backgroundColor = item.backgroundColor;
			var lazyList: [BuiltInType[]| null, String[]][] = [
				[item.weaknesses, entry.weaknesses],
				[item.resistances, entry.resistances],
				[item.immunities, entry.immunities]
			];
			for (var pair of lazyList) {
				if (!pair[0]) {
					continue;
				}
				for (var type of pair[0]) {
					pair[1].push(type.toString());
				}
			}
			result.types.push(entry);
		}
		return result;
	}

}
export enum BuiltInType {
	Normal = "Normal",//normal is first so it will be the default
	Typeless = "Typeless",
	Fire = "Fire",
	Fighting = "Fighting",
	Water = "Water",
	Flying = "Flying",
	Grass = "Grass",
	Poison = "Poison",
	Electric = "Electric",
	Ground = "Ground",
	Psychic = "Psychic",
	Rock = "Rock",
	Ice = "Ice",
	Bug = "Bug",
	Dragon = "Dragon",
	Ghost = "Ghost",
	Dark = "Dark",
	Steel = "Steel",
	Fairy = "Fairy"
}
export class TypeTableEntry{
	name: string;
	/**
	 * The color for this type expressed as 0xAARRGGBB or undefinded if one has not been defined
	 */
	backgroundColor?: number;
	resistances: string[] = [];
	weaknesses: string[] = [];
	immunities: string[] = [];
	builtInType?: BuiltInType;
	constructor(name: string) {
		this.name = name;
	}

}
