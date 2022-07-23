import { ActorData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/module.mjs";

export class ActorRollData{

}
/**
 * Extend the base Actor document by defining a custom roll data structure which is ideal for the Simple system.
 * @extends {Actor}
 */
export class PokeroleActor extends Actor {

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
		const actorData = this.data;
		const data = actorData.data as PokeroleActorData;
		const flags = actorData.flags["Pokerole"] || {};

		data.confidence = this._calculateConfidence(data.nature)

		// Make separate methods for each Actor type (character, npc, etc.) to keep
		// things organized.
		switch (actorData.type) {
			case "pokemon":
				this._preparePokemonData(actorData);
				break;
			case "trainer":
				this._prepareTrainerData(actorData);
				break;
			case "rival":
				this._prepareRivalData(actorData);
				break;
		}
	}

	/**
	 * Prepare Character type specific data
	 */
	_preparePokemonData(actorData: ActorData) {

		// Make modifications to data here. For example:
		const data = actorData.data;
	}
	/**
	 * Prepare Trainer type specific data
	 */
	_prepareTrainerData(actorData: ActorData) {

		// Make modifications to data here. For example:
		const data = actorData.data;
	}
	/**
	 * Prepare Rival type specific data
	 */
	_prepareRivalData(actorData: ActorData) {
		// Page 475, Storyteller Note
		// "Rivals don’t need Attributes or Skill Points, simply assume they will Roll one or two more dice
		// than the players."

		// Make modifications to data here. For example:
		const data = actorData.data;
	}

	_calculateConfidence(nature: Nature) : number {
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
		if (this.data.type !== 'character') return;
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
export interface PokeroleActorData{
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
	confidence: number;
}
export interface HumanActorData extends PokeroleActorData{

}
export interface RivalActorData extends HumanActorData{

}
export interface PlayerActorData extends PokeroleActorData {
	dexterity: {
		min: number;
		max: number;
		value: number;
	}
	alert: {
		min: number;
		max: number;
		value: number;
	}

}
export interface PokemonActorData extends PlayerActorData{
	
}
export interface TrainerActorData extends Merge<PlayerActorData, HumanActorData>{
	
}

