import { TypeTableData, TypeTableEntry } from "./PokemonTypes.js"

export class TypeManager {
	//currently not supporting custom types
	private readonly _typeData: TypeTableData = TypeTableData.createDefault();
	private readonly _typeTable: Map<string, TypeTableEntry> = new Map();
	constructor() {
		for (const type of this._typeData.types) {
			this._typeTable.set(type.name, type);
		}
	}
	listTypes(): string[]{
		return [... this._typeTable.keys()];
	}
	/**
	 * Get the color assigned to the given type, undefined if the type doesn't have a color,
	 * or null if there is no such type. The number is formatted as 0xAARRGGBB. AA should always be FF
	 * @param typename type to get the color for
	 */
	getColorForType(typename: string): number | null | undefined {
		var type = this._typeTable.get(typename);
		return type === undefined ? null : type.backgroundColor;
	} 
	/**
	 * Calculate how effective {@param attackType} is against the combinations of {@param defenseTypes}.
	 * A null result inidates an immunity, 0 indicates "effective", values greater than 0 indicate
	 * "super effective", and values less than 0 indicate "ineffective".
	 * The number returned is suitable for adding straight to a dice roll
	 * @param attackType 
	 * @param defenseTypes 
	 */
	calculateEffectiveness(attackType: string, ... defenseTypes: string[]) : number | null {
		if (defenseTypes.length < 1) {
			return 0;
		}
		var result = 0;
		for (const defense of defenseTypes) {
			var entry = this._typeTable.get(defense);
			if (!entry) {
				continue;
			}
			if (entry.resistances.indexOf(attackType) > -1) {
				result--;
			}
			else if (entry.weaknesses.indexOf(attackType) > -1) {
				result++;
			}
			else if (entry.immunities.indexOf(attackType) > -1) {
				return null;
			}
		}
		return result;
	}
}
