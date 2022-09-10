import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/module.mjs";
import { ToObjectFalseType } from "@league-of-foundry-developers/foundry-vtt-types/src/types/helperTypes.js";
import { POKEROLE } from "../helpers/config.js";
import { PokeroleItem, PokeroleItemData } from "./item.js";

export class ActorRollData{
	item?: PokeroleItemData
}
/**
 * Extend the base Actor document by defining a custom roll data structure which is ideal for the Simple system.
 * @extends {Actor}
 */
export class PokeroleActor extends Actor {
	// /**
	//  * @override
	//  */
	// override get system(): PokeroleActorData & SystemDataField {
	// 	return <PokemonActorData & SystemDataField>super.system;
	// }

	/** @override */
	prepareData() {
		// Prepare data for the actor. Calling the super version of this executes
		// the following, in order: data reset (to clear active effects),
		// prepareBaseData(), prepareEmbeddedDocuments() (including active effects),
		// prepareDerivedData().
		super.prepareData();
	}

	/** @override */
	prepareBaseData() {
		// Data modifications in this step occur before processing embedded
		// documents or derived data.

	}
	// readonly system: PokeroleActorData = super.system as PokeroleActorData;
	/**
	 * @override
	 * Augment the basic actor data with additional dynamic data. Typically,
	 * you'll want to handle most of your calculated/derived data in this step.
	 * Data calculated in this step should generally not exist in template.json
	 * (such as ability modifiers rather than ability scores) and should be
	 * available both inside and outside of character sheets (such as if an actor
	 * is queried and has a roll executed directly from it).
	 */
	prepareDerivedData() {
		var actorData = this.system;
		// const actorData = this.data;
		const data = actorData as unknown as PokeroleActorData;

		data.confidence = this._calculateConfidence(data.nature)

		// Make separate methods for each Actor type (character, npc, etc.) to keep
		// things organized.
		//Note: the type of data is OBJECT! We are just showing the data via interfaces that will likely
		//be discarded at runtime
		switch (this.type) {
			case POKEROLE.ActorTypes.mon:// "pokemon":
				this._preparePokemonData(data as PokemonActorData);
				break;
			case POKEROLE.ActorTypes.trainer:
				this._prepareTrainerData(data as TrainerActorData);
				break;
			case POKEROLE.ActorTypes.rival:
				this._prepareRivalData(data as RivalActorData);
				break;
		}
	}

	/**
	 * Prepare Character type specific data
	 */
	_preparePokemonData(monData: PokemonActorData) {
		
		monData.defense = monData.stats.vitality;
		monData.specialDefense = POKEROLE.UseInsightForSpecialDefense ?
			monData.stats.insight :
			monData.stats.vitality;
	}
	/**
	 * Prepare Trainer type specific data
	 */
	_prepareTrainerData(trainderData: TrainerActorData) {

		// // Make modifications to data here. For example:
		// const data = actorData.data;
	}
	/**
	 * Prepare Rival type specific data
	 */
	_prepareRivalData(rivalData: RivalActorData) {
		// Page 475, Storyteller Note
		// "Rivals don’t need Attributes or Skill Points, simply assume they will Roll one or two more dice
		// than the players."

		// // Make modifications to data here. For example:
		// const data = actorData.data;
	}

	_calculateConfidence(nature: Nature | string): number {
		if (typeof nature === 'string') {
			//convert to enum
			var val = (<any>Nature)[<string>nature] as Nature;
			if (!val) {
				return 0;
			}
			// const temp: keyof typeof Nature = <string>nature as any;
			// if (!temp) {
			// 	return 0;
			// }
			nature = val;
			// nature = keyof typeof Nature[<string>nature]; (<any>Nature)[nature];
		}
		switch (nature) {
			case Nature.Adamant:
			case Nature.Serious:
			case Nature.Timid:
				return 4;
			case Nature.Careful:
			case Nature.Lonely:
			case Nature.Quiet:
				return 5;
			case Nature.Bashful:
			case Nature.Naughty:
			case Nature.Rash:
				return 6;
			case Nature.Docile:
			case Nature.Hasty:
			case Nature.Impish:
			case Nature.Naive:
			case Nature.Sassy:
				return 7;
			case Nature.Calm:
			case Nature.Lax:
			case Nature.Mild:
			case Nature.Relaxed:
				return 8;
			case Nature.Bold:
			case Nature.Brave:
			case Nature.Hardy:
			case Nature.Quirky:
				return 9;
			case Nature.Gentle:
			case Nature.Jolly:
			case Nature.Modest:
				return 10;
		}
	}
	
	/**
	 * Override getRollData() that's supplied to rolls.
	 */
	getRollData() : ActorRollData {
		const data = super.getRollData();

		// Prepare character roll data.
		this._getCharacterRollData(data);

		return data;
	}

	/**
	 * Prepare character roll data.
	 */
	_getCharacterRollData(data: any) {
		if (this.type !== 'character') return;
		console.log(data);
		// Add level for easier access, or fall back to 0.
		if (data.attributes.level) {
			data.lvl = data.attributes.level.value ?? 0;
		}
	}

}
export enum Nature {
	Adamant,
	Bashful,
	Bold,
	Brave,
	Calm,
	Careful,
	Docile,
	Gentle,
	Hardy,
	Hasty,
	Impish,
	Jolly,
	Lax,
	Lonely,
	Mild,
	Modest,
	Naive,
	Naughty,
	Quiet,
	Quirky,
	Rash,
	Relaxed,
	Sassy,
	Serious,
	Timid
}
//Strongly typed data containers
export interface PokeroleActorData{//extends SystemDataField(This breaks too many things...) {//extends foundry.abstract.Document<any,any>{
	nature: Nature;
	health: {
		min: number;
		max: number;
		value: number;
	}
	will: {
		min: number;
		max: number;
		value: number;
	}
	//calucated data
	confidence: number;
}
export interface HumanActorData extends PokeroleActorData{
	age: number;
	money: number;
}
export interface RivalActorData extends HumanActorData{
	/**
	 * Id of the player related to this rival
	 */
	relatedPlayer: string;
}
export interface PlayerStats {
	strength: number,
	dexterity: number,
	vitality: number,
	insight: number,
	brawl: number,
	evasion: number,
	alert: number,
	athletic: number,
	nature: number,
	stealth: number,
	allure: number,
	etiquette: number,
	intimidate: number,
	perform: number,
	tough: number,
	cool: number,
	beauty: number,
	clever: number,
	cute: number,
	misc1: number
}
export interface PlayerActorData extends PokeroleActorData {
	stats: PlayerStats;
}
export interface PokemonStats extends PlayerStats {
	special: number,
	channel: number,
	clash: number,
	happiness: number,
	loyalty: number,
}
export interface PokemonActorData extends PlayerActorData {
	stats: PokemonStats,
	battleCount: number,
	vicoryCount: number,
	//calculated data
	defense: number,
	specialDefense: number
}
export interface TrainerStats extends PlayerStats{
	throw: number,
	evasion: number,
	weapons: number,
	crafts: number,
	lore: number,
	medicine: number,
	science: number,
	misc2: number,
	misc3: number,
	misc4: number,
}
export interface TrainerActorData extends PlayerActorData, HumanActorData {
	stats: TrainerStats,
	monSeen: number,
	monCaught: number
}
export function getActorStat(data: PlayerActorData, stat: string): number | undefined  {
	//only return data from stats
	if (!POKEROLE.StatsUtils.isStat(stat)) {
		return undefined;
	}
	return getProperty(data.stats, stat) as number;
}
export function setActorStat(data: PlayerActorData, stat: string, val: number){
	//only set data from stats
	if (!POKEROLE.StatsUtils.isStat(stat)) {
		return;
	}
	setProperty(data.stats, stat, val);
}

