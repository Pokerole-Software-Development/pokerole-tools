import { ItemData } from "@league-of-foundry-developers/foundry-vtt-types/src/foundry/common/data/data.mjs";
import { ActorRollData, PokeroleActor } from "./actor.js";
// import { Item } from "@league-of-foundry-developers/foundry-vtt-types"

/**
 * Extend the basic Item with some very simple modifications.
 * @extends {Item}
 */
export class PokeroleItem extends Item {

	// get system(): PokeroleItemData & SystemDataField {
	// 	return <PokeroleItemData & SystemDataField>super.system;
	// }
	/**
	 * Augment the basic Item data model with additional dynamic data.
	 */
	prepareData() {
		// As with the actor class, items are documents that can have their data
		// preparation methods overridden (such as prepareBaseData()).
		super.prepareData();
	}

	/**
	 * Prepare a data object which is passed to any Roll formulas which are created related to this Item
	 * @private
	 */
	getRollData(): ActorRollData{//refine return value later...
		// If present, return the actor's roll data.
		if (!this.actor) {
			return {};
		}
		const rollData = (this.actor as PokeroleActor).getRollData();
		rollData.item = foundry.utils.deepClone(this.data.data) as PokeroleItemData;

		return rollData;
	}

	/**
	 * Handle clickable rolls.
	 * @param {Event} event   The originating click event
	 * @private
	 */
	async roll() {
		if (!(game instanceof Game)) {
			//wat??
			return undefined;
		}
		const item = this.system as unknown as PokeroleItemData;

		// Initialize chat data.
		const speaker = ChatMessage.getSpeaker({ actor: this.actor ?? undefined });
		const rollMode = game.settings.get('core', 'rollMode');
		const label = `[${this.type}] ${item.name}`;

		if (this.type === 'move') {
			//that is a doosie. It gets its own method
			return await this.rollMove(item as MoveItemData);
		}
		//don't know of any other item that rolls dice ATM
		var innerData = item;
		ChatMessage.create({
			speaker: speaker,
			rollMode: rollMode,
			flavor: label,
			content: innerData.description ?? ''
		});
		return undefined;
// 		// If there's no roll data, send a chat message.
// 		if (!this.data.data.formula) {
// 			ChatMessage.create({
// 				speaker: speaker,
// 				rollMode: rollMode,
// 				flavor: label,
// 				content: innerData.description ?? ''
// 			});
// 			return undefined;
// 		}
// 		// Otherwise, create a roll and send a chat message from it.
// 		else {
// 			// Retrieve roll data.
// 			const rollData = this.getRollData();
// ask how to roll against multiple opponents
// 			// Invoke the roll and submit it to chat.
// 			const roll = new Roll(rollData.item.formula, rollData);
// 				// If you need to store the value first, uncomment the next line.
// 			// let result = await roll.roll({async: true});
// 			roll.toMessage({
// 				speaker: speaker,
// 				rollMode: rollMode,
// 				flavor: label,
// 			});
// 			return roll;
// 		}
	}
	async rollMove(move: MoveItemData) {
		if (!(game instanceof Game) || !game.user) {
			//wat??
			return undefined;
		}
		var targets = game.user.targets;
		return undefined;
	}
}
export interface PokeroleItemData {
	name: string;
	description: string;
}
export interface AccessoryItemData extends PokeroleItemData{

}
export interface AbilityItemData extends PokeroleItemData{
	//todo: add special logics???
}
export enum MoveCategory{
	Physical,
	Special,
	Support
}
export enum MoveTarget {
	Foe,
	RandomFoe,
	AllFoes,
	User,
	OneAlly,
	UserAndAllies,
	Area,
	Battlefield,
	BattlefieldAndArea
}
export interface MoveItemData extends PokeroleItemData{
	//oh dear...
	// name: string;covered by base
	// description: string;
	power: number;
	moveCategory: MoveCategory;
	type: string,
	moveTarget: MoveTarget;
	ranged: boolean;
	primaryAccuracyStat: string,
	primaryAccuracyIsNegative: boolean;
	secondaryAccuracyStat: string,
	reducedAccuracy: number;
	damageStat?: string,
	secondaryDamageStat?: string,
	secondaryDamageIsNegative: boolean;
	damageModifier: number
	hasSpecialAccuracyMod: boolean;
	hasSpecialDamageMod: boolean;
	additionalInfo: string;
	effects: string[];
}
